using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Flex.Buffers;
using Flex.Buffers.Adaptors;

namespace Flex
{

    public class TypicalMessage
    {
        public string StringProp { get; set; }

        public int IntProp { get; set; }

        public Guid GuidProp { get; set; }

        public DateTime DateProp { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var s = new Serializer(true);
            var stream = new MemoryStream();

            var message = new TypicalMessage()
            {
                DateProp = DateTime.Now,
                GuidProp = Guid.NewGuid(),
                IntProp = 123,
                StringProp = "Hello"
            };

            var bytes = new byte[100];
            // s.Serialize(message,stream);
            var b2 = new SingleSegmentBuffer(bytes);
            var writer2 = new Writer<SingleSegmentBuffer>(b2, null);
            s.Serialize(message, ref writer2);

            BenchmarkBaseline(message);
            Benchmark(bytes, s, message);
        }

        private static void Benchmark(byte[] bytes, Serializer s, TypicalMessage message)
        {
            Console.WriteLine("Benchmarking Flex");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10_000_000; i++)
            {
                var b = new SingleSegmentBuffer(bytes);
                var writer = new Writer<SingleSegmentBuffer>(b, null);
                s.Serialize(message, ref writer);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        
        private static void BenchmarkBaseline(TypicalMessage message)
        {
            var bytes = new byte[100];
            Console.WriteLine("Benchmarking baseline - faster than this is probably not possible");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10_000_000; i++)
            {
                //Note to reader:
                // moving each row to static methods with aggressive inlining makes it identical to this
                Encoding.UTF8.GetBytes(message.StringProp, bytes);
                BitConverter.TryWriteBytes(bytes, message.IntProp);
                message.GuidProp.TryWriteBytes(bytes);
                BitConverter.TryWriteBytes(bytes, message.DateProp.Ticks);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
    }
}
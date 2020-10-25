using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Flex.Buffers;
using Flex.Buffers.Adaptors;

namespace Flex
{
    public sealed class Other
    {
        public int IntOther { get; set; }
    }

    public sealed class TypicalMessage
    {
        public Other OtherProp { get; set; }
        
        public string StringProp { get; set; } = "";

        public int IntProp { get; set; }

        public Guid GuidProp { get; set; }

        public DateTime DateProp { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new Serializer(new SerializerOptions(false,new []{typeof(TypicalMessage)}));

            var message = new TypicalMessage
            {
                DateProp = DateTime.Now,
                GuidProp = Guid.NewGuid(),
                IntProp = 123,
                StringProp = "Hello",
                OtherProp = new Other()
                {
                    IntOther = 123,
                }
            };

            var session = serializer.CreateSession();
            var bytes = new byte[100];
            // s.Serialize(message,stream);
            var b2 = new SingleSegmentBuffer(bytes);
            var writer2 = new Writer<SingleSegmentBuffer>(b2, session);
            serializer.Serialize(message, ref writer2);

      //      BenchmarkBaseline(message);
            Benchmark(bytes, serializer,session, message);
        }

        private static void Benchmark(byte[] bytes, Serializer s, SerializerSession session, TypicalMessage message)
        {
            Console.WriteLine("Benchmarking Flex");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10_000_000; i++)
            {
                var b = new SingleSegmentBuffer(bytes);
                var writer = new Writer<SingleSegmentBuffer>(b, session);
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
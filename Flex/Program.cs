using System;
using System.Diagnostics;
using System.IO;
using Flex.Buffers;
using Flex.Buffers.Adaptors;

namespace Flex
{
    public class TypicalMessage
    {
        public  string StringProp { get; set; }

        public  int IntProp { get; set; }

        public  Guid GuidProp { get; set; }

        public  DateTime DateProp { get; set; }
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
            var writer2 = new Writer<SingleSegmentBuffer>(b2,null);
            s.Serialize(message,ref writer2);

            Benchmark(bytes, s, message);
        }

        private static void Benchmark(byte[] bytes, Serializer s, TypicalMessage message)
        {
            //
            // var ms = new MemoryStream {Capacity = 100, Position = 0};
            // s.Serialize(message, ms);
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10_000_000; i++)
            {
                // ms.Position = 0;
             var b = new SingleSegmentBuffer(bytes);
             var writer = new Writer<SingleSegmentBuffer>(b, null);
                s.Serialize(message,ref writer);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
    }
}
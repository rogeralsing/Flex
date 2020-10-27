using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Apex.Serialization;
using Flex.Buffers;
using Flex.Buffers.Adaptors;
using Flex.Generics;

namespace Flex
{
    public sealed class TypicalMessage
    {
        public string StringProp { get; set; } = "";
        public int IntProp { get; set; }
        public Guid GuidProp { get; set; }
        public DateTime DateProp { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {

            var message = new TypicalMessage
            {
                DateProp = DateTime.Now,
                GuidProp = Guid.NewGuid(),
                IntProp = 123,
                StringProp = "Hello"
            };

            
            BenchmarkFlex(message);
        //    BenchmarkBaseline(message);
        BenchmarkFlexUntyped(message);
        BenchmarkFlexMemoryStream(message);


        //    BenchmarkApex(message);
        }

        private static void BenchmarkApex(TypicalMessage message)
        {
            var s = new MemoryStream();
            var settings = new Settings
            {
                AllowFunctionSerialization = false,
                SerializationMode = Mode.Tree,
                SupportSerializationHooks = false,
            };
            settings.MarkSerializable(typeof(TypicalMessage));
            
            var binary = Binary.Create(settings);
           
            binary.Write(message,s);
            Console.WriteLine("Benchmarking Apex " + s.Length);
            binary.Write(message,s);
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                s.Position = 0;
                binary.Write (message,s);
            }
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        private static void BenchmarkFlex(TypicalMessage message)
        {
            var serializer = new Serializer(new SerializerOptions(false, new[] {typeof(TypicalMessage)}));
            var session = serializer.CreateSession();
            var bytes = new byte[100];
            var b = new SingleSegmentBuffer(bytes);
            var writer = new Writer<SingleSegmentBuffer>(b, session);
            //warmup
            serializer.Serialize(message, ref writer);
            
            Console.WriteLine("Benchmarking Flex ");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                serializer.Serialize(message, ref writer);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        
        private static void BenchmarkFlexMemoryStream(TypicalMessage message)
        {
            var serializer = new Serializer(new SerializerOptions(false, new[] {typeof(TypicalMessage)}));
            var session = serializer.CreateSession();
            var ms = new MemoryStream();
            //warmup
            serializer.Serialize(message, ms,session);
            
            Console.WriteLine("Benchmarking Flex ");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                ms.Position = 0;
                serializer.Serialize(message, ms,session);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }
        
        private static void BenchmarkFlexUntyped(TypicalMessage message)
        {
            var serializer = new Serializer(new SerializerOptions(false, new[] {typeof(TypicalMessage)}));
            var session = serializer.CreateSession();
            var bytes = new byte[100];
            var b = new SingleSegmentBuffer(bytes);
            var writer = new Writer<SingleSegmentBuffer>(b, session);
            
            //warmup
            serializer.SerializeUntyped(message, ref writer);

            Console.WriteLine("Benchmarking Flex Untyped");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                b = new SingleSegmentBuffer(bytes);
                writer = new Writer<SingleSegmentBuffer>(b, session);
                serializer.SerializeUntyped(message, ref writer);
            }

            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        private static void BenchmarkBaseline(TypicalMessage message)
        {
            var bytes = new byte[100];
            Console.WriteLine("Benchmarking baseline - faster than this is probably not possible");
            var sw = Stopwatch.StartNew();

            for (var i = 0; i < 10_000_000; i++)
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
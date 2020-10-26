using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Apex.Serialization;
using Flex.Buffers;
using Flex.Buffers.Adaptors;
using Flex.Generics;
using MessagePack;

namespace Flex
{
    [MessagePackObject]
    public sealed class TypicalMessage
    {
        [Key(0)]
        public string StringProp { get; set; } = "";

        [Key(1)]
        public int IntProp { get; set; }

        [Key(2)]
        public Guid GuidProp { get; set; }
        
        [Key(3)]
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

            

            BenchmarkBaseline(message);
            BenchmarkFlex(message);
            BenchmarkApex(message);
            BenchmarkMessagePack(message);
        }

        private static void BenchmarkMessagePack(TypicalMessage message)
        {
            var bytes = new byte[100];
            Console.WriteLine("Benchmarking MessagePack " );
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                var w = new MessagePackWriter(new SingleSegmentBuffer(bytes));
                MessagePackSerializer.Serialize(ref w, message);
            }
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
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
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10_000_000; i++)
            {
                s.Position = 0;
                binary.Write(message,s);
            }
            Console.WriteLine(sw.Elapsed.TotalMilliseconds);
        }

        private static void BenchmarkFlex(TypicalMessage message)
        {
            var serializer = new Serializer(new SerializerOptions(false, new[] {typeof(TypicalMessage)}));

            var session = serializer.CreateSession();
            var bytes = new byte[100];
            var b2 = new SingleSegmentBuffer(bytes);
            var writer2 = new Writer<SingleSegmentBuffer>(b2, session);
            var d = TypedSerializers<SingleSegmentBuffer, Tree, TypicalMessage>.SerializeWithManifest;
            d(message, ref writer2);
            var size = writer2.BufferPos;
            
            Console.WriteLine("Benchmarking Flex " + size);
            var sw = Stopwatch.StartNew();
            //    var ss = TypedSerializers<SingleSegmentBuffer, Tree, TypicalMessage>.SerializeWithManifest;
            for (var i = 0; i < 10_000_000; i++)
            {
                var b = new SingleSegmentBuffer(bytes);
                var writer = new Writer<SingleSegmentBuffer>(b, session);
                serializer.Serialize(message, ref writer);
                // ss(message, ref writer);
                // writer.Commit();
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
using System.Buffers;
using System.Runtime.CompilerServices;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class StringSerializer<TBuffer> : ValueSerializer<string, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            writer.Write((byte)5);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(string value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(string value, ref Writer<TBuffer> writer)
        {
            WriteStatic(value, ref writer);
        }
        
        
        public override Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(string)});
            var call = Expression.Call(typedWriter,method, value);
            return call;
        }
    }
}
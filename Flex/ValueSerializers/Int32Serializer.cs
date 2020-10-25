using System.Buffers;
using System.Runtime.CompilerServices;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class Int32Serializer<TBuffer> : ValueSerializer<int, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = sizeof(int);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            ByteSerializer<TBuffer>.WriteStatic(2, ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(int value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(int value, ref Writer<TBuffer> writer)
        {
            WriteStatic(value, ref writer);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(int)});
            var call = Expression.Call(typedWriter,method, value);
            return call;
        }
    }
}
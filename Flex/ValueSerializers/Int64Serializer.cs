using System.Buffers;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class Int64Serializer<TBuffer> : ValueSerializer<long, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const long Size = sizeof(long);

        public override void Write(long value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            if (writeManifest) writer.Write((byte) 6);
            writer.Write(value);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter, bool includeManifest)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] { typeof(long) });
            var call = Expression.Call(typedWriter, method, value);
            return call;
        }
    }

    public class Int64Deserializer : ValueDeserializer<long>
    {
        public override long Read(ref Reader reader) => reader.ReadInt64();
    }
}

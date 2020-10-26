using System.Buffers;
using System.IO;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class Int32Serializer<TBuffer> : ValueSerializer<int, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = sizeof(int);

        public override void Write(int value, ref Writer<TBuffer> writer,bool writeManifest)
        {
            if (writeManifest) writer.Write((byte) 2);
            writer.Write(value);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter, bool includeManifest)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(int)});
            var call = Expression.Call(typedWriter, method, value);
            return call;
        }
    }

    public class Int32Deserializer : ValueDeserializer<int>
    {
        public override int Read(ref Reader reader) => reader.ReadInt32();
    }
}
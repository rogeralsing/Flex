using System;
using System.Buffers;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class GuidSerializer<TBuffer> : ValueSerializer<Guid, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = 16;

        public override void Write(Guid value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            if (writeManifest) writer.Write((byte) 3);
            writer.Write(value);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter, bool includeManifest)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(Guid)});
            var call = Expression.Call(typedWriter, method, value);
            return call;
        }
    }
    
    public class GuidDeserializer : ValueDeserializer<Guid>
    {
        public override Guid Read(ref Reader reader) => reader.ReadGuid();
    }
}
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class GuidSerializer<TBuffer> : ValueSerializer<Guid, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = 16;

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            writer.Write((byte) 3);
        }

        public override void Write(Guid value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(Guid)});
            var call = Expression.Call(typedWriter, method, value);
            return call;
        }
    }
}
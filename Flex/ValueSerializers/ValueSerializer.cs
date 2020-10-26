using System.Buffers;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;

namespace Flex.ValueSerializers
{
    public abstract class ValueSerializer<TBuffer> where TBuffer : IBufferWriter<byte>
    {
        public virtual Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var method = GetType().GetMethod("WriteStatic");
            return Expression.Call(method, value, typedWriter);
        }

        public abstract void WriteObject(object value, ref Writer<TBuffer> writer, bool writeManifest);
    }

    public abstract class ValueSerializer<TValue, TBuffer> : ValueSerializer<TBuffer>
        where TBuffer : IBufferWriter<byte>
    {
        public override void WriteObject(object value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            Write((TValue) value, ref writer, writeManifest);
        }

        public abstract void Write(TValue value, ref Writer<TBuffer> writer, bool writeManifest);
    }
}
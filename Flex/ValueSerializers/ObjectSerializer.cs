using System.Buffers;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

#pragma warning disable 693

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public sealed class ObjectSerializer<TValue, TStyle, TBuffer> : ValueSerializer<TValue, TBuffer>
        where TBuffer : IBufferWriter<byte>
    {
        private readonly ObjectSerializerDelegate<TBuffer, TStyle, TValue> 
            _serializer;
        
        public ObjectSerializer(ObjectSerializerDelegate<TBuffer, TStyle, TValue> serializer) => 
            _serializer = serializer;

        public override void Write(TValue value, ref Writer<TBuffer> writer, bool writeManifest) => 
            _serializer(value, ref writer, writeManifest);

        public override Expression EmitExpression(Expression value, Expression typedWriter, bool includeManifest)
        {
            var target = Expression.Constant(this);
            var method = GetType().GetMethod(nameof(Write));
            var call = Expression.Call(target, method, value, typedWriter, Expression.TrueConstant);
            return call;
        }
    }
}
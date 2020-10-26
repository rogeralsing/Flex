using System;
using System.Buffers;
using System.Linq;
using System.Text;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

#pragma warning disable 693

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class ObjectSerializer<TValue, TStyle, TBuffer> : ValueSerializer<TValue, TBuffer>
        where TBuffer : IBufferWriter<byte>
    {
        public const byte ManifestFull = 255;
        public const byte ManifestIndex = 254;
        private readonly ObjectSerializerDelegate<TBuffer, TStyle, TValue> _serializer;
        
        public ObjectSerializer(ObjectSerializerDelegate<TBuffer, TStyle, TValue> serializer)
        {
            _serializer = serializer;
        }

        public override void Write(TValue value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            _serializer(value, ref writer);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var target = Expression.Constant(this);
            var method = GetType().GetMethod(nameof(Write));
            var call = Expression.Call(target, method, value, typedWriter);
            return call;
        }
    }
}
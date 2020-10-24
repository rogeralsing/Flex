using System.Buffers;
using Flex.Buffers;
using JetBrains.Annotations;

#pragma warning disable 693

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class ObjectSerializer<TValue,TBuffer> : ValueSerializer<TValue,TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private readonly ObjectSerializerDelegate<TBuffer, TValue> _serializer;

        public ObjectSerializer(ObjectSerializerDelegate<TBuffer,TValue> serializer) => 
            _serializer = serializer;

        public override void WriteManifest(ref Writer<TBuffer> writer) =>
            ByteSerializer<TBuffer>.WriteStatic(255, ref writer);

        public override void Write(TValue value, ref Writer<TBuffer> writer) => 
            _serializer(value, ref writer);
    }
}
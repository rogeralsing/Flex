using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using Flex.Generics;
using Flex.SerializeReferences;
namespace Flex
{
    public sealed class Serializer
    {
        private readonly bool _preserveObjectReferences;

        public Serializer(SerializerOptions options)
        {
            Options = options;
            _preserveObjectReferences = options.PreserveObjectReferences;
        }

        public SerializerOptions Options { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SerializerSession CreateSession()
        {
            return new SerializerSession(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize<T>(T value, MemoryStream stream)
        {
            var writer = Writer.Create(stream, new SerializerSession(this));
            Serialize(value, writer, _preserveObjectReferences);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize<T>(T value, Stream stream)
        {
            var writer = Writer.Create(stream, new SerializerSession(this));
            Serialize(value, writer, _preserveObjectReferences);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize<T, TBuffer>(T value, ref Writer<TBuffer> writer) where TBuffer : IBufferWriter<byte>
        {
            Serialize(value, writer, _preserveObjectReferences);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeUntyped<TBuffer>(object value, ref Writer<TBuffer> writer) where TBuffer : IBufferWriter<byte>
        {
            if (_preserveObjectReferences)
                Serializers<TBuffer, Graph>.ForType(value.GetType()).WriteObject(value,ref writer,true);
            else 
                Serializers<TBuffer, Tree>.ForType(value.GetType()).WriteObject(value,ref writer,true);
            writer.Commit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Serialize<T, TBuffer>(T value, Writer<TBuffer> writer, bool preserveObjectReferences)
            where TBuffer : IBufferWriter<byte>
        {
            if (preserveObjectReferences)
                TypedSerializers<TBuffer, Graph, T>.Serialize(value, ref writer, true);
            else
                TypedSerializers<TBuffer, Tree, T>.Serialize(value, ref writer, true);

            writer.Commit();
        }
    }
}
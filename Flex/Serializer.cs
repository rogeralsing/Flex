using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using Flex.Generics;

namespace Flex
{
    public class Serializer
    {
        private readonly bool _preserveObjectReferences;

        public Serializer(SerializerOptions options)
        {
            Options = options;
            _preserveObjectReferences = options.PreserveObjectReferences;
        }

        public SerializerOptions Options { get; }

        public SerializerSession CreateSession()
        {
            return new SerializerSession(this);
        }

        public void Serialize<T>(T value, MemoryStream stream)
        {
            var writer = Writer.Create(stream, new SerializerSession(this));
            Serialize(value, writer, _preserveObjectReferences);
        }

        public void Serialize<T>(T value, Stream stream)
        {
            var writer = Writer.Create(stream, new SerializerSession(this));
            Serialize(value, writer, _preserveObjectReferences);
        }

        public void Serialize<T, TBuffer>(T value, ref Writer<TBuffer> writer) where TBuffer : IBufferWriter<byte>
        {
            Serialize(value, writer, _preserveObjectReferences);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Serialize<T, TBuffer>(T value, Writer<TBuffer> writer, bool preserveReferences)
            where TBuffer : IBufferWriter<byte>
        {
            if (preserveReferences)
                TypedSerializers<TBuffer, Graph, T>.SerializeWithManifest(value, ref writer);
            else
                TypedSerializers<TBuffer, Tree, T>.SerializeWithManifest(value, ref writer);

            writer.Commit();
        }
    }
}
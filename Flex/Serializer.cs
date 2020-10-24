using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using Flex.Generics;

namespace Flex
{
    public class Serializer
    {
        private readonly bool _preserveReferences;

        public Serializer(bool preserveReferences)
        {
            _preserveReferences = preserveReferences;
        }
        public void Serialize<T>(T value, Stream stream)
        {
            var writer = Writer.Create(stream, new SerializerSession());
            Serialize(value, writer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Serialize<T,TBuffer>(T value, Writer<TBuffer> writer) where TBuffer:IBufferWriter<byte>
        {
            if (_preserveReferences)
                Serialize<T, TBuffer, Graph>(value, ref writer);
            else
                Serialize<T, TBuffer, Tree>(value, ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Serialize<T,TBuffer,TStyle>(T value, ref Writer<TBuffer> writer) where TBuffer:IBufferWriter<byte>
        {
            var s = SerializerCache<TBuffer, TStyle>.GetOrBuild<T>();
            s.WriteManifest(ref writer);
            s.Write(value, ref writer);
            writer.Commit();
        }
    }
}
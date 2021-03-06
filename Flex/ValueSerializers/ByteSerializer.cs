using System.Buffers;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class ByteSerializer<TBuffer> : ValueSerializer<byte, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = sizeof(byte);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(byte value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }

        public override void Write(byte value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            if (writeManifest) writer.Write((byte) 1);
            writer.Write(value);
        }
    }
    
    public class ByteDeserializer : ValueDeserializer<byte>
    {
        public override byte Read(ref Reader reader) => reader.ReadByte();
    }
}
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
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            writer.Write((byte)1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(byte value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(byte value, ref Writer<TBuffer> writer)
        {
            writer.Write(value);
        }
    }
}
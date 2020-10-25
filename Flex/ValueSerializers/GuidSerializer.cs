using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class GuidSerializer<TBuffer> : ValueSerializer<Guid, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = 16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            ByteSerializer<TBuffer>.WriteStatic(3, ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(Guid value, ref Writer<TBuffer> writer)
        {
            writer.EnsureContiguous(Size);
            value.TryWriteBytes(writer.WritableSpan);
            writer.AdvanceSpan(Size);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(Guid value, ref Writer<TBuffer> writer)
        {
            WriteStatic(value, ref writer);
        }
    }
}
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class DateTimeSerializer<TBuffer> : ValueSerializer<DateTime, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = 9;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            ByteSerializer<TBuffer>.WriteStatic(4, ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(DateTime value, ref Writer<TBuffer> writer)
        {
            writer.Write(value.Ticks);
            writer.Write((byte) value.Kind);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(DateTime value, ref Writer<TBuffer> writer)
        {
            WriteStatic(value, ref writer);
        }
    }
}
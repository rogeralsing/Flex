using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class StringSerializer<TBuffer> : ValueSerializer<string, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteManifestStatic(ref Writer<TBuffer> writer)
        {
            ByteSerializer<TBuffer>.WriteStatic(5, ref writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteStatic(string value, ref Writer<TBuffer> writer)
        {
            var maxPotentialLength = value.Length * 2; //max two bytes per char
            writer.EnsureContiguous(maxPotentialLength + 4);

            //first write string bytes, 4 bytes into the span
            var actualLength = Encoding.UTF8.GetBytes(value, writer.WritableSpan[4..]);
            //then write the actual length at 0 bytes into the span
            BitConverter.TryWriteBytes(writer.WritableSpan, actualLength);
            writer.AdvanceSpan(actualLength + 4);
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            WriteManifestStatic(ref writer);
        }

        public override void Write(string value, ref Writer<TBuffer> writer)
        {
            WriteStatic(value, ref writer);
        }
    }
}
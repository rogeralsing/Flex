using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Flex.Buffers.Adaptors
{
    /// <summary>
    ///     An implementation of <see cref="IBufferWriter{T}" /> which writes to an array.
    /// </summary>
    public struct ArrayBufferWriter : IBufferWriter<byte>
    {
        internal ArrayBufferWriter(byte[] buffer)
        {
            Buffer = buffer;
            BytesWritten = 0;
        }

        public byte[] Buffer { get; }

        public Memory<byte> Memory => Buffer.AsMemory(0, BytesWritten);

        public int BytesWritten { get; private set; }

        public void Advance(int count)
        {
            if (BytesWritten > Buffer.Length)
            {
                ThrowInvalidCount();

                [MethodImpl(MethodImplOptions.NoInlining)]
                static void ThrowInvalidCount()
                {
                    throw new InvalidOperationException("Cannot advance past the end of the buffer");
                }
            }

            BytesWritten += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (BytesWritten + sizeHint > Buffer.Length) ThrowInsufficientCapacity(sizeHint);

            return Buffer.AsMemory(BytesWritten);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            if (BytesWritten + sizeHint > Buffer.Length) ThrowInsufficientCapacity(sizeHint);

            return Buffer.AsSpan(BytesWritten);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowInsufficientCapacity(int sizeHint)
        {
            throw new InvalidOperationException(
                $"Insufficient capacity to perform the requested operation. Buffer size is {Buffer.Length}. Current length is {BytesWritten} and requested size increase is {sizeHint}");
        }
    }
}
using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Flex.Buffers.Adaptors
{
    /// <summary>
    ///     A <see cref="IBufferWriter{T}" /> implementation for <see cref="Memory{T}" />
    /// </summary>
    public struct MemoryBufferWriter : IBufferWriter<byte>
    {
        private readonly Memory<byte> _buffer;

        internal MemoryBufferWriter(Memory<byte> buffer)
        {
            _buffer = buffer;
            BytesWritten = 0;
        }

        public int BytesWritten { get; private set; }

        /// <inheritdoc />
        public void Advance(int count)
        {
            if (BytesWritten > _buffer.Length)
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

        /// <inheritdoc />
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (BytesWritten + sizeHint > _buffer.Length) ThrowInsufficientCapacity(sizeHint);

            return _buffer.Slice(BytesWritten);
        }

        /// <inheritdoc />
        public Span<byte> GetSpan(int sizeHint = 0)
        {
            if (BytesWritten + sizeHint > _buffer.Length) ThrowInsufficientCapacity(sizeHint);

            return _buffer.Span.Slice(BytesWritten);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowInsufficientCapacity(int sizeHint)
        {
            throw new InvalidOperationException(
                $"Insufficient capacity to perform the requested operation. Buffer size is {_buffer.Length}. Current length is {BytesWritten} and requested size increase is {sizeHint}");
        }
    }
}
using System;
using System.Buffers;
using System.Diagnostics.Contracts;
using System.Text;

namespace Flex.Buffers.Adaptors
{
    public struct SingleSegmentBuffer : IBufferWriter<byte>
    {
        private readonly byte[] _buffer;

        public SingleSegmentBuffer(byte[] buffer)
        {
            _buffer = buffer;
            Length = 0;
        }

        public void Advance(int bytes)
        {
            Length += bytes;
        }

        [Pure]
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return _buffer.AsMemory(Length);
        }

        [Pure]
        public Span<byte> GetSpan(int sizeHint)
        {
            return _buffer.AsSpan(Length);
        }

        public byte[] ToArray()
        {
            return _buffer.AsSpan(0, Length).ToArray();
        }

        public void Reset()
        {
            Length = 0;
        }

        [Pure] public int Length { get; private set; }

        [Pure]
        public ReadOnlySequence<byte> GetReadOnlySequence()
        {
            return new ReadOnlySequence<byte>(_buffer, 0, Length);
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(_buffer.AsSpan(0, Length).ToArray());
        }
    }
}
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Flex.Buffers
{
    public ref struct Reader
    {
        private DeserializerSession _session;
        private Stream _stream;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
        {
            var buffer = _session.GetSpan(8);
            _stream.Read(buffer);
            return BitConverter.ToInt64(buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
        {
            var buffer = _session.GetSpan(4);
            _stream.Read(buffer);
            return BitConverter.ToInt32(buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
        {
            var buffer = _session.GetSpan(2);
            _stream.Read(buffer);
            return BitConverter.ToInt16(buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            var buffer = _session.GetSpan(1);
            _stream.Read(buffer);
            return default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
        {
            var buffer = _session.GetSpan(8);
            _stream.Read(buffer);
            return BitConverter.ToUInt64(buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
        {
            var buffer = _session.GetSpan(4);
            _stream.Read(buffer);
            return BitConverter.ToUInt32(buffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
        {
            var buffer = _session.GetSpan(2);
            _stream.Read(buffer);
            return BitConverter.ToUInt16(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            var buffer = _session.GetSpan(1);
            _stream.Read(buffer);
            return buffer[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid ReadGuid()
        {
            var buffer = _session.GetSpan(16);
            _stream.Read(buffer);
            return new Guid(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DateTime ReadDateTime()
        {
            var buffer = _session.GetSpan(9);
            _stream.Read(buffer);
            var ticks = BitConverter.ToInt64(buffer[..8]);
            var kind = (DateTimeKind) buffer[8];
            return new DateTime(ticks, kind);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString()
        {
            var buffer = _session.GetSpan(2);
            _stream.Read(buffer);
            var length = BitConverter.ToInt16(buffer);

            var stringBuffer = _session.GetSpan(length);
            _stream.Read(stringBuffer);
            var str = Encoding.UTF8.GetString(stringBuffer);
            return str;
        }
    }
}
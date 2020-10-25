using System;
using System.Buffers;
using System.Linq;
using System.Text;
using Flex.Buffers;
using JetBrains.Annotations;

#pragma warning disable 693

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class ObjectSerializer<TValue,TBuffer> : ValueSerializer<TValue,TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private readonly ObjectSerializerDelegate<TBuffer, TValue> _serializer;
        public const byte ManifestFull = 255;
        public const byte ManifestIndex = 254;
        private static readonly byte[] Manifest = GetManifest();
        private static readonly Type Type = typeof(TValue);

        private static byte[] GetManifest() => 
            Encoding
                .UTF8
                .GetBytes(typeof(TValue)!.FullName!)
                .Prepend(ManifestFull)
                .ToArray();

        public ObjectSerializer(ObjectSerializerDelegate<TBuffer,TValue> serializer) => 
            _serializer = serializer;
        
        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            if (writer.Session.ShouldWriteTypeManifest(Type, out var typeIndex))
            {
                writer.Write(ManifestFull);
            }
            else
            {
                writer.Write(ManifestIndex);
                writer.Write(typeIndex);
            }
        }

        public override void Write(TValue value, ref Writer<TBuffer> writer) => 
            _serializer(value, ref writer);
    }
}
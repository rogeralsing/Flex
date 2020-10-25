using System;
using System.Buffers;
using System.Linq;
using System.Text;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

#pragma warning disable 693

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class ObjectSerializer<TValue, TStyle, TBuffer> : ValueSerializer<TValue, TBuffer>
        where TBuffer : IBufferWriter<byte>
    {
        public const byte ManifestFull = 255;
        public const byte ManifestIndex = 254;
        private readonly byte[] Manifest = GetManifest();
        private readonly Type Type = typeof(TValue);
        private readonly ObjectSerializerDelegate<TBuffer, TStyle, TValue> _serializer;

        public ObjectSerializer(ObjectSerializerDelegate<TBuffer, TStyle, TValue> serializer)
        {
            _serializer = serializer;
        }

        private static byte[] GetManifest()
        {
            return Encoding
                .UTF8
                .GetBytes(typeof(TValue)!.FullName!)
                .Prepend(ManifestFull)
                .ToArray();
        }

        public override void WriteManifest(ref Writer<TBuffer> writer)
        {
            if (writer.Session.ShouldWriteTypeManifest(Type, out var knownTypeIndex))
            {
                writer.Write(ManifestFull);
            }
            else
            {
                writer.Write(ManifestIndex);
                writer.Write(knownTypeIndex);
            }
        }
        
        //not sure why this is slower, because _serializer was not static readonly?
        // public static void WriteStatic(TValue value, ref Writer<TBuffer> writer)
        // {
        //     _serializer(value, ref writer);
        // }

        public override void Write(TValue value, ref Writer<TBuffer> writer)
        {
            _serializer(value, ref writer);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter)
        {
            var target = Expression.Constant(this);
            var method = GetType().GetMethod("Write");
            var call = Expression.Call(target,method, value,typedWriter);
            return call;
        }
    }
}
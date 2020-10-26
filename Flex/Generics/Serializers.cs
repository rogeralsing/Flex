using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Flex.Compilation;
using Flex.ValueSerializers;
using JetBrains.Annotations;

namespace Flex.Generics
{
    [PublicAPI]
    public static class Serializers<TBuffer, TStyle> where TBuffer : IBufferWriter<byte>
    {
        private static readonly ConcurrentDictionary<Type, ValueSerializer<TBuffer>> Cache =
            GetDefaultSerializers();

        private static ConcurrentDictionary<Type, ValueSerializer<TBuffer>> GetDefaultSerializers()
        {
            return new ConcurrentDictionary<Type, ValueSerializer<TBuffer>>
            {
                [typeof(byte)] = new ByteSerializer<TBuffer>(),
                [typeof(int)] = new Int32Serializer<TBuffer>(),
                [typeof(string)] = new StringSerializer<TBuffer>(),
                [typeof(Guid)] = new GuidSerializer<TBuffer>(),
                [typeof(DateTime)] = new DateTimeSerializer<TBuffer>()
            };
        }

        public static ValueSerializer<TBuffer> ForType(Type type)
        {
            if (Cache.TryGetValue(type, out var s)) return s;

            var res = Compiler<TBuffer, TStyle>.CompileSerializer(type);
            Cache.TryAdd(type, res);
            return res;
        }
    }

    public static class TypedSerializers<TBuffer, TStyle, TValue> where TBuffer : IBufferWriter<byte>
    {
        public static readonly ObjectSerializerDelegate<TBuffer,TStyle, TValue> SerializeWithManifest =
            Compiler<TBuffer, TStyle>.CompileSerializer<TValue>(typeof(TValue), true);
        
        public static readonly ObjectSerializerDelegate<TBuffer,TStyle, TValue> SerializeNoManifest =
            Compiler<TBuffer, TStyle>.CompileSerializer<TValue>(typeof(TValue), false);
        //
        // public static readonly ObjectSerializer<TValue, TStyle, TBuffer> Serializer =
        //     new ObjectSerializer<TValue, TStyle, TBuffer>(SerializerDelegate);
    }
}
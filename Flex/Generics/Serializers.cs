using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Flex.Compilation;
using Flex.MessagePackStuff;
using Flex.ValueSerializers;
using JetBrains.Annotations;

namespace Flex.Generics
{
    [PublicAPI]
    public static class Serializers<TBuffer, TStyle> where TBuffer : IBufferWriter<byte>
    {
        private static readonly ThreadsafeTypeKeyHashTable<ValueSerializer<TBuffer>> Cache =
            GetDefaultSerializers();

        
        private static ThreadsafeTypeKeyHashTable<ValueSerializer<TBuffer>> GetDefaultSerializers()
        {
            var ht = new ThreadsafeTypeKeyHashTable<ValueSerializer<TBuffer>>();
            ht.TryAdd(typeof(byte), new ByteSerializer<TBuffer>());
            ht.TryAdd(typeof(int), new Int32Serializer<TBuffer>());
            ht.TryAdd(typeof(string), new StringSerializer<TBuffer>());
            ht.TryAdd(typeof(Guid), new GuidSerializer<TBuffer>());
            ht.TryAdd(typeof(DateTime), new DateTimeSerializer<TBuffer>());
            return ht;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueSerializer<TBuffer> ForType(Type type)
        {
            if (Cache.TryGetValue(type, out var s)) return s;

            var res = Compiler<TBuffer, TStyle>.CompileSerializer(type);
            Cache.TryAdd(type, res);
            return res;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueSerializer<TBuffer> ForValue(object value) => ForType(value.GetType());
    }

    public static class TypedSerializers<TBuffer, TStyle, TValue> where TBuffer : IBufferWriter<byte>
    {
        public static readonly ObjectSerializerDelegate<TBuffer, TStyle, TValue> Serialize =
            Compiler<TBuffer, TStyle>.CompileSerializer<TValue>(typeof(TValue));

    }
}
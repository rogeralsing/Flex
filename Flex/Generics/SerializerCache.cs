using System;
using System.Buffers;
using System.Collections.Concurrent;
using Flex.Compilation;
using Flex.ValueSerializers;
using JetBrains.Annotations;

namespace Flex.Generics
{
    [PublicAPI]
    public static class SerializerCache<TBuffer, TStyle> where TBuffer:IBufferWriter<byte>
    {
        private static readonly ConcurrentDictionary<Type, ValueSerializer<TBuffer>> Serializers =
            GetDefaultSerializers();

        private static ConcurrentDictionary<Type, ValueSerializer<TBuffer>> GetDefaultSerializers() =>
            new ConcurrentDictionary<Type, ValueSerializer<TBuffer>>
            {
                [typeof(byte)] = new ByteSerializer<TBuffer>(),
                [typeof(int)] = new Int32Serializer<TBuffer>(),
                [typeof(string)] = new StringSerializer<TBuffer>(),
                [typeof(Guid)] = new GuidSerializer<TBuffer>(),
                [typeof(DateTime)] = new DateTimeSerializer<TBuffer>()
            };

        public static ValueSerializer<TBuffer> GetOrBuild(Type type)
        {
            if (Serializers.TryGetValue(type, out var s))
            {
                return s;
            }

            var res = Compiler<TBuffer, TStyle>.CompileSerializer(type);
            Serializers.TryAdd(type, res);
            return res;
        }

        public static ValueSerializer<T,TBuffer> GetOrBuild<T>()
        {
            return Inner<T>.Serializer;
        }

        internal static class Inner<T>
        {
            internal static readonly ValueSerializer<T, TBuffer> Serializer = (ValueSerializer<T, TBuffer>)Compiler<TBuffer, TStyle>.CompileSerializer(typeof(T));
        }
    }
}
using System;
using System.Buffers;
using System.Collections.Concurrent;
using Flex.Compilation;
using Flex.ValueSerializers;
using JetBrains.Annotations;

namespace Flex.Generics
{
    [PublicAPI]
    public static class Serializers<TBuffer, TStyle> where TBuffer:IBufferWriter<byte>
    {
        private static readonly ConcurrentDictionary<Type, ValueSerializer<TBuffer>> Cache =
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
            if (Cache.TryGetValue(type, out var s)) return s;

            var res = Compiler<TBuffer, TStyle>.CompileSerializer(type);
            Cache.TryAdd(type, res);
            return res;
        }
    }
    
    public static class TypedSerializers<TBuffer, TStyle,TObj> where TBuffer:IBufferWriter<byte>
    {
        public static readonly ValueSerializer<TObj, TBuffer> Serializer = (ValueSerializer<TObj, TBuffer>)Compiler<TBuffer, TStyle>.CompileSerializer(typeof(TObj));
    }
}
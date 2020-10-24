using System;
using System.Buffers;
using System.Linq;
using Flex.Buffers;
using Flex.Generics;
using Flex.Reflection;
using Flex.ValueSerializers;
using JetBrains.Annotations;

namespace Flex.Compilation
{
    [PublicAPI]
    public static class Compiler<TBuffer, TStyle> where TBuffer:IBufferWriter<byte>
    {
        private static bool PreserveReferences => typeof(TStyle) == typeof(Graph);

        public static ValueSerializer<TBuffer> CompileSerializer(Type type)
        {
            var fields = type.GetFieldsForType();
            var fieldSerializers =
                fields
                    .Select(field => SerializerCache<TBuffer, TStyle>.GetOrBuild(field.FieldType))
                    .ToArray();

            foreach (var field in fields)
            {
                if (field.FieldType.IsSealed)
                {
                    
                }
                else
                {
                    
                }
            }

            var objectSerializer = new ObjectSerializer<string, TBuffer>((string value, ref Writer<TBuffer> writer) =>
            {
            });
            return null;
        }
    }
}
using System;
using System.Buffers;
using System.Linq;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using Flex.Generics;
using Flex.Reflection;
using Flex.ValueSerializers;
using JetBrains.Annotations;
#pragma warning disable 8321

namespace Flex.Compilation
{
    [PublicAPI]
    public static class Compiler<TBuffer, TStyle> where TBuffer:IBufferWriter<byte>
    {
        private static bool PreserveReferences => typeof(TStyle) == typeof(Graph);

        public static ValueSerializer<TBuffer> CompileSerializer(Type type) =>
            GenericCaller.RunGeneric<ValueSerializer<TBuffer>>(type,() =>
            {
                ValueSerializer<TBuffer> Create<TValue>() => CompileSerializer<TValue>();
            });

        private static ValueSerializer<TBuffer> CompileSerializer<TValue>()
        {
            var type = typeof(TValue);
            var writerType = typeof(Writer<TBuffer>).ReflectedType;
            
            var fields = type.GetFieldsForType();
            var fieldSerializers =
                fields
                    .Select(field => SerializerCache<TBuffer, TStyle>.GetOrBuild(field.FieldType))
                    .ToArray();

            var typedTarget = Expression.Parameter(type, "target");
            var typedWriter =  Expression.Parameter(writerType, "writer");
            var body = Expression.Block(Array.Empty<Expression>());
            
            
            foreach (var field in fields)
            {
                if (field.FieldType.IsSealed)
                {
                    
                }
                else
                {
                    
                }
            }

            var lambda = Expression.Lambda<ObjectSerializerDelegate<TBuffer, TValue>>(body, typedTarget, typedWriter);

            var del =lambda.CompileFast();

            var objectSerializer = new ObjectSerializer<TValue, TBuffer>(del);
            return objectSerializer;
        }
    }
}
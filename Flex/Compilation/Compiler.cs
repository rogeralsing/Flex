using System;
using System.Buffers;
using System.Collections.Generic;
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
                ValueSerializer<TBuffer> Create<TValue>() => CompileSerializer<TValue>(type);
            });

        private static ValueSerializer<TBuffer> CompileSerializer<TValue>(Type type)
        {
            var writerType = typeof(Writer<TBuffer>).MakeByRefType();
            
            var fields = type.GetFieldsForType();
            var fieldSerializers =
                fields
                    .Select(field => SerializerCache<TBuffer, TStyle>.GetOrBuild(field.FieldType))
                    .ToArray();

            var typedTarget = Expression.Parameter(type, "target");
            var typedWriter =  Expression.Parameter(writerType, "writer");
            
            

            var expressions = new List<Expression>();

            for (var index = 0; index < fields.Length; index++)
            {
                var field = fields[index];
                var serializer = fieldSerializers[index];
                
                var memberAccess = Expression.MakeMemberAccess(typedTarget, field);
                if (field.FieldType.IsSealed)
                {
                    var writeField = serializer.EmitExpression(memberAccess, typedWriter);
                    expressions.Add(writeField);
                }
                else
                {
                }
            }

            Expression body = expressions.Any() ? Expression.Block(expressions) : Expression.Empty();
            var lambda = Expression.Lambda<ObjectSerializerDelegate<TBuffer, TValue>>(body, typedTarget, typedWriter);

            var del =lambda.CompileFast();
            var cs = lambda.ToCSharpString();
            Console.WriteLine(cs);

            var objectSerializer = new ObjectSerializer<TValue, TBuffer>(del);
            return objectSerializer;
        }
    }
}
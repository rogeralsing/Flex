using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class Compiler<TBuffer, TStyle> where TBuffer : IBufferWriter<byte>
    {
        private static bool PreserveReferences => typeof(TStyle) == typeof(Graph);

        public static ValueSerializer<TBuffer> CompileSerializer(Type type)
        {
            return GenericCaller.RunGeneric<ValueSerializer<TBuffer>>(type, () =>
            {
                ValueSerializer<TBuffer> Create<TValue>()
                {
                    var del = CompileSerializer<TValue>(type, false);
                    var objectSerializer = new ObjectSerializer<TValue, TStyle, TBuffer>(del);
                    return objectSerializer;
                }
            });
        }

        public static ObjectSerializerDelegate<TBuffer, TStyle, TValue> CompileSerializer<TValue>(Type type,
            bool includeManifest)
        {
            var writerType = typeof(Writer<TBuffer>).MakeByRefType();

            var fields = type.GetFieldsForType();
            var fieldSerializers =
                fields
                    .Select(field => Serializers<TBuffer, TStyle>.ForType(field.FieldType))
                    .ToArray();

            var typedTarget = Expression.Parameter(type, "target");
            var typedWriter = Expression.Parameter(writerType, "writer");


            var expressions = new List<Expression>();


            if (includeManifest)
            {
                var manifest = Encoding
                    .UTF8
                    .GetBytes(type.FullName!)
                    .Prepend((byte) 255)
                    .ToArray();
                var typeExpression = Expression.Constant(type);
                var manifestExpression = Expression.Constant(manifest);

                var method = typeof(Writer<TBuffer>).GetMethod(nameof(Writer<TBuffer>.WriteManifest));
                var writeManifestCall = Expression.Call(typedWriter, method, typeExpression, manifestExpression);
                expressions.Add(writeManifestCall);
            }

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
            }

            Expression body = expressions.Any() ? Expression.Block(expressions) : Expression.Empty();
            var lambda =
                Expression.Lambda<ObjectSerializerDelegate<TBuffer, TStyle, TValue>>(body, typedTarget, typedWriter);

            var del = lambda.CompileFast();
            var cs = lambda.ToCSharpString();
            Console.WriteLine(cs);

            return del;
        }
    }
}
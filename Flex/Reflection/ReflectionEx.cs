using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flex.Reflection
{
    public static class ReflectionEx
    {
        public static readonly Assembly CoreAssembly = typeof(int).Assembly;

        public static FieldInfo[] GetFieldsForType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var fieldInfos = new List<FieldInfo>();
            var current = type;
            while (current != null)
            {
                var typeFields =
                    current
                        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                        .Where(f => !f.IsDefined(typeof(NonSerializedAttribute)))
                        .Where(f => !f.IsStatic)
                        .Where(f => f.FieldType != typeof(IntPtr))
                        .Where(f => f.FieldType != typeof(UIntPtr))
                        .Where(f => f.Name != "_syncRoot"); //HACK: ignore these 

                fieldInfos.AddRange(typeFields);
                current = current.BaseType;
            }

            var fields = fieldInfos.OrderBy(f => f.Name).ToArray();
            return fields;
        }
    }
}
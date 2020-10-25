using System;
using System.Collections.Generic;
using System.Linq;

namespace Flex
{
    public class SerializerOptions
    {
        internal readonly Type[] KnownTypes;
        internal readonly Dictionary<Type, ushort> KnownTypesDict = new Dictionary<Type, ushort>();

        internal readonly bool PreserveObjectReferences;

        public SerializerOptions(bool preserveObjectReferences = false,
            IEnumerable<Type>? knownTypes = null)
        {
            KnownTypes = knownTypes?.ToArray() ?? new Type[] { };
            for (var i = 0; i < KnownTypes.Length; i++) KnownTypesDict.Add(KnownTypes[i], (ushort) i);

            PreserveObjectReferences = preserveObjectReferences;
        }
    }
}
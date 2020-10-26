using System.IO;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public abstract class ValueDeserializer<TValue>
    {
        public abstract TValue Read(ref Reader reader);
    }
}
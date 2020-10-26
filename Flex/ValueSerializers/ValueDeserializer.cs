using System.IO;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public abstract class ValueDeserializer<TValue>
    {
        public abstract TValue Read(ref Reader reader);
    }
}
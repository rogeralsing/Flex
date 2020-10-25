using System.Buffers;
using Flex.Buffers;
using JetBrains.Annotations;
// ReSharper disable UnusedTypeParameter

namespace Flex.ValueSerializers
{
    
    [PublicAPI]
    // ReSharper disable once TypeParameterCanBeVariant
    public delegate void ObjectSerializerDelegate<TBuffer, TStyle, TValue>(TValue value, ref Writer<TBuffer> writer)
        where TBuffer : IBufferWriter<byte>;
    
}
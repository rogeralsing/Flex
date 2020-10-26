using System;
using System.Buffers;
using FastExpressionCompiler.LightExpression;
using Flex.Buffers;
using JetBrains.Annotations;

namespace Flex.ValueSerializers
{
    [PublicAPI]
    public class DateTimeSerializer<TBuffer> : ValueSerializer<DateTime, TBuffer> where TBuffer : IBufferWriter<byte>
    {
        private const int Size = 9;

        public override void Write(DateTime value, ref Writer<TBuffer> writer, bool writeManifest)
        {
            if (writeManifest) writer.Write((byte) 4);
            writer.Write(value);
        }

        public override Expression EmitExpression(Expression value, Expression typedWriter, bool includeManifest)
        {
            var method = typeof(Writer<TBuffer>).GetMethod("Write", new[] {typeof(DateTime)});
            var call = Expression.Call(typedWriter, method, value);
            return call;
        }
    }
    
    public class DateTimeDeserializer : ValueDeserializer<DateTime>
    {
        public override DateTime Read(ref Reader reader) => reader.ReadDateTime();
    }
}
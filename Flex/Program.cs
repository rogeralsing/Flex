using System;
using System.IO;

namespace Flex
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Serializer(true);
            var stream = new MemoryStream();
            s.Serialize(Guid.NewGuid(), stream);
        }
    }
}
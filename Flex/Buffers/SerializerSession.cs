using System;
using System.Collections.Generic;

namespace Flex.Buffers
{
    public class SerializerSession
    {
        private readonly ushort _nextTypeId;
        private readonly Dictionary<object, int> _objects = null!;
        private readonly List<Type> _trackedTypes = new List<Type>(100);
        private readonly Serializer Serializer;

        private int _nextObjectId;

        public SerializerSession(Serializer serializer)
        {
            Serializer = serializer;
            if (serializer.Options.PreserveObjectReferences) _objects = new Dictionary<object, int>();

            foreach (var type in serializer.Options.KnownTypes) TrackSerializedType(type);
        }

        public void TrackSerializedObject(object obj)
        {
            try
            {
                _objects.Add(obj, _nextObjectId++);
            }
            catch (Exception x)
            {
                throw new Exception("Error tracking object ", x);
            }
        }

        public bool TryGetObjectId(object obj, out int objectId)
        {
            return _objects.TryGetValue(obj, out objectId);
        }


        public bool ShouldWriteManifestIndex(Type key, out uint value)
        {
            for (var i = 0; i < _trackedTypes.Count; i++)
            {
                if (key != _trackedTypes[i]) continue;
                
                //shift one byte to the left, add object index serializer manifest
                value = ((uint) i << 8) | 254;
                return true;
            }

            value = 0;
            return false;
        }

        public void TrackSerializedType(Type type)
        {
            _trackedTypes.Add(type);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BulletHell.Collections
{
    [Serializable]
    public class DefaultValueDictionary<S,T> : Dictionary<S,T>
    {
        T defVal;

        public DefaultValueDictionary(T def = default(T))
        {
            defVal = def;
        }
        protected DefaultValueDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public new T this[S s]
        {
            get
            {
                if (ContainsKey(s))
                    return base[s];
                return defVal;
            }
            set
            {
                base[s] = value;
            }
        }
    }
}

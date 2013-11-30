using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Collections
{
    public class DefaultValueDictionary<S,T> : Dictionary<S,T>
    {
        T defVal;

        public DefaultValueDictionary(T def = default(T))
        {
            defVal = def;
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

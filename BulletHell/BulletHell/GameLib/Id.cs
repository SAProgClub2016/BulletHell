using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib
{
    public struct Id
    {
        int id;

        static int curid=0;
        static Dictionary<int, string> back = new Dictionary<int,string>();
        static Dictionary<string, int> forw = new Dictionary<string,int>();

        public Id(string s)
        {
            if (forw.ContainsKey(s))
            {
                id = forw[s];
            }
            else
            {
                id = curid++;
                forw[s] = id;
                back[id] = s;
            }
        }
        public Id(int id)
        {
            if(back.ContainsKey(id))
            {
                this.id = id;
            }
            else
            {
                this.id = id;
                forw[id.ToString()] = id;
                back[id] = id.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("<Id: id={0}, name={1}>",id,back[id]);
        }

        public override bool Equals(object obj)
        {
            Id? pc = obj as Id?;
            if (pc == null)
                return false;
            Id o = pc.Value;
            return o.id == id;
        }

        /*public static bool operator ==(Id o1, object o2)
        {
            return o1.Equals(o2);
        }
        public static bool operator ==(object o2, Id o1)
        {
            return o1.Equals(o2);
        }

        public static bool operator !=(Id o1, object o2)
        {
            return !o1.Equals(o2);
        }
        public static bool operator !=(object o2, Id o1)
        {
            return !o1.Equals(o2);
        }*/

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
        public static implicit operator Id(string s)
        {
            return new Id(s);
        }
        public static implicit operator Id(int id)
        {
            return new Id(id);
        }
    }
}

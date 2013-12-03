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
        static Dictionary<int, string> back;
        static Dictionary<string, int> forw;

        static Id()
        {
            back = new Dictionary<int, string>();
            forw = new Dictionary<string, int>();
        }

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

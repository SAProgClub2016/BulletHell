using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Physics
{
    public struct PhysicsClass
    {
        int id;

        static int curid=0;
        static Dictionary<int, string> back;
        static Dictionary<string, int> forw;

        static PhysicsClass()
        {
            back = new Dictionary<int, string>();
            forw = new Dictionary<string, int>();
        }

        public PhysicsClass(string s)
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
        public PhysicsClass(int id)
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
            return string.Format("<PhysicsClass: id={0}, name={1}>",id,back[id]);
        }

        public override bool Equals(object obj)
        {
            PhysicsClass? pc = obj as PhysicsClass?;
            if (pc == null)
                return false;
            PhysicsClass o = pc.Value;
            return o.id == id;
        }
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}

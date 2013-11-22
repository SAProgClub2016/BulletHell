using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib
{
    class GenRect<T>
    {
        public int Dimension { get; private set; }
        Vector<T> first;
        Vector<T> last;
        public GenRect(Vector<T> pos, Vector<T> oppPos)
        {
            Dimension = Math.Max(pos.Dimension, oppPos.Dimension);
            pos=pos.MakeDim(Dimension);
            oppPos = oppPos.MakeDim(Dimension);
            first = new Vector<T>(Dimension);
            last = new Vector<T>(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                if ((dynamic)pos[i] < oppPos[i])
                {
                    first[i] = pos[i];
                    last[i] = oppPos[i];
                }
                else
                {
                    first[i] = oppPos[i];
                    last[i] = pos[i];
                }
            }
        }
        public bool Contains(Vector<T> v)
        {
            if (v.Dimension != Dimension)
                return false;
            for (int i = 0; i < Dimension; i++)
            {
                if ((dynamic)v[i] < first[i] || (dynamic)v[i] > last[i])
                    return false;
            }
            return true;
        }
        public Vector<T> MakeInside(Vector<T> v)
        {
            Vector<T> ans = new Vector<T>(Dimension);
            v=v.MakeDim(Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                if ((dynamic)v[i] < first[i])
                    ans[i] = first[i];
                else if ((dynamic)v[i] > last[i])
                    ans[i] = last[i];
                else
                    ans[i] = v[i];
            }
            return ans;
        }
    }
}

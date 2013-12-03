using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib
{
    public class GenRect<T>
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
        public override string ToString()
        {
            return string.Format("Rectangle: {0} - {1}", first, last);
        }
    }
    public class GenRectTest
    {
        public static void Main()
        {
            GenRect<double> rect = new GenRect<double>(new Vector<double>(1, 2), new Vector<double>(6, 5));
            Console.WriteLine(rect);
            long thous = 1000;
            long million = thous * thous;
            long billion = thous * million;
            long trillion = million * million;
            Random r = new Random();
            int range = 10;
            long total = million;
            int count=0;
            for (long i = 0; i < total; i++)
            {
                double rand = range*r.NextDouble();
                double rand2 = range*r.NextDouble();
                Vector<double> ri = new Vector<double>(rand, rand2);
                //Console.WriteLine("Rectangle {0} {1}", rect.Contains(ri) ? "contains" : "does not contain", ri);
                
                if (rect.Contains(ri))
                {
                    count++;
                    //Console.WriteLine(ri);
                }
            }
            Console.WriteLine(rect);
            Console.WriteLine((double)count / total * range * range);
            Console.ReadKey();
        }
    }
}

using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.CoordLib
{
    class ManagedCoords : Coords
    {
        Vector<int> aspect;
        Vector<double> inS, outS;

        public ManagedCoords(params int[] rat) : base(rat.Length, 1)
        {
            aspect = new Vector<int>(rat);
            inS = Vector<double>.Fill(rat.Length,Nil.N,x=>1);
            outS = Vector<double>.Fill(rat.Length,Nil.N,x=>1);
        }
        private void Recompute()
        {
            for (int i = 0; i < Dimension; i++)
                this[i] = this[false, i] / this[true, i];
        }
        public double this[bool @in, int index]
        {
            get
            {
                if (@in)
                    return inS[index];
                else
                    return outS[index];
            }
            set
            {
                if(@in)
                {
                    double rat = value / aspect[index];
                    aspect.Map(x=>rat * x, inS);
                }
                else
                {
                    double rat = value / aspect[index];
                    aspect.Map(x => rat * x, outS);
                }
                Recompute();
            }
        }
    }
}

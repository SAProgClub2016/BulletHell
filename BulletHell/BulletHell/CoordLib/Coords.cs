using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.CoordLib
{
    public class Coords
    {
        int dim;
        Vector<double> scaling;
        Vector<double> counterScale;

        public int Dimension
        {
            get
            {
                return dim;
            }
        }

        public Coords(double x, double y, params double[] scale)
        {
            double[] ans = new double[scale.Length+2];
            ans[0]=x;
            ans[1]=y;
            for(int i=2;i<ans.Length;i++)
            {
                ans[i]=scale[i-2];
            }
            scaling = new Vector<double>(scale);
            dim = ans.Length;
            counterScale=scaling.Map(t=>1/t);
        }
        public Coords(int dim, double scale)
        {
            this.dim = dim;
            scaling = new Vector<double>(dim);
            foreach (int i in Enumerable.Range(0, dim))
                scaling[i] = scale;
            counterScale=scaling.Map(t=>1/t);
        }

        public Coords(Vector<double> scale)
        {
            this.scaling = scale;
            this.dim = scaling.Dimension;
            this.counterScale = scaling.Map(t => 1 / t);
        }

        public Vector<double> Transform(Vector<double> vec)
        {
            return vec.MultiplyE(scaling);
        }
        public Vector<double> Untransform(Vector<double> vec)
        {
            return vec.MultiplyE(counterScale);
        }

        public Coords Inverse
        {
            get
            {
                return new Coords(counterScale);
            }
        }

        public double this[int i]
        {
            get
            {
                return scaling[i];
            }
            set
            {
                scaling[i] = value;
                counterScale[i] = 1 / value;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

// Screw this, it's too complicated. We'll figure it out later. Need a basic physics class then.
namespace BulletHell.PhysicsLibOld
{
    public delegate T TimeFunc<T>(double time);

    public interface CalculusTimeFunction
    {
        TimeFunc<double> f
        {
            get;
        }

        CalculusTimeFunction df
        {
            get;
        }

        CalculusTimeFunction intf
        {
            get;
        }
    }

    public interface VectorTimeFunction
    {
        TimeFunc<Vector<double>> f
        {
            get;
        }
        // returns derivative of f
        VectorTimeFunction df
        {
            get;
        }
        // returns def integral of f from 0 to t (if easily computable)
        VectorTimeFunction intf
        {
            get;
        }
    }

    public class PolyVFunc : VectorTimeFunction
    {
        public Vector<double>[] Coefficients { get; private set; }
        public int Dimension { get; private set; }
        public int Degree
        {
            get
            {
                return Coefficients.Length;
            }
        }
        public PolyVFunc(int dim,params Vector<double>[] coeffs)
        {
            this.Coefficients = new Vector<double>[coeffs.Length];
            Dimension = dim;
            for (int i = 0; i < Coefficients.Length; i++)
            {
                if (coeffs[i].Dimension != dim)
                {
                    Coefficients[i] = coeffs[i].MakeDim(dim);
                }
                else
                {
                    Coefficients[i] = coeffs[i];
                }
            }
        }
        public TimeFunc<Vector<double>> f
        {
            get
            {
                return t =>
                {
                    Vector<double> sum = new Vector<double>(Dimension);
                    for (int i = Coefficients.Length - 1; i >= 0; i--)
                    {
                        sum *= t;
                        sum += Coefficients[i];
                    }
                    return sum;
                };
            }
        }
        public VectorTimeFunction df
        {
            get
            {
                if(Coefficients.Length<1)
                    return new PolyVFunc(Dimension,new Vector<double>(Dimension));
                Vector<double>[] dCoeffs = new Vector<double>[Coefficients.Length-1];
                for(int i = 0;i<dCoeffs.Length;i++)
                {
                    dCoeffs[i]=Coefficients[i+1]*(i+1);
                }
                return new PolyVFunc(Dimension, dCoeffs);
            }
        }
        public VectorTimeFunction intf
        {
            get
            {
                Vector<double>[] iCoeffs = new Vector<double>[Coefficients.Length + 1];
                iCoeffs[0] = new Vector<double>(Dimension);
                for (int i = 1; i < iCoeffs.Length; i++)
                {
                    iCoeffs[i] = Coefficients[i - 1] / i;
                }
                return new PolyVFunc(Dimension, iCoeffs);
            }
        }

        public static implicit operator PolyVFunc(Vector<double> v)
        {
            return new PolyVFunc(v.Dimension,v);
        }
        public static PolyVFunc operator +(PolyVFunc f1, PolyVFunc f2)
        {
            int newDim = Math.Max(f1.Dimension, f2.Dimension);
            int d = Math.Max(f1.Degree,f2.Degree);
            Vector<double>[] cs = new Vector<double>[d];
            int i = 0;
            for (; i < f1.Degree && i < f2.Degree; i++)
            {
                cs[i] = f1.Coefficients[i].MakeDim(newDim) + f2.Coefficients[i].MakeDim(newDim);
            }
            for (; i < f1.Degree; i++) cs[i] = f1.Coefficients[i];
            for (; i < f2.Degree; i++) cs[i] = f2.Coefficients[i];
            return new PolyVFunc(newDim, cs);
        }
        public static PolyVFunc operator +(PolyVFunc f1, Vector<double> v)
        {
            return f1 + (PolyVFunc)v;
        }
        public static PolyVFunc operator +(Vector<double> v, PolyVFunc f1)
        {
            return f1 + (PolyVFunc)v;
        }
        public Vector<double> this[int i]
        {
            get
            {
                return Coefficients[i];
            }
            set
            {
                Coefficients[i] = value.MakeDim(Dimension);
            }
        }
    }

    /*
    class Particle
    {
        private PolyVFunc pos, vel, acc;
        
        public Particle(PolyVFunc p)
        {
            Pos = p;
            
        }/*
        public Particle(PolyVFunc v, Vector x0)
        {
            Pos = v.intf + x0;
        }
        public Particle(PolyVFunc a, Vector v0, Vector x0)
        {
            Pos=(a.intf + v0).intf+x0;
        }
        public Particle(Vector a, Vector v0, Vector x0)
        {
            int dim = Math.Max(Math.Max(a.Dimension, v0.Dimension), x0.Dimension);
            Pos = new PolyVFunc(dim, x0, v0, a / 2);
        } */ /*

        public PolyVFunc Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                vel = pos.df;
                acc = vel.df;
            }
        }

        public PolyVFunc Vel
        {
            get
            {
                return vel;
            }
            set
            {
                vel = value;
                pos = vel.intf + pos.f(0);
                acc = vel.df;
            }
        }
        public PolyVFunc Acc
        {
            get
            {
                return acc;
            }
            set
            {
                acc = value;
                vel = acc.intf + vel.f(0);
                pos = vel.intf + pos.f(0);
            }
        }
    }*/
}

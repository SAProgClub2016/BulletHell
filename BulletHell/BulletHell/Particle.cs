using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

namespace BulletHell.PhysicsLib
{
    public delegate T TimeFunc<T>(double time);
    
    public interface VectorTimeFunction
    {
        TimeFunc<Vector> f
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
        public Vector[] Coefficients { get; private set; }
        public int Dimension { get; private set; }
        public PolyVFunc(int dim,params Vector[] coeffs)
        {
            this.Coefficients = new Vector[coeffs.Length];
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
        public TimeFunc<Vector> f
        {
            get
            {
                return t =>
                {
                    Vector sum = new Vector(Dimension);
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
                Vector[] dCoeffs = new Vector[Coefficients.Length-1];
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
                Vector[] iCoeffs = new Vector[Coefficients.Length + 1];
                iCoeffs[0] = new Vector(Dimension);
                for (int i = 1; i < iCoeffs.Length; i++)
                {
                    iCoeffs[i] = Coefficients[i - 1] / i;
                }
                return new PolyVFunc(Dimension, iCoeffs);
            }
        }
    }


    struct Particle
    {
        PolyVFunc pos;
        
    }
}

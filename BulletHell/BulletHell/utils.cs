using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell
{
    public class Utils
    {
        public static Func<S, T> MakeClosure<Q, S, T>(Q q, Func<Q, S, T> f)
        {
            return (S s) => f(q,s);
        }
        
        public static readonly int NUMTRIG = 2048;
        public static double[] Cosines,Sines;
        public static readonly double TWOPI = 2*Math.PI;
        public static readonly double CONVERSION = NUMTRIG/TWOPI;
        static Utils()
        {
            Cosines = new double[NUMTRIG];
            Sines = new double[NUMTRIG];
            for (int i = 0; i < NUMTRIG; i++)
            {
                double d = i/CONVERSION;
                Cosines[i] = Math.Cos(d);
                Sines[i] = Math.Sin(d);
            }
        }
        public static double FastCos(double t)
        {
            int i = (int)(t * CONVERSION);
            i %= NUMTRIG;
            if (i < 0) i += NUMTRIG;
            return Cosines[i];
        }
        public static double FastSin(double t)
        {
            int i = (int)(t * CONVERSION);
            i %= NUMTRIG;
            if (i < 0) i += NUMTRIG;
            return Sines[i];
        }
    }
}

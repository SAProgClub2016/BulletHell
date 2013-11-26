using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulletHell
{
    public delegate void Process();
    public class Utils
    {
        public static Func<S, T> MakeClosure<Q, S, T>(Q q, Func<Q, S, T> f)
        {
            return (S s) => f(q,s);
        }
        
        public static long TimeProcess(Process p)
        {
            BulletHell.Time.Timer timer = new BulletHell.Time.Timer();
            timer.Reset();
            p();
            return timer.Time;
        }

        public static void PrintTimeProcess(Process p, string name)
        {
            Console.WriteLine("{0}: {1}",name,TimeProcess(p));
        }

        public static readonly int NUMTRIG = 2048;
        public static double[] Cosines,Sines;
        public static readonly double TWOPI = 2*Math.PI;
        public static readonly double CONVERSION = NUMTRIG/TWOPI;
        public static readonly double TOLERANCE = 0.000001;

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
        public static bool IsZero(int i)
        {
            return i == 0;
        }
        public static bool IsZero(short i)
        {
            return i == 0;
        }
        public static bool IsZero(byte i)
        {
            return i == 0;
        }
        public static bool IsZero(long i)
        {
            return i == 0;
        }
        public static bool IsZero(float f)
        {
            return Math.Abs(f) < TOLERANCE;
        }
        public static bool IsZero(double d)
        {
            return Math.Abs(d) < TOLERANCE;
        }

        public static bool IsZero(object o)
        {
            return false;
        }
        public static bool IsZero(char c)
        {
            return false;
        }
        public static T Identity<T>(T t)
        {
            return t;
        }

    }
    public class Reference<T>
    {
        public T Value;
        public Reference(T t)
        {
            Value = t;
        }
        public static implicit operator Reference<T>(T t)
        {
            return new Reference<T>(t);
        }
        public static implicit operator T(Reference<T> t)
        {
            return t.Value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    public static class Operations<T>
    {
        public static readonly Func<T, T, T> AddT, MulT, SubT, DivT;
        public static readonly Func<T,T> NegT;


        static Operations()
        {
            try
            {
                var first = Expression.Parameter(typeof(T), "x");
                var second = Expression.Parameter(typeof(T), "y");
                var body = Expression.Add(first, second);
                var bodym = Expression.Multiply(first, second);
                var bodys = Expression.Subtract(first, second);
                var bodyd = Expression.Divide(first, second);
                var bodyn = Expression.Negate(first);
                AddT = Expression.Lambda<Func<T, T, T>>(body, first, second).Compile();
                MulT = Expression.Lambda<Func<T, T, T>>(bodym, first, second).Compile();
                SubT = Expression.Lambda<Func<T, T, T>>(bodys, first, second).Compile();
                DivT = Expression.Lambda<Func<T, T, T>>(bodyd, first, second).Compile();
                NegT = Expression.Lambda<Func<T, T>>(bodyn, first).Compile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error creating operators for: {0}", typeof(T));
            }
        }
    }
}

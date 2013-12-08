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

        public const int NUMTRIG = 2048;
        public static double[] Cosines,Sines;
        public const double TWOPI = 2*Math.PI;
        public const double CONVERSION = NUMTRIG/TWOPI;
        public const double TOLERANCE = 0.000001;

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
        public static readonly Func<T, T, T> Add, Mul, Sub, Div;
        public static readonly Func<T, T> Neg;


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
                Add = Expression.Lambda<Func<T, T, T>>(body, first, second).Compile();
                Mul = Expression.Lambda<Func<T, T, T>>(bodym, first, second).Compile();
                Sub = Expression.Lambda<Func<T, T, T>>(bodys, first, second).Compile();
                Div = Expression.Lambda<Func<T, T, T>>(bodyd, first, second).Compile();
                Neg = Expression.Lambda<Func<T, T>>(bodyn, first).Compile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error creating operators for: {0}", typeof(T));
            }
        }
    }
    public static class VectorScalar<V,S>
    {
        public static readonly Func<S, V, V> ScalarLeft;
        public static readonly Func<V,S,V> ScalarRight;
        public static readonly Func<V,V,S> Dot;

        static VectorScalar()
        {
            try
            {
                var vector = Expression.Parameter(typeof(V), "v");
                var vector2 = Expression.Parameter(typeof(V), "v2");
                var scalar = Expression.Parameter(typeof(S), "s");
                var bodysv = Expression.Multiply(scalar, vector);
                var bodyvs = Expression.Multiply(vector, scalar);
                var bodydot = Expression.Multiply(vector, vector2);
                ScalarLeft = Expression.Lambda<Func<S, V, V>>(bodysv, scalar, vector).Compile();
                ScalarRight = Expression.Lambda<Func<V, S, V>>(bodyvs, vector, scalar).Compile();
                Dot = Expression.Lambda<Func<V, V, S>>(bodydot, vector, vector2).Compile();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Error creating vector-scalar ops for: {0}, {1}\r\nContinuing", typeof(V),typeof(S));
            }
        }
    }
    public static class ArrayMapExtensions
    {
        public static S[] Map<T,S>(this T[] ts,Func<T,S> f)
        {
            S[] ans = new S[ts.Length];
            for (int i = 0; i < ts.Length; i++)
            {
                ans[i] = f(ts[i]);
            }
            return ans;
        }
    }
}

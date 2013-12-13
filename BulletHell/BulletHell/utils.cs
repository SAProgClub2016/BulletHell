using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace BulletHell
{
    public interface Value<T>
    {
        T Value { get; set; }
    }

    public class PropertyValue<S,T> : Value<T>
    {
        PropertyInfo prop;
        S obj;
        public PropertyValue(S s, string name)
        {
            obj = s;
            prop = typeof(S).GetProperty(name, typeof(T));
        }
        public T Value
        {
            get
            {
                return (T)prop.GetValue(obj, null);
            }
            set
            {
                prop.SetValue(obj, value, null);
            }
        }
    }
    public class LambdaValue<T> : Value<T>
    {
        public delegate S Getter<S>();
        public delegate void Setter<S>(S t);

        Getter<T> g; Setter<T> s;

        public LambdaValue(Getter<T> get, Setter<T> set)
        {
            g = get; s = set;
        }
        public LambdaValue(Func<Nil,T> get, Setter<T> set)
            : this(()=>get(Nil.N),set)
        {
        }
        public LambdaValue(Getter<T> get, Func<T, Nil> set)
            : this(get, (t) => { set(t); return; })
        {
        }
        public LambdaValue(Func<Nil,T> get, Func<T,Nil> set)
            : this(() => get(Nil.N), (t) => { set(t); return; })
        {
        }

        public T Value
        {
            get
            {
                return g();
            }
            set
            {
                s(value);
            }
        }
    }

    public struct Nil
    {
        public static readonly Nil N = new Nil();
    }

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
        public static Pair<double> RandomPairInBox(this Random r, Pair<double> p1, Pair<double> sizes)
        {
            return new Pair<double>(sizes.x*r.NextDouble()+p1.x,sizes.y*r.NextDouble()+p1.y);
        }
        public static Pair<double> RandomPairOnSegment(this Random r, Pair<double> p1, Pair<double> p2)
        {
            double t = r.NextDouble();
            double s = 1-t;
            return new Pair<double>(t*p1.x+s*p2.x,t*p1.y+s*p2.y);
        }
        public static Pair<double> RandomPairInCircle(this Random r, Pair<double> p1, double ra)
        {
            double th = Utils.TWOPI*r.NextDouble();
            double rad = ra*Math.Sqrt(r.NextDouble());

            return new Pair<double>(p1.x+rad*Utils.FastCos(th),p1.y+rad*Utils.FastSin(th));
        }
        public static Vector<double> RandomVectorInBox(this Random r, Vector<double> v1, Vector<double> sizes)
        {
            return v1 + sizes.Map<double>(x => r.NextDouble() * x);
        }
        public static Vector<double> RandomUnitVector(this Random r, int dimension)
        {
            return r.RandomVectorOnNSphere(dimension, 1);
        }
        public static Vector<double> RandomVectorOnNSphere(this Random r, int dimension, double rad)
        {
            Vector<double> randNorm = Vector<double>.Fill(dimension, r, t => t.NextNormal());
            double d = rad / randNorm.Length;
            return d * randNorm;
        }
        public static Vector<double> RandomVectorInNSphere(this Random r, int dimension, double rad)
        {
            double d = dimension;
            
            double rrad = rad * Math.Pow(r.NextDouble(),1/d);
            return r.RandomVectorOnNSphere(dimension,rrad);
        }
        public static Vector<double> RandomVectorInNSphere(this Random r, int dim, double rmin, double rmax)
        {
            double rminn = Math.Pow(rmin, dim);
            double rmaxn = Math.Pow(rmax, dim);
            double rad = Math.Pow((rmaxn-rminn)*r.NextDouble()+rminn,1/dim);
            return r.RandomVectorOnNSphere(dim,rad);
        }
        public static double NextNormal(this Random r, double mu = 0, double sigma = 1)
        {
            return Math.Sqrt(-2 * Math.Log(r.NextDouble())) * Utils.FastCos(Utils.TWOPI * r.NextDouble());
        }
    }
}

#define USING_EXPRESSIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib
{

    public struct Pair<T>
    {
        public T x, y;
        public Pair(T x1, T y1)
        {
            x = x1;
            y = y1;
        }
        public T this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    public interface Mappable<T>
    {
        S Map<Q, S>(Func<T, Q> f, ref S res);
    }

    public interface Mappable2<T> : Mappable<T>
    {
        U Map<Q, R, S, U>(Func<T, Q, R> f, S m2, ref U res);
    }

    public interface MappableD<M>
    {
        M Map(Func<double,double> f, M res);
    }

    public interface Mappable2D<M> : MappableD<M>
    {
        M Map(Func<double, double, double> f, M m2, M res);
    }
    public abstract class IntegrableFunction<T, Q>
    {
        public abstract Func<T, Q> F { get; }
        public abstract Func<T, Q> FI { get; }
        public abstract IntegrableFunction<T, Q> StrongInt { get; }

        public class SumFunc<R, S> : IntegrableFunction<R, S>
        {
            IntegrableFunction<R, S> f1, f2;
            Func<R, S> fSt;
            Func<R, S> fISt;
            public override Func<R, S> F
            {
                get
                {
                    return fSt;
                }
            }
            public override Func<R, S> FI
            {
                get
                {
                    return fISt;
                }
            }
            public override IntegrableFunction<R, S> StrongInt
            {
                get
                {
                    return new SumFunc<R, S>(f1.StrongInt, f2.StrongInt);
                }
            }
            public SumFunc(IntegrableFunction<R, S> f1a, IntegrableFunction<R, S> f2a)
            {
                f1 = f1a;
                f2 = f2a;
#if(USING_EXPRESSIONS)
                fSt = x => Operations<S>.AddT(f1.F(x),f2.F(x));
                fISt = x => Operations<S>.AddT(f1.FI(x),f2.FI(x));
#else
                fSt = x => (dynamic)f1.F(x) + f2.F(x);
                fISt = x => (dynamic)f1.FI(x) + f2.FI(x);
#endif
            }
        }
        public class MultFunc<R, S> : IntegrableFunction<R, S>
        {
            IntegrableFunction<R, S> f1;
            S val;
            Func<R, S> fSt;
            Func<R, S> fISt;
            public override Func<R, S> F
            {
                get
                {
                    return fSt;
                }
            }
            public override Func<R, S> FI
            {
                get
                {
                    return fISt;
                }
            }
            public override IntegrableFunction<R, S> StrongInt
            {
                get
                {
                    return new MultFunc<R, S>(f1.StrongInt, val);
                }
            }
            public MultFunc(IntegrableFunction<R, S> f1a, S val)
            {
                f1 = f1a;
                this.val = val;
#if(USING_EXPRESSIONS)
                fSt = x => Operations<S>.MulT(f1.F(x), val);
                fISt = x => Operations<S>.MulT(f1.FI(x), val);
#else
                fSt = x => (dynamic)f1.F(x) * val;
                fISt = x => (dynamic)f1.FI(x) * val;
#endif
            }
        }


        public static IntegrableFunction<T, Q> operator +(IntegrableFunction<T, Q> i1, IntegrableFunction<T, Q> i2)
        {
            return new SumFunc<T, Q>(i1, i2);
        }
        public static IntegrableFunction<T,Q> operator *(IntegrableFunction<T, Q> i1, Q r)
        {
            return new MultFunc<T, Q>(i1, r);
        }
    }
    public class PolyFunc<T> : IntegrableFunction<T,T>
    {
        Vector<T> coeffs;
        PolyFunc<T> integral, derivative;
        
        public int Degree
        {
            get
            {
                return coeffs.Dimension-1;
            }
        }

        public override Func<T, T> F
        {
            get
            {
                return this.Evaluate;
            }
        }

        public override Func<T, T> FI
        {
            get
            {
                return this.Integral.F;
            }
        }

        public override IntegrableFunction<T,T> StrongInt
        {
            get
            {
                return this.Integral;
            }
        }

        public PolyFunc<T> Derivative
        {
            get
            {
                if(derivative == null)
                {
                    if(Degree <= 0)
                    {
                        derivative = new PolyFunc<T>();
                    }
                    else
                    {
                        T[] dCos = new T[Degree];
                        for(int i = 1; i < coeffs.Dimension; i++)
                        {
                            dCos[i-1] = (dynamic) i * coeffs[i]; // no choice since i must be int here.
                        }
                        derivative = new PolyFunc<T>(dCos);
                    }
                }
                return derivative;
            }
        }

        public PolyFunc<T> Integral
        {
            get
            {
                if (integral == null)
                {
                    if (Degree < 0)
                    {
                        integral = new PolyFunc<T>();
                    }
                    else
                    {
                        T[] iCos = new T[coeffs.Dimension + 1];
                        for (int i = 0; i < coeffs.Dimension; i++)
                        {
                            iCos[i + 1] = (dynamic)coeffs[i] / (i + 1); // no choice since i must be int here.
                        }
                        derivative = new PolyFunc<T>(iCos);
                    }
                }
                return integral;
            }
        }

        public PolyFunc(params T[] cos)
        {
            int count=0;
            while (Utils.IsZero((dynamic)cos[cos.Length - 1 - count])) count++;
            T[] ans;
            if (count == 0)
            {
                ans = cos;
            }
            else
            {
                ans = new T[cos.Length - count];
                for (int i = 0; i < ans.Length; i++)
                {
                    ans[i] = cos[i];
                }
            }
            coeffs = new Vector<T>(ans);
        }

        public PolyFunc(Vector<T> cos) : this(cos.AsArray)
        {
        }

        public T Evaluate(T t)
        {
            T sum = default(T);
            if (coeffs.Dimension == 0)
                return sum;
            for (int i = coeffs.Dimension - 1; i > 0; i--)
            {
#if(USING_EXPRESSIONS)
                sum = Operations<T>.MulT(Operations<T>.AddT(sum, coeffs[i]),t);
#else
                sum = (dynamic)t * ((dynamic)sum + coeffs[i]);
#endif
            }
#if(USING_EXPRESSIONS)
            sum = Operations<T>.AddT(sum,coeffs[0]);
#else
            sum += (dynamic)coeffs[0];
#endif
            return sum;
        }

        public T this[int i]
        {
            get
            {
                return coeffs[i];
            }
            set
            {
                if (i >= coeffs.Dimension)
                {
                    coeffs = coeffs.MakeDim(i+1);
                }
                coeffs[i] = value;
                integral = null; derivative = null;
            }
        }

        public static PolyFunc<T> operator+(PolyFunc<T> t1, PolyFunc<T> t2)
        {
            Vector<T> c1=t1.coeffs,c2=t2.coeffs;
            if(c1.Dimension<c2.Dimension)
                c1 = c1.MakeDim(c2.Dimension);
            if(c1.Dimension>c2.Dimension)
                c2 = c2.MakeDim(c1.Dimension);
            return new PolyFunc<T>(c1 + c2);
        }

        public static PolyFunc<T> operator *(PolyFunc<T> t1, T t2)
        {
            return new PolyFunc<T>(t1.coeffs * t2);
        }

        public static PolyFunc<T> operator *(T t2, PolyFunc<T> t1)
        {
            return new PolyFunc<T>(t1.coeffs * t2);
        }

        public static PolyFunc<T> operator -(PolyFunc<T> t)
        {
            return new PolyFunc<T>(-t.coeffs);
        }

        public static PolyFunc<T> operator -(PolyFunc<T> t1, PolyFunc<T> t2)
        {
            return t1 + (-t2);
        }

        public static implicit operator PolyFunc<T>(T t)
        {
            return new PolyFunc<T>(t);
        }
    }
}

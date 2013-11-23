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
                fSt = x => (dynamic)f1.F(x) + f2.F(x);
                fISt = x => (dynamic)f1.FI(x) + f2.FI(x);
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
                this.val=val;
                fSt = x => (dynamic)f1.F(x) * val;
                fISt = x => (dynamic)f1.FI(x) * val;
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
    public class PolyFunc<T> // : IntegrableFunction<T,T>
    {
        
    }
}

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
}

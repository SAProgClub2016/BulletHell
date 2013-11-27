using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib.Function
{
    class VectorAggregateFunc<S, T> : IntegrableFunction<S,Vector<T>>
    {
        IntegrableFunction<S,T>[] comps;
        Func<S, Vector<T>> Fst, FIst;

        public VectorAggregateFunc(params IntegrableFunction<S, T>[] comps)
        {
            this.comps = comps;
            Fst = Vector<T>.Aggregate<S>(comps.Map(x => x.F));
            FIst = Vector<T>.Aggregate<S>(comps.Map(x => x.FI));
        }


        public override Func<S, Vector<T>> F
        {
            get
            {
                return Fst;
            }
        }
        public override Func<S, Vector<T>> FI
        {
            get
            {
                return FIst;
            }
        }
        public override IntegrableFunction<S, Vector<T>> StrongInt
        {
            get
            {
                return new VectorAggregateFunc<S, T>(comps.Map(x => x.StrongInt));
            }
        }
    }
}

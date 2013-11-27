#define USING_EXPRESSIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

namespace BulletHell.MathLib.Function
{

    
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
                fSt = x => Operations<S>.Add(f1.F(x),f2.F(x));
                fISt = x => Operations<S>.Add(f1.FI(x),f2.FI(x));
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
                fSt = x => Operations<S>.Mul(f1.F(x), val);
                fISt = x => Operations<S>.Mul(f1.FI(x), val);
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

        public static IntegrableFunction<T,Q> SplitAt(IntegrableFunction<T,Q> f1, T t, IntegrableFunction<T, Q> f, bool seal = false)
        {
            return new SplitFunction<T,Q>(f1, t, f, seal);
        }
        public class SplitFunction<R,S> : IntegrableFunction<R, S>
        {
            Func<R, S> fSt, fISt;
            IntegrableFunction<R,S> f1,f2;
            R t;
            S ft,st, fit, sit, deltat, deltait;
            bool s;

            public SplitFunction(IntegrableFunction<R, S> first, R tsplit, IntegrableFunction<R, S> second, bool seal = false)
            {
                s = seal;
                f1 = first;
                f2 = second;
                t = tsplit;
                ft = first.F(t);
                fit = first.FI(t);
                sit = second.FI(t);
                st = second.F(t);
#if(USING_EXPRESSIONS)
                deltat = Operations<S>.Sub(ft, st);
                deltait = Operations<S>.Sub(fit, sit);
#else
                deltat = (dynamic)ft - st;
                deltait = (dynamic)fit - sit;
#endif
                if (seal)
                {
                    f2 += new PolyFunc<S,R>(deltat);

                    sit = f2.FI(t);
#if(USING_EXPRESSIONS)
                    deltait = Operations<S>.Sub(fit, sit);
#else
                    deltait = (dynamic)fit - sit;
#endif
                }

                fSt = x =>
                {
                    if ((dynamic)x < t)
                    {
                        return f1.F(x);
                    }
                    return f2.F(x);
                };
                fISt = x =>
                {
                    if ((dynamic)x < t)
                    {
                        return f1.FI(x);
                    }
#if(USING_EXPRESSIONS)
                    return Operations<S>.Add(f2.FI(x), deltait);
#else
                    return (dynamic)f2.FI(x)+deltait;
#endif
                };
            }

            public override Func<R, S> F
            {
                get { return fSt; }
            }

            public override Func<R, S> FI
            {
                get { return fISt; }
            }

            public override IntegrableFunction<R, S> StrongInt
            {
                get
                {
                    return new SplitFunction<R,S>(f1.StrongInt, t, f2.StrongInt,true);
                }
            }
        }
    }
    
}

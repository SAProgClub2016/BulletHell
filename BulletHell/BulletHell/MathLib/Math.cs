using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib
{
    public delegate Pair<int> CoordTransformF<T>(Matrix<T> m, int r, int c);
    public delegate Pair<int> DimensionTransformF(int r, int c);

    public struct CoordTransform<T>
    {
        public CoordTransformF<T> CoordF;
        public DimensionTransformF DimF;
        public CoordTransform(CoordTransformF<T> cf, DimensionTransformF df)
        {
            CoordF=cf;
            DimF=df;
        }
    }

    public class MathTest
    {
        public static void Main()
        {
            Matrix<double> m = Matrix<double>.Identity<double>(3);
            WriteMatrix("M", m);
            Matrix<double> m3 = 3 * m;
            WriteMatrix("3*M", m3);
            Pause();
            Matrix<double> m2 = new Matrix<double>(new double[,] {{0,1,2},{3,4,5},{6,7,8}});
            WriteMatrix("N",m2);
            Func<double,double> square = x => x*x;
            //Matrix<double> mf = m2.Map(square);
            //WriteMatrix("Elementwise square of N: A", mf);
            //Matrix<double> mfm = mf * m2;
            //WriteMatrix("A*N", mfm);
            Pause();
            Matrix<double> m20 = new Matrix<double>(new double[,] {{0,1},{1,0}});
            WriteMatrix("B", m20);
            WriteMatrix("B*B", m20*m20);
            Matrix<double> m21 = new Matrix<double>(new double[,] { { 0, 1 }, { 2, 3 } });
            WriteMatrix("C", m21);
            WriteMatrix("B*C", m20 * m21);
            WriteMatrix("B+C", m20 + m21);
            Pause();
            Vector<double> v1 = new Vector<double>(1, 2, 4);
            P("v1",v1);
            Vector<double> v1u = v1.Unit;
            P("unit of v1",v1u);
            Vector<double> v2 = new Vector<double>(1, 2, 2);
            P("v2", v2);
            P("unit of v2", v2.Unit);
            P("v1*v2", v1 * v2);
            P("v1+v2", v1 + v2);
            P("3*v1",3 * v1);
            Pause();
            P("v1", v1);
            P("m", m);
            P("v1*m", v1 * m);
            Pause();
            P("v1", v1);
            P("m3", m3);
            P("v1*m", v1 * m3);
            Pause();
            Matrix<double> m3rref = m3.RREF();
            WriteMatrix("RREF of 3*M",m3rref);
            Matrix<double> ml = new Matrix<double>(new double[,]
                {
                    {1,2,3,10},
                    {2,4,1,15},
                    {5,1,2,19}
                },false);
            Matrix<double> mlrref = new Matrix<double>(ml);
            mlrref.RREF();
            WriteMatrix("L", ml);
            WriteMatrix("RREF of L", mlrref);
            Pause();
        }
        public static void P(string name, object o)
        {
            Console.WriteLine("{0}: {1}",name, o);
        }
        public static void WriteMatrix<T>(string name, Matrix<T> m)
        {
            Console.WriteLine("Matrix: name: {2}, determinant: {0}, value: {1}", m.Determinant, m, name);
        }
        public static void WriteMatrix<T>(Matrix<T> m)
        {
            Console.WriteLine("Matrix: determinant: {0}, value: {1}", m.Determinant, m);
        }
        public static void Pause()
        {
            Console.ReadKey();
        }
    }
}
    /*
    class Module2D<S,R> : Module<Module2D<S,R>, R> where R : Ring<R>
    {
        public WRing<S,R> x, y;
        public Module2D(R x1, R y1)
        {
            x = x1;
            y = y1;
        }
        public Module2D<R> Zero()
        {
            R z = x.Zero();
            return new Module2D<R>(z,z);
        }
        public Module2D<R> Negate()
        {
            return new Module2D<R>(x.Negate(), y.Negate());
        }
        public Module2D<R> Plus(Module2D<R> o)
        {
            return new Module2D<R>(x + o.x, y + o.y);
        }
        public Module2D<R> Times(R d)
        {
            return new Module2D<R>(d * x, d * y);
        }
        public R Dot(Module2D<R> o)
        {
            return x * o.x + y * o.y;
        }
    }
    */
    /*
    class Vector2D<F> : Module2D<F>, Vector<Vector2D<F>,F>
    {
        public Vector2D(F x1, F y1) : base(x1,y1) { }
        public static Vector2D<F> operator /(Vector2D<F> v1, F d)
        {
            return new Vector2D(v1.x / d, v1.y / d);
        }
        public double Length2
        {
            get
            {
                return this * this;
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(Length2);
            }
            set
            {
                Vector2D ans = this * value / Length;
                x = ans.x;
                y = ans.y;
            }
        }
    }
    */
    /*
    public interface AbGroup<G>
    {
        G Plus(G o);
        G Zero();
        G Negate();
    }
    public interface Ring<R> : Module<R,R> where R : Ring<R>
    {
        // R Zero(); - from Module<R,R>
        // R Negate(); - from AbGroup<R>
        // R Plus(R r2); - from Module<R,R>
        // R Times(R r2); - from Module<R,R>
    }
    public interface Field<F> : Ring<F>, Vector<F,F> where F : Field<F>
    {
        F Unit();
        F Invert();
        // F Div(F o); - in Vector<F,F>
    }
    public class Wrapper<S, R> where S : Wrapper<S, R>, new()
    {
        public R Element;
        public static S testWrap(R r)
        {
            return r;
        }
        public static implicit operator S(R r)
        {
            S w = new S();
            w.Element = r;
            return w;
        }
        public static implicit operator R(S w)
        {
            return w.Element;
        }
    }
    public class WRing<S,R> : Wrapper<S,R>, Ring<S> where R : Ring<R> where S : WRing<S,R>,new()
    {
        public WRing(R r)
        {
            Element = r;
        }
        public S Zero()
        {
            return Element.Zero();
        }
        public WRing<S,R> Plus(WRing<S,R> r2)
        {
            return Element.Plus(r2.Element);
        }
        public WRing<S,R> Negate()
        {
            return Element.Negate();
        }
        public WRing<S,R> Times(WRing<S,R> r2)
        {
            return Element.Times(r2.Element);
        }
        public WRing<S,R> Dot(WRing<S,R> r2)
        {
            return Element.Times(r2.Element);
        }
        public static WRing<S,R> operator +(WRing<S,R> r1, WRing<S,R> r2)
        {
            return r1.Plus(r2);
        }
        public static WRing<S,R> operator -(WRing<S,R> r1)
        {
            return r1.Negate();
        }
        public static WRing<S,R> operator -(WRing<S,R> r1, WRing<S,R> r2)
        {
            return r1 + (-r2);
        }
        public static WRing<S,R> operator *(WRing<S,R> r1, WRing<S,R> r2)
        {
            return r1.Times(r2);
        }
        public static implicit operator S(R r)
        {
            S w = new S();
            w.Element = r;
            return w;
        }
        public static implicit operator R(S w)
        {
            return w.Element;
        }
    }
    public class BaseField<F> : BaseRing<F>, Field<BaseField<F>> where F : Field<F>
    {
        public BaseField(F f) : base(f) { }
        public BaseField<F> Unit()
        {
            return Element.Unit();
        }
        public BaseField<F> Invert()
        {
            return Element.Invert();
        }
        public BaseField<F> Div(BaseField<F> o)
        {
            return Element.Div(o.Element);
        }
        public static BaseField<F> operator /(BaseField<F> f1, BaseField<F> f2)
        {
            return f1.Div(f2);
        }
        public static implicit operator F(BaseField<F> k)
        {
            return k.Element;
        }
        public static implicit operator BaseField<F>(F f)
        {
            return new BaseField<F>(f);
        }
    }
    public interface Module<T,R> : AbGroup<T> where R : Ring<R>
    {
        T Times(R r);
        R Dot(T t);
    }
    public class BaseModule<T,R> : Module<BaseModule<T,R>,R> where T : Module<T,R> where R : Ring<R>
    {
        public T Element;
        public BaseModule(T t)
        {
            Element = t;
        }
        public BaseModule<T, R> Zero()
        {
            return Element.Zero();
        }
        public BaseModule<T, R> Plus(BaseModule<T, R> o)
        {
            return Element.Plus(o.Element);
        }
        public BaseModule<T, R> Negate()
        {
            return Element.Negate();
        }
        public BaseModule<T, R> Times(R r)
        {
            return Element.Times(r);
        }
        public R Dot(BaseModule<T, R> o)
        {
            return Element.Dot(o.Element);
        }
        public static implicit operator T(BaseModule<T, R> k)
        {
            return k.Element;
        }
        public static implicit operator BaseModule<T, R>(T t)
        {
            return new BaseModule<T, R>(t);
        }
        public static BaseModule<T,R> operator *(R d, BaseModule<T,R> v)
        {
            return v.Element.Times(d);
        }
        public static BaseModule<T,R> operator *(BaseModule<T,R> v, R d)
        {
            return v.Element.Times(d);
        }
        public static R operator *(BaseModule<T,R> v1, BaseModule<T,R> v2)
        {
            return v1.Element.Dot(v2.Element);
        }
        public static BaseModule<T,R> operator +(BaseModule<T,R> v1, BaseModule<T,R> v2)
        {
            return v1.Element.Plus(v2.Element);
        }
        public R Length2
        {
            get
            {
                return this * this;
            }
        }
    }
    public interface Vector<T, F> : Module<T, F> where F : Field<F>
    {
        T Div(F f);
    }
    public class BaseVector<T, F> : BaseModule<T, F>, Vector<BaseVector<T, F>, F>
        where F : Field<F>
        where T : Vector<T, F>
    {
        public static implicit operator T(BaseVector<T, F> k)
        {
            return k.Element;
        }
        public static implicit operator BaseVector<T, F>(T t)
        {
            return new BaseVector<T, F>(t);
        }
        public BaseVector(T t) : base(t) { }
        public BaseVector<T, F> Div(F f)
        {
            return Element.Div(f);
        }
    }
}
    */
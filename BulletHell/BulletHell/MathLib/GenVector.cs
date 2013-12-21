#define USING_EXPRESSIONS

using BulletHell.MathLib.Function;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace BulletHell.MathLib
{
    public struct Vector<T> //: Mappable<T>
    {
        T[] vec;

        public Vector(int dim)
        {
            vec = new T[dim];
        }
        public Vector(T[] v, int offset = 0, int dim = -1)
        {
            if (v == null)
                throw new ArgumentNullException();
            int indmax = 0;
            if (dim > 0)
            {
                vec = new T[dim];
                indmax = Math.Min(dim, Math.Max(v.Length - offset, 0));
            }
            else
            {
                vec = new T[Math.Max(v.Length - offset, 0)];
                indmax = Math.Max(v.Length - offset, 0);
            }
            for (int i = 0; i < indmax; i++)
            {
                this[i] = v[i + offset];
            }
        }
        public Vector(Vector<T> o, int offset = 0, int dim = -1)
            : this(o.vec, offset, dim)
        {
        }

        public static Vector<T> Fill<Q>(int dimension, Q q, Func<Q,T> f)
        {
            Vector<T> ans = new Vector<T>(dimension);
            for (int i = 0; i < dimension; i++)
            {
                ans[i] = f(q);
            }
            return ans;
        }

        public Vector<T> MakeDim(int dim)
        {
            Vector<T> ans = new Vector<T>(dim);
            int i = 0;
            for (; i < dim && i < this.Dimension; i++)
            {
                ans[i] = this[i];
            }
            return ans;
        }
        public Vector(params T[] v)
            : this(v, (int)0)
        {
            if (v == null)
                throw new ArgumentNullException();
        }

        public static Vector<S> MakeStandardBasisVector<S>(int dimension, int basisDim)
        {
            Vector<S> ans = new Vector<S>(dimension);
            ans[basisDim] = (dynamic)1;
            return ans;
        }

        public T Dot(Vector<T> v2)
        {
            ValidateDimensions(this, v2, "Dot(Vector<T> v2)", "this", "v2");
            T sum = default(T);
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                sum = Operations<T>.Add(sum, Operations<T>.Mul(this[i], v2[i]));
#else
                sum += (dynamic) this[i]*v2[i];
#endif
            }
            return sum;
        }
        public Vector<T> Negate(Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Neg(this[i]);
#else
                res[i] = -(dynamic)this[i];
#endif
                }
            return res;
        }
        public Vector<T> Multiply(T d, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Mul(d, this[i]);
#else
                res[i] = (dynamic)d*this[i];
#endif
            }
            return res;
        }
        public Vector<T> MultiplyE(Vector<T> o, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Mul(o[i], this[i]);
#else
                res[i] = (dynamic)d*this[i];
#endif
            }
            return res;
        }
        public Vector<T> Add(Vector<T> v2, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Add(Vector<T> v2, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Add(v2[i],this[i]);
#else
                res[i] = (dynamic)v2[i]+this[i];
#endif
            }
            return res;
        }
        public Vector<T> Subtract(Vector<T> v2, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Subtract(Vector<T> v2, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Sub(this[i], v2[i]);
#else
                res[i] = (dynamic)this[i] - v2[i];
#endif
            }
            return res;
        }
        public Vector<T> LComb(T a, Vector<T> v2, T b, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.LComb(T a, Vector<T> v2, T b, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
#if(USING_EXPRESSIONS)
                res[i] = Operations<T>.Add(Operations<T>.Mul(b, v2[i]), Operations<T>.Mul(a, this[i]));
#else
                res[i] =(dynamic)b*v2[i]+(dynamic)a*this[i];
#endif
            }
            return res;
        }
        public Vector<T> Divide(T d, Vector<T> res = default(Vector<T>))
        {
            return Multiply(1 / (dynamic)d, res);
        }

        public Vector<Q> Map<Q>(Func<T, Q> f, Vector<Q> res = default(Vector<Q>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i]);
            }
            return res;
        }
        public Vector<Q> Map<S,Q>(Func<T, S, Q> f, Vector<S> o, Vector<Q> res = default(Vector<Q>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, o, "public Vector<Q> Map<S,Q>(Func<T, S, Q> f, Vector<S> o, Vector<Q> res = default(Vector<Q>))", "this", "o");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i],o[i]);
            }
            return res;
        }
        public static Func<S, Vector<T>> Aggregate<S>(params Func<S, T>[] comps)
        {
            return x =>
            {
                Vector<T> ans = new Vector<T>(comps.Length);
                for (int i = 0; i < ans.Dimension; i++)
                {
                    ans[i] = comps[i](x);
                }
                return ans;
            };
        }
        public static IntegrableFunction<S, Vector<T>> Aggregate<S>(params IntegrableFunction<S, T>[] comps)
        {
            return new VectorAggregateFunc<S, T>(comps);
        }
        /*
        public S Map<Q, S>(Func<T, Q> f, ref S res) where S : struct
        {
            Vector<Q>? rn = res as Vector<Q>?;
            if (rn == null)
            {
                throw new ArgumentException("Res is not of correct type");
            }
            Vector<Q> r = rn.Value;
            bool reassign = false;
            if (r.vec == null)
            {
                r = new Vector<Q>(Dimension);
                reassign = true;
            }
            for (int i = 0; i < Dimension; i++)
            {
                r[i] = f(this[i]);
            }
            if (reassign)
            {
                Nullable<S> q = r as Nullable<S>;
                if (q == null)
                    throw new ArgumentException("Res is not of correct type");
                else
                    res = q.Value;
            }
            return res;
        }
        /*public Vector<T> Map(Func<double,double> f, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i]);
            }
            return res;
        }
        public Vector<T> Map(Func<double,double,double> f, Vector<T> v2, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Map(Function2 f, Vector<T> v2, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i], v2[i]);
            }
            return res;
        }*/

        public Vector<T> Multiply(Matrix<T> m, Vector<T> res = default(Vector<T>))
        {
            if (m.Rows != Dimension)
                throw new InvalidOperationException(string.Format("Vector.Multiply(Matrix<T> m) - Dimension Mismatch - m.Rows[{0}] != this.Dimension[{1}]", m.Rows, this.Dimension));
            MakeOrValidate(ref res, m.Cols);
            if (res.vec == this.vec)
                throw new InvalidOperationException("Vector.Multiply(Matrix<T> m) - Cannot multiply in place");
            for (int c = 0; c < m.Cols; c++)
            {
                T sum = default(T);
                for (int r = 0; r < m.Rows; r++)
                {
#if(USING_EXPRESSIONS)
                    sum = Operations<T>.Add(sum, Operations<T>.Mul(this[r], m[r, c]));
#else
                    sum += (dynamic) this[r]*m[r,c];
#endif
                }
                res[c] = sum;
            }

            return res;
        }
        public Vector<T> MultiplyL(Matrix<T> m, Vector<T> res = default(Vector<T>))
        {
            if (m.Cols != Dimension)
                throw new InvalidOperationException(string.Format("Vector.Multiply(Matrix<T> m) - Dimension Mismatch - m.Rows[{0}] != this.Dimension[{1}]", m.Rows, this.Dimension));
            MakeOrValidate(ref res, m.Rows);
            if (res.vec == this.vec)
                throw new InvalidOperationException("Vector.Multiply(Matrix<T> m) - Cannot multiply in place");
            for (int r = 0; r < m.Rows; r++)
            {
                T sum = default(T);
                for (int c = 0; c < m.Cols; c++)
                {
#if(USING_EXPRESSIONS)
                    sum = Operations<T>.Add(sum, Operations<T>.Mul(this[c], m[r, c]));
#else
                    sum += (dynamic) this[c] * m[r,c];
#endif
                }
                res[r] = sum;
            }

            return res;
        }

        public static Vector<T> operator -(Vector<T> v1)
        {
            return v1.Negate();
        }
        public static Vector<T> operator *(Vector<T> v1, T d)
        {
            return v1.Multiply(d);
        }
        public static Vector<T> operator *(T d, Vector<T> v1)
        {
            return v1.Multiply(d);
        }
        public static Vector<T> operator /(Vector<T> v1, T d)
        {
            return v1.Divide(d);
        }
        public static Vector<T> operator -(Vector<T> v1, Vector<T> v2)
        {
            return v1.Subtract(v2);
        }
        public static Vector<T> operator +(Vector<T> v1, Vector<T> v2)
        {
            return v1.Add(v2);
        }
        public static T operator *(Vector<T> v1, Vector<T> v2)
        {
            return v1.Dot(v2);
        }
        public static Vector<T> operator *(Matrix<T> m, Vector<T> v)
        {
            return v.MultiplyL(m);
        }
        public static Vector<T> operator *(Vector<T> v, Matrix<T> m)
        {
            return v.Multiply(m);
        }

        private static void MakeOrValidate<S>(ref Vector<S> v1, int dimension)
        {
            if (v1.vec == null)
            {
                v1.vec = new S[dimension];
            }
            else
            {
                ValidateDimensions(v1, dimension, "MakeOrValidate(Vector<T> v1, int dimension)", "v1");
            }
        }
        private static void ValidateDimensions<Q,S>(Vector<Q> v1, Vector<S> v2, string method, string pName1, string pName2)
        {
            if (v1.Dimension != v2.Dimension)
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: {1}({2}) {3}({4})", method, pName1, v1.Dimension, pName2, v2.Dimension));
            }
        }
        private static void ValidateDimensions<S>(Vector<S> v1, int dim, string method, string pName1)
        {
            if (v1.Dimension != dim)
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: expected(({1}) given {2}({3})", method, dim, pName1, v1.Dimension));
            }
        }

        public Vector<T> Unit
        {
            get
            {
                return this / (dynamic)Length;
            }
        }
        public int Dimension
        {
            get
            {
                return vec.Length;
            }
        }

        public T Length2
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
                return Math.Sqrt((double)(dynamic)Length2);
            }
            set
            {
                double d = value / Length;
                Multiply((dynamic)d, this);
            }
        }
        public Matrix<T> AsColumnMatrix
        {
            get
            {
                Matrix<T> res = new Matrix<T>(Dimension, 1);
                for (int i = 0; i < Dimension; i++)
                {
                    res[i, 1] = this[i];
                }
                return res;
            }
        }
        public Matrix<T> AsRowMatrix
        {
            get
            {
                Matrix<T> res = new Matrix<T>(1, Dimension);
                for (int i = 0; i < Dimension; i++)
                {
                    res[1, i] = this[i];
                }
                return res;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append("<");
            for (int i = 0; i < this.Dimension - 1; i++)
            {
                b.Append(string.Format("{0}, ", this[i]));
            }
            b.Append(string.Format("{0}", this[Dimension - 1]));
            b.Append(">");
            return b.ToString();
        }

        public T this[int i]
        {
            get
            {
                return vec[i];
            }
            set
            {
                vec[i] = value;
            }
        }

        public T[] AsArray
        {
            get
            {
                return vec;
            }
        }
    }
}
using System;
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
                sum += (dynamic) this[i] * v2[i];
            }
            return sum;
        }
        public Vector<T> Negate(Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = -(dynamic)this[i];
            }
            return res;
        }
        public Vector<T> Multiply(T d, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = (dynamic) d * this[i];
            }
            return res;
        }
        public Vector<T> Add(Vector<T> v2, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Add(Vector<T> v2, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = (dynamic) v2[i] + this[i];
            }
            return res;
        }
        public Vector<T> Subtract(Vector<T> v2, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Subtract(Vector<T> v2, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = (dynamic) v2[i] - this[i];
            }
            return res;
        }
        public Vector<T> LComb(T a, Vector<T> v2, T b, Vector<T> res = default(Vector<T>))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.LComb(T a, Vector<T> v2, T b, Vector<T> res = default(Vector<T>))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = (dynamic) b * v2[i] + (dynamic) a * this[i];
            }
            return res;
        }
        public Vector<T> Divide(T d, Vector<T> res = default(Vector<T>))
        {
            return Multiply(1 / (dynamic)d, res);
        }/*
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
                    sum += (dynamic) this[r] * m[r, c];
                }
                res[c] = (dynamic)sum; //TODO come fix this.
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
                    sum += (dynamic)this[c] * m[r, c];
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

        private static void MakeOrValidate(ref Vector<T> v1, int dimension)
        {
            if (v1.vec == null)
            {
                v1.vec = new T[dimension];
            }
            else
            {
                ValidateDimensions(v1, dimension, "MakeOrValidate(Vector<T> v1, int dimension)", "v1");
            }
        }
        private static void ValidateDimensions(Vector<T> v1, Vector<T> v2, string method, string pName1, string pName2)
        {
            if (v1.Dimension != v2.Dimension)
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: {1}({2}) {3}({4})", method, pName1, v1.Dimension, pName2, v2.Dimension));
            }
        }
        private static void ValidateDimensions(Vector<T> v1, int dim, string method, string pName1)
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
    }
}
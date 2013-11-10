using System;
using System.Net;
using System.Text;

namespace BulletHell.MathLib.Deprecated
{
    public struct Vector : Mappable2D<Vector>
    {
        double[] vec;

        public Vector(int dim)
        {
            vec = new double[dim];
        }
        public Vector(double[] v, int offset = 0, int dim = -1)
        {
            int indmax = 0;
            if (dim > 0)
            {
                vec = new double[dim];
                indmax = Math.Min(dim, Math.Max(v.Length - offset, 0));
            }
            else
            {
                vec = new double[Math.Max(v.Length - offset, 0)];
                indmax = Math.Max(v.Length - offset, 0);
            }
            for (int i = 0; i < indmax; i++)
            {
                this[i] = v[i + offset];
            }
        }
        public Vector(Vector o, int offset = 0, int dim = -1)
            : this(o.vec, offset, dim)
        {
        }
        public Vector MakeDim(int dim)
        {
            Vector ans = new Vector(dim);
            int i = 0;
            for (; i < dim && i < this.Dimension; i++)
            {
                ans[i] = this[i];
            }
            return ans;
        }
        public Vector(params double[] v)
            : this(v, (int)0)
        {
        }

        public static Vector MakeStandardBasisVector(int dimension, int basisDim)
        {
            Vector ans = new Vector(dimension);
            ans[basisDim] = 1;
            return ans;
        }

        public double Dot(Vector v2)
        {
            ValidateDimensions(this, v2, "Dot(Vector v2)", "this", "v2");
            double sum = 0;
            for (int i = 0; i < this.Dimension; i++)
            {
                sum += this[i] * v2[i];
            }
            return sum;
        }
        public Vector Negate(Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = -this[i];
            }
            return res;
        }
        public Vector Multiply(double d, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = d * this[i];
            }
            return res;
        }
        public Vector Add(Vector v2, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Add(Vector v2, Vector res = default(Vector))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = v2[i] + this[i];
            }
            return res;
        }
        public Vector Subtract(Vector v2, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Subtract(Vector v2, Vector res = default(Vector))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = v2[i] - this[i];
            }
            return res;
        }
        public Vector LComb(double a, Vector v2, double b, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.LComb(double a, Vector v2, double b, Vector res = default(Vector))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = b * v2[i] + a * this[i];
            }
            return res;
        }
        public Vector Divide(double d, Vector res = default(Vector))
        {
            return Multiply(1 / d, res);
        }

        public Vector Map(Func<double,double> f, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i]);
            }
            return res;
        }
        public Vector Map(Func<double,double,double> f, Vector v2, Vector res = default(Vector))
        {
            MakeOrValidate(ref res, this.Dimension);
            ValidateDimensions(this, v2, "Vector.Map(Function2 f, Vector v2, Vector res = default(Vector))", "this", "v2");
            for (int i = 0; i < this.Dimension; i++)
            {
                res[i] = f(this[i], v2[i]);
            }
            return res;
        }

        public Vector Multiply(Matrix m, Vector res = default(Vector))
        {
            if (m.Rows != Dimension)
                throw new InvalidOperationException(string.Format("Vector.Multiply(Matrix m) - Dimension Mismatch - m.Rows[{0}] != this.Dimension[{1}]", m.Rows, this.Dimension));
            MakeOrValidate(ref res, m.Cols);
            if (res.vec == this.vec)
                throw new InvalidOperationException("Vector.Multiply(Matrix m) - Cannot multiply in place");
            for (int c = 0; c < m.Cols; c++)
            {
                double sum = 0;
                for (int r = 0; r < m.Rows; r++)
                {
                    sum += this[r] * m[r, c];
                }
                res[c] = sum;
            }

            return res;
        }
        public Vector MultiplyL(Matrix m, Vector res = default(Vector))
        {
            if (m.Cols != Dimension)
                throw new InvalidOperationException(string.Format("Vector.Multiply(Matrix m) - Dimension Mismatch - m.Rows[{0}] != this.Dimension[{1}]", m.Rows, this.Dimension));
            MakeOrValidate(ref res, m.Rows);
            if (res.vec == this.vec)
                throw new InvalidOperationException("Vector.Multiply(Matrix m) - Cannot multiply in place");
            for (int r = 0; r < m.Rows; r++)
            {
                double sum = 0;
                for (int c = 0; c < m.Cols; c++)
                {
                    sum += this[c] * m[r, c];
                }
                res[r] = sum;
            }

            return res;
        }

        public static Vector operator -(Vector v1)
        {
            return v1.Negate();
        }
        public static Vector operator *(Vector v1, double d)
        {
            return v1.Multiply(d);
        }
        public static Vector operator *(double d, Vector v1)
        {
            return v1.Multiply(d);
        }
        public static Vector operator /(Vector v1, double d)
        {
            return v1.Divide(d);
        }
        public static Vector operator -(Vector v1, Vector v2)
        {
            return v1.Subtract(v2);
        }
        public static Vector operator +(Vector v1, Vector v2)
        {
            return v1.Add(v2);
        }
        public static double operator *(Vector v1, Vector v2)
        {
            return v1.Dot(v2);
        }
        public static Vector operator *(Matrix m, Vector v)
        {
            return v.MultiplyL(m);
        }
        public static Vector operator *(Vector v, Matrix m)
        {
            return v.Multiply(m);
        }

        private static void MakeOrValidate(ref Vector v1, int dimension)
        {
            if (v1.vec == null)
            {
                v1.vec = new double[dimension];
            }
            else
            {
                ValidateDimensions(v1, dimension, "MakeOrValidate(Vector v1, int dimension)", "v1");
            }
        }
        private static void ValidateDimensions(Vector v1, Vector v2, string method, string pName1, string pName2)
        {
            if (v1.Dimension != v2.Dimension)
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: {1}({2}) {3}({4})", method, pName1, v1.Dimension, pName2, v2.Dimension));
            }
        }
        private static void ValidateDimensions(Vector v1, int dim, string method, string pName1)
        {
            if (v1.Dimension != dim)
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: expected(({1}) given {2}({3})", method, dim, pName1, v1.Dimension));
            }
        }

        public Vector Unit
        {
            get
            {
                return this / Length;
            }
        }
        public int Dimension
        {
            get
            {
                return vec.Length;
            }
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
                double d = value / Length;
                Multiply(d, this);
            }
        }
        public Matrix AsColumnMatrix
        {
            get
            {
                Matrix res = new Matrix(Dimension, 1);
                for (int i = 0; i < Dimension; i++)
                {
                    res[i, 1] = this[i];
                }
                return res;
            }
        }
        public Matrix AsRowMatrix
        {
            get
            {
                Matrix res = new Matrix(1, Dimension);
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

        public double this[int i]
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
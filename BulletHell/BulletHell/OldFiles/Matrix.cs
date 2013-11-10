using System;
using System.Net;
using System.Text;
using System.Linq;

namespace BulletHell.MathLib.Deprecated
{
    public class Matrix : Mappable2D<Matrix>
    {
        // Data
        private double[,] mat;

        // Offsets
        private int offC, offR;
        private bool offsets;

        // Determinant stuff
        private double determinant;
        private bool detValid;

        // Dimensions
        public int Rows { get; protected set; }
        public int Cols { get; protected set; }

        // Constructors
        public Matrix(int rows, int cols)
        {
            mat = new double[rows, cols];
            determinant = 0;
            offC = offR = 0;
            offsets = false;
            detValid = true;
            Rows = rows;
            Cols = cols;
        }
        public Matrix(Pair<int> dims)
            : this(dims.x, dims.y)
        {
        }
        public Matrix(int sqSize)
            : this(sqSize, sqSize)
        {
        }
        public Matrix(Matrix m, bool copy = true, int offR = 0, int offC = 0, int rows = -1, int cols = -1)
            : this(m.mat, copy, offR, offC, rows, cols)
        {
        }
        public Matrix(double[,] m, bool copy = true, int offR = 0, int offC = 0, int rows = -1, int cols = -1)
        {
            if (!copy)
            {
                mat = m;
                this.offC = offC;
                this.offR = offR;

                if (rows > 0)
                {
                    Rows = rows;
                    if (rows > m.GetLength(0) - offR)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Rows = m.GetLength(0) - offR;
                }
                if (cols > 0)
                {
                    Cols = cols;
                    if (cols > m.GetLength(1) - offC)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Cols = m.GetLength(1) - offC;
                }
            }
            else
            {
                this.offC = 0;
                this.offR = 0;
                offsets = false;

                if (rows > 0)
                {
                    Rows = rows;
                    if (rows > m.GetLength(0) - offR)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Rows = m.GetLength(0) - offR;
                }
                if (cols > 0)
                {
                    Cols = cols;
                    if (cols > m.GetLength(1) - offC)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Cols = m.GetLength(1) - offC;
                }
                mat = new double[Rows, Cols];
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Cols; c++)
                    {
                        mat[r, c] = m[r + offC, c + offR];
                    }
                }
            }
            detValid = false;
            determinant = 0;
        }

        // Nice operations
        public Matrix Add(Matrix m2, Matrix res = null)
        {
            ValidateDimensions(this, m2, "Matrix.Add(Matrix m2, Matrix res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = this[r, c] + m2[r, c];
                }
            }
            return res;
        }
        public Matrix Subtract(Matrix m2, Matrix res = null)
        {
            ValidateDimensions(this, m2, "Matrix.Sub(Matrix m2, Matrix res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = this[r, c] - m2[r, c];
                }
            }
            return res;
        }
        public Matrix Negate(Matrix res = null)
        {
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = -this[r, c];
                }
            }
            return res;
        }
        public Matrix Multiply(double d, Matrix res = null)
        {
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = d * this[r, c];
                }
            }
            return res;
        }
        public Matrix Divide(double d, Matrix res = null)
        {
            return Multiply(1 / d, res);
        }
        public Matrix LComb(double a, Matrix m2, double b, Matrix res = null)
        {
            ValidateDimensions(this, m2, "Matrix.LComb(double a, Matrix m2, double b, Matrix res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = a * this[r, c] + b * m2[r, c];
                }
            }
            return res;
        }
        public Matrix Multiply(Matrix m2, Matrix res = null)
        {
            if (Cols != m2.Rows)
            {
                throw new InvalidOperationException(string.Format("Error - Matrix.Multiply(Matrix m2, Matrix res = null) - Dimension Mismatch - this.Cols [{0}] != m2.Rows [{1}]", Cols, m2.Rows));
            }
            if (res == this || res == m2)
            {
                throw new InvalidOperationException("Error - Matrix.Multiply(Matrix m2, Matrix res = null) - Cannot multiply in place");
            }
            MakeOrValidate(Rows, m2.Cols, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < m2.Cols; c++)
                {
                    double sum = 0;
                    for (int i = 0; i < Cols; i++)
                    {
                        sum += this[r, i] * m2[i, c];
                    }
                    res[r, c] = sum;
                }
            }
            return res;
        }

        // Mappable interface
        public Matrix Map(Func<double, double> f, Matrix res = null)
        {
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = f(this[r, c]);
                }
            }
            return res;
        }
        public Matrix Map(Func<double, double, double> f, Matrix m2, Matrix res = null)
        {
            ValidateDimensions(this, m2, "Matrix.Map(Func<double,double,double> f, Matrix m2, Matrix res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = f(this[r, c], m2[r, c]);
                }
            }
            return res;
        }

        // Operator overloads
        public static Matrix operator -(Matrix m1)
        {
            return m1.Negate();
        }
        public static Matrix operator *(Matrix m1, double d)
        {
            return m1.Multiply(d);
        }
        public static Matrix operator *(double d, Matrix m1)
        {
            return m1.Multiply(d);
        }
        public static Matrix operator /(Matrix m1, double d)
        {
            return m1.Divide(d);
        }
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            return m1.Subtract(m2);
        }
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            return m1.Add(m2);
        }
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return m1.Multiply(m2);
        }

        // Rearrange the Matrix as a whole
        public Matrix Transpose(Matrix res = null)
        {
            if (res == null)
            {
                res = new Matrix(this.Cols, this.Rows);
            }
            else
            {
                ValidateDimensions(res, this.Cols, this.Rows, "Transpose(Matrix res = null)", "res");
            }

            if (res == this)
            {
                return InPlaceTranspose();
            }

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[c, r] = this[r, c];
                }
            }
            return res;
        }
        public Matrix InPlaceTranspose()
        {
            return this.InPlaceTransform(TRANSPOSE, true);
            //TODO implement this. PROPERLY :P
            //return this;
        }
        public Matrix Transform(CoordTransform newCoords, Matrix res = null, bool everyCycleOfLength2 = false)
        {
            if (res == this)
                return InPlaceTransform(newCoords, everyCycleOfLength2);
            MakeOrValidate(newCoords.DimF(Rows, Cols), ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[newCoords.CoordF(this, r, c)] = this[r, c];
                }
            }
            return res;
        }
        // newCoords MUST permute the indices
        public Matrix InPlaceTransform(CoordTransform newCoords, bool everyCycleOfLength2 = false)
        {
            ValidateDimensions(this, newCoords.DimF(Rows, Cols), "InPlaceTransform(CoordTransform newCoords, bool everyCycleOfLength2 = false)", "this");
            if (!everyCycleOfLength2)
            {
                bool[,] completed = new bool[Rows, Cols]; // has to search through this. *HIGHLY* inefficient. FIX.
                //double[] cycleV = new double[Rows * Cols];
                Pair<int>[] cycleC = new Pair<int>[Rows * Cols];
                int cycleBegin = 0;
                int pos = 0;
                cycleC[0] = new Pair<int>(0, 0);
                while (pos < cycleC.Length)
                {
                    Pair<int> nextCoords = newCoords.CoordF(this, cycleC[pos].x, cycleC[pos].y);
                    if (cycleC[cycleBegin].x == nextCoords.x && cycleC[cycleBegin].y == nextCoords.y)
                    {
                        // Cycle through
                        double temp1 = this[cycleC[pos]];
                        double temp2;
                        for (int i = cycleBegin; i <= pos; i++)
                        {
                            Pair<int> cs = cycleC[i];
                            temp2 = this[cs];
                            this[cs] = temp1;
                            temp1 = temp2;
                            completed[cs.x, cs.y] = true; // mark transformed
                        }
                        int r = 0, c = 0;
                        for (r = 0; r < Rows && completed[r, c]; r++)
                            for (c = 0; c < Cols && completed[r, c]; c++) ; // Loop through until we find something not completed

                        if (pos == cycleC.Length - 1)
                        {
                            break;
                        }
                        cycleBegin = pos++;
                        cycleC[cycleBegin] = new Pair<int>(r, c);
                    }
                    else
                    {
                        cycleC[++pos] = nextCoords;
                    }
                }
            }
            else
            {
                bool[,] completed = new bool[Rows, Cols];
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Cols; c++)
                    {
                        if (!completed[r, c])
                        {
                            Pair<int> otCoords = newCoords.CoordF(this, r, c);
                            double temp = this[otCoords];
                            this[otCoords] = this[r, c];
                            this[r, c] = temp;
                            completed[r, c] = true; // not strictly necessary, since we're about to move past it.
                            completed[otCoords.x, otCoords.y] = true;
                        }
                    }
                }
            }
            return this;
        }

        public enum ReflectDirection
        {
            HORIZONTAL, VERTICAL, CENTRAL
        }
        public Matrix Reflect(ReflectDirection rd, Matrix res = null)
        {
            //TODO implement PROPERLY. :P
            return Transform(GetTransform(rd), res, true);
        }

        //DEPRECATED DO NOT USE.
        public Matrix ReflectHorizontal(Matrix res = null)
        {
            MakeOrValidate(Rows, Cols, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[r, c] = this[Rows - r - 1, c];
                }
            }
            return res;
        }
        public Matrix ReflectVertical(Matrix res = null)
        {
            MakeOrValidate(Rows, Cols, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[r, c] = this[r, Cols - c - 1];
                }
            }
            return res;
        }
        public Matrix ReflectCentral(Matrix res = null)
        {
            MakeOrValidate(Rows, Cols, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[r, c] = this[Rows - r - 1, Cols - c - 1];
                }
            }
            return res;
        }

        // CoordTransformFs
        public readonly CoordTransform
            TRANSPOSE = new CoordTransform(
                (m, r, c) => new Pair<int>(c, r),
                (r, c) => new Pair<int>(c, r)),
            REFLECT_HORIZONTAL = new CoordTransform(
                (m, r, c) => new Pair<int>(r, m.Cols - c - 1),
                (r, c) => new Pair<int>(r, c)),
            REFLECT_VERTICAL = new CoordTransform(
                (m, r, c) => new Pair<int>(m.Rows - r - 1, c),
                (r, c) => new Pair<int>(r, c)),
            REFLECT_CENTRAL = new CoordTransform(
                (m, r, c) => new Pair<int>(m.Rows - r - 1, m.Cols - c - 1),
                (r, c) => new Pair<int>(r, c)),
            IDENTITY = new CoordTransform(
                (m, r, c) => new Pair<int>(r, c),
                (r, c) => new Pair<int>(r, c));

        public CoordTransform GetTransform(ReflectDirection r)
        {
            switch (r)
            {
                case ReflectDirection.HORIZONTAL:
                    return REFLECT_HORIZONTAL;
                case ReflectDirection.VERTICAL:
                    return REFLECT_VERTICAL;
                case ReflectDirection.CENTRAL:
                    return REFLECT_CENTRAL;
            }
            return IDENTITY;
        }

        // Row/column operations **WILL CHANGE THE CURRENT MATRIX**
        public Matrix SwapRows(int r1, int r2)
        {
            double temp = 0;
            for (int c = 0; c < Cols; c++)
            {
                temp = this[r1, c];
                this[r1, c] = this[r2, c];
                this[r2, c] = temp;
            }
            return this;
        }
        public Matrix ScaleRow(int i, double d)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[i, c] *= d;
            }
            return this;
        }
        public Matrix AddRows(int source, int dest)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] += this[source, c];
            }
            return this;
        }
        // Scales source by a and adds to dest
        public Matrix ScaleAndAdd(int source, double a, int dest)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] += a * this[source, c];
            }
            return this;
        }
        public Matrix LCombRows(int source, double a, int dest, double b)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] = b * this[dest, c] + a * this[source, c];
            }
            return this;
        }

        // Echelon Form stuff currently no in-place version of MakeEchelonForm...
        public Matrix MakeEchelonForm()
        {
            Pair<int>[] rows = new Pair<int>[Rows];
            for (int r = 0; r < Rows; r++)
            {
                int lc = 0;
                while (this[r, lc] == 0) lc++;
                rows[r] = new Pair<int>(r, lc);
            }

            rows.OrderBy(x => x.y);

            Matrix ans = new Matrix(this.Rows, this.Cols);

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    ans[r, c] = this[rows[r].x, c];
                }
            }

            return ans;
        }
        // RREF will be in place. MAKE A COPY IF YOU NEED ONE.
        public Matrix RREF()
        {
            //Matrix ans = MakeEchelonForm();

            // Loop vars
            int curRow = 0, nextRow = 0, rowStart = Cols + 1, lrstart = -1;

            while (curRow < Rows) // might repeat at most Rows times
            {
                // Search rows for next longest
                for (int r = curRow; r < Rows; r++)
                {
                    int lc = lrstart + 1; // first nonzero pos has to be further than lrstart
                    while (lc < Cols && this[r, lc] == 0) lc++; // Find first nonzero pos in row

                    if (lc == lrstart + 1) // can't be any longer, break
                    {
                        rowStart = lc;
                        nextRow = r;
                        break;
                    }

                    if (lc < rowStart) // longer than next longest, switch reference.
                    {
                        rowStart = lc;
                        nextRow = r;
                    }
                }

                if (rowStart == Cols) // all the rest of the rows are all zero, so we're done
                {
                    break;
                }

                // Move to top
                this.SwapRows(curRow, nextRow);
                // Scale leading term to 1
                this.ScaleRow(curRow, 1 / this[curRow, rowStart]);

                // Cancel with all other rows
                for (int r = 0; r < Rows; r++)
                {
                    if (r != curRow)
                    {
                        this.ScaleAndAdd(curRow, -this[r, rowStart], r);
                    }
                }

                lrstart = rowStart; // last row start is now rowStart
                curRow++; // move on to next row
            }
            return this;
        }

        // Validation
        private static Matrix MakeOrValidate(int r, int c, ref Matrix m)
        {
            if (m == null)
            {
                m = new Matrix(r, c);
            }
            else
            {
                ValidateDimensions(m, r, c, "Matrix.MakeOrValidate(int r, int c, ref Matrix m)", "m");
            }
            return m;
        }
        private static Matrix MakeOrValidate(Pair<int> dims, ref Matrix m)
        {
            return MakeOrValidate(dims.x, dims.y, ref m);
        }
        private static void ValidateDimensions(Matrix m1, Matrix m2, string method, string pName1, string pName2)
        {
            if ((m1.Cols != m2.Cols) || (m1.Rows != m2.Rows))
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: {1}({2},{3}) {4}({5},{6})", method, pName1, m1.Rows, m1.Cols, pName2, m2.Rows, m2.Cols));
            }
        }
        private static void ValidateDimensions(Matrix m1, int r, int c, string method, string pName1)
        {
            if ((m1.Cols != c) || (m1.Rows != r))
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: expected(({1},{2}) given {3}({4},{5})", method, r, c, pName1, m1.Rows, m1.Cols));
            }
        }
        private static void ValidateDimensions(Matrix m1, Pair<int> dims, string method, string pName1)
        {
            ValidateDimensions(m1, dims.x, dims.y, method, pName1);
        }

        // Construct other matrices
        public Matrix SubMatrix(bool copy = false, int offC = 0, int offR = 0, int rows = -1, int cols = -1)
        {
            return new Matrix(this, copy, offC, offR, rows, cols);
        }
        public static Matrix Identity(int n)
        {
            Matrix ans = new Matrix(n, n);
            for (int i = 0; i < n; i++)
            {
                ans[i, i] = 1;
            }
            return ans;
        }

        // Calculate the determinant
        private double RecalcDeterminant()
        {
            determinant = 0;
            if (Rows != Cols)
            {
                detValid = true;
            }
            else
            {
                if (Rows == 1)
                {
                    return this[0, 0];
                }
                if (Rows == 2)
                {
                    return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
                }
                int c = 0;
                Matrix m = SubMatrix(true, 1, 1, -1, -1);
                for (; c < Cols; c++)
                {
                    determinant += this[0, c] * (1 - 2 * (c % 2)) * m.Determinant;
                    for (int r = 1; r < Rows && c < (Cols - 1); r++)
                    {
                        m[r - 1, c] = this[r, c];
                    }
                }
            }
            return determinant;
        }

        // ToString override
        public override string ToString()
        {
            StringBuilder ans = new StringBuilder();
            ans.AppendLine("[");
            for (int r = 0; r < Rows; r++)
            {
                ans.Append("[ ");
                for (int c = 0; c < Cols - 1; c++)
                {
                    ans.AppendFormat("{0}, ", this[r, c]);
                }
                ans.AppendFormat("{0} ]", this[r, Cols - 1]);
                ans.AppendLine();
            }
            ans.Append("]");
            return ans.ToString();
        }

        // Misc. properties and the indexer
        public double Determinant
        {
            get
            {
                if (detValid)
                {
                    return determinant;
                }
                return RecalcDeterminant();
            }
        }

        public double this[Pair<int> coords]
        {
            get
            {
                return this[coords.x, coords.y];
            }
            set
            {
                this[coords.x, coords.y] = value;
            }
        }

        public double this[int row, int col]
        {
            get
            {
                if (offsets)
                {
                    if (row < Rows && col < Cols)
                        return mat[row + offC, col + offR];
                    throw new ArgumentOutOfRangeException();
                }
                return mat[row, col];
            }
            set
            {
                detValid = false;
                if (offsets)
                {
                    if (row < Rows && col < Cols)
                        mat[row + offC, col + offR] = value;
                    throw new ArgumentOutOfRangeException();
                }
                mat[row, col] = value;
            }
        }

        public Vector<double>[] AsRows
        {
            get
            {
                Vector<double>[] rows = new Vector<double>[Rows];
                for (int r = 0; r < Rows; r++)
                {
                    rows[r] = new Vector<double>(Cols);
                    for (int c = 0; c < Cols; c++)
                    {
                        rows[r][c] = this[r, c];
                    }
                }
                return rows;
            }
        }
        public Vector<double>[] AsCols
        {
            get
            {
                Vector<double>[] cols = new Vector<double>[Cols];
                for (int c = 0; c < Cols; c++)
                {
                    cols[c] = new Vector<double>(Rows);
                    for (int r = 0; r < Rows; r++)
                    {
                        cols[c][r] = this[r, c];
                    }
                }
                return cols;
            }
        }
    }
}
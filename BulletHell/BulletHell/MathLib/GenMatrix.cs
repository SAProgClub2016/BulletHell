using System;
using System.Net;
using System.Text;
using System.Linq;

namespace BulletHell.MathLib
{
    public class Matrix<T>
    {
        // Data
        private T[,] mat;

        // Offsets
        private int offC, offR;
        private bool offsets;

        // Determinant stuff
        private T determinant;
        private bool detValid;

        // Dimensions
        public int Rows { get; protected set; }
        public int Cols { get; protected set; }
        public Pair<int> Dimensions
        {
            get
            {
                return new Pair<int>(Rows, Cols);
            }
        }

        // Constructors
        public Matrix(int rows, int cols)
        {
            mat = new T[rows,cols];
            determinant = default(T);
            offC = offR = 0;
            offsets = false;
            detValid = true;
            Rows=rows;
            Cols=cols;
        }
        public Matrix(Pair<int> dims)
            : this(dims.x, dims.y)
        {
        }
        public Matrix(int sqSize)
            : this(sqSize, sqSize)
        {
        }
        public Matrix(Matrix<T> m, bool copy = true, int offR = 0, int offC = 0, int rows = -1, int cols = -1)
            : this(m.mat, copy, offR, offC, rows, cols)
        {
        }
        public Matrix(T[,] m, bool copy = true, int offR = 0, int offC = 0, int rows = -1, int cols = -1)
        {
            if (!copy)
            {
                mat = m;
                this.offC = offC;
                this.offR = offR;
                
                if(rows > 0)
                {
                    Rows = rows;
                    if(rows>m.GetLength(0)-offR)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Rows = m.GetLength(0) - offR;
                }
                if(cols > 0)
                {
                    Cols = cols;
                    if(cols>m.GetLength(1)-offC)
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
                
                if(rows > 0)
                {
                    Rows = rows;
                    if(rows>m.GetLength(0)-offR)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Rows = m.GetLength(0) - offR;
                }
                if(cols > 0)
                {
                    Cols = cols;
                    if(cols>m.GetLength(1)-offC)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Cols = m.GetLength(1) - offC;
                }
                mat = new T[Rows,Cols];
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Cols; c++)
                    {
                        mat[r, c] = m[r + offC, c + offR];
                    }
                }
            }
            detValid = false;
            determinant = default(T);
        }

        // Nice operations
        public Matrix<T> Add(Matrix<T> m2, Matrix<T> res = null)
        {
            ValidateDimensions(this, m2, "Matrix<T>.Add(Matrix<T> m2, Matrix<T> res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = (dynamic)this[r, c] + m2[r, c];
                }
            }
            return res;
        }
        public Matrix<T> Subtract(Matrix<T> m2, Matrix<T> res = null)
        {
            ValidateDimensions(this, m2, "Matrix<T>.Sub(Matrix<T> m2, Matrix<T> res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = (dynamic)this[r, c] - m2[r, c];
                }
            }
            return res;
        }
        public Matrix<T> Negate(Matrix<T> res = null)
        {
            MakeOrValidate(this.Rows,this.Cols,ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = -(dynamic)this[r, c];
                }
            }
            return res;
        }
        public Matrix<T> Multiply(T d, Matrix<T> res = null)
        {
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = d * (dynamic)this[r, c];
                }
            }
            return res;
        }
        public Matrix<T> Divide(T d, Matrix<T> res = null)
        {
            return Multiply(1 / (dynamic)d, res);
        }
        public Matrix<T> LComb(T a, Matrix<T> m2, T b, Matrix<T> res = null)
        {
            ValidateDimensions(this, m2, "Matrix<T>.LComb(T a, Matrix<T> m2, T b, Matrix<T> res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = (dynamic)a * this[r, c] + (dynamic)b * m2[r, c];
                }
            }
            return res;
        }
        public Matrix<T> Multiply(Matrix<T> m2, Matrix<T> res = null)
        {
            if(Cols!=m2.Rows)
            {
                throw new InvalidOperationException(string.Format("Error - Matrix<T>.Multiply(Matrix<T> m2, Matrix<T> res = null) - Dimension Mismatch - this.Cols [{0}] != m2.Rows [{1}]", Cols, m2.Rows));
            }
            if (res == this || res == m2)
            {
                throw new InvalidOperationException("Error - Matrix<T>.Multiply(Matrix<T> m2, Matrix<T> res = null) - Cannot multiply in place");
            }
            MakeOrValidate(Rows, m2.Cols,ref res);
            for(int r=0;r<Rows;r++)
            {
                for(int c=0;c<m2.Cols;c++)
                {
                    T sum=default(T);
                    for(int i=0;i<Cols;i++)
                    {
                        sum += (dynamic)this[r, i] * m2[i, c];
                    }
                    res[r,c]=sum;
                }
            }
            return res;
        }

        public Matrix<S> Map<S>(Func<T,S> f, Matrix<S> res = null)
        {
            MakeOrValidate(this.Dimensions, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[r, c] = f(this[r, c]);
                }
            }
            return res;
        }

        public Matrix<Q> Map<S, Q>(Func<T, S, Q> f, Matrix<S> o, Matrix<Q> res = null)
        {
            ValidateDimensions(o, this.Dimensions, "Map<S,Q>", "o");
            MakeOrValidate(this.Dimensions, ref res);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    res[r, c] = f(this[r, c],o[r,c]);
                }
            }
            return res;
        }

        // Mappable interface
        /*
        public Matrix<T> Map(Func<T,T> f, Matrix<T> res = null)
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
        public Matrix<T> Map(Func<T,T,T> f, Matrix<T> m2, Matrix<T> res = null)
        {
            ValidateDimensions(this, m2, "Matrix<T>.Map(Func<T,T,T> f, Matrix<T> m2, Matrix<T> res = null)", "this", "m2");
            MakeOrValidate(this.Rows, this.Cols, ref res);
            for (int r = 0; r < res.Rows; r++)
            {
                for (int c = 0; c < res.Cols; c++)
                {
                    res[r, c] = f(this[r, c],m2[r,c]);
                }
            }
            return res;
        }*/
        
        // Operator overloads
        public static Matrix<T> operator -(Matrix<T> m1)
        {
            return m1.Negate();
        }
        public static Matrix<T> operator *(Matrix<T> m1, T d)
        {
            return m1.Multiply(d);
        }
        public static Matrix<T> operator *(T d, Matrix<T> m1)
        {
            return m1.Multiply(d);
        }
        public static Matrix<T> operator /(Matrix<T> m1, T d)
        {
            return m1.Divide(d);
        }
        public static Matrix<T> operator -(Matrix<T> m1, Matrix<T> m2)
        {
            return m1.Subtract(m2);
        }
        public static Matrix<T> operator +(Matrix<T> m1, Matrix<T> m2)
        {
            return m1.Add(m2);
        }
        public static Matrix<T> operator *(Matrix<T> m1, Matrix<T> m2)
        {
            return m1.Multiply(m2);
        }

        // Rearrange the Matrix<T> as a whole
        public Matrix<T> Transpose(Matrix<T> res = null)
        {
            if (res == null)
            {
                res = new Matrix<T>(this.Cols, this.Rows);
            }
            else
            {
                ValidateDimensions(res, this.Cols, this.Rows, "Transpose(Matrix<T> res = null)", "res");
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
        public Matrix<T> InPlaceTranspose()
        {
            return this.InPlaceTransform(TRANSPOSE, true);
            //TODO implement this. PROPERLY :P
            //return this;
        }
        public Matrix<T> Transform(CoordTransform<T> newCoords, Matrix<T> res = null, bool everyCycleOfLength2=false)
        {
            if(res==this)
                return InPlaceTransform(newCoords,everyCycleOfLength2);
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
        public Matrix<T> InPlaceTransform(CoordTransform<T> newCoords, bool everyCycleOfLength2 = false)
        {
            ValidateDimensions(this,newCoords.DimF(Rows,Cols),"InPlaceTransform(CoordTransform<T> newCoords, bool everyCycleOfLength2 = false)","this");
            if (!everyCycleOfLength2)
            {
                bool[,] completed = new bool[Rows, Cols]; // has to search through this. *HIGHLY* inefficient. FIX.
                //T[] cycleV = new T[Rows * Cols];
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
                        T temp1 = this[cycleC[pos]];
                        T temp2;
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
                            T temp = this[otCoords];
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
            HORIZONTAL,VERTICAL,CENTRAL
        }
        public Matrix<T> Reflect(ReflectDirection rd, Matrix<T> res = null)
        {
            //TODO implement PROPERLY. :P
            return Transform(GetTransform(rd), res, true);
        }

        //DEPRECATED DO NOT USE.
        public Matrix<T> ReflectHorizontal(Matrix<T> res = null)
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
        public Matrix<T> ReflectVertical(Matrix<T> res = null)
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
        public Matrix<T> ReflectCentral(Matrix<T> res = null)
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

        // CoordTransform<T>Fs
        public readonly CoordTransform<T>
            TRANSPOSE = new CoordTransform<T>(
                (m, r, c) => new Pair<int>(c, r),
                (r,c)=>new Pair<int>(c,r)),
            REFLECT_HORIZONTAL = new CoordTransform<T>(
                (m, r, c) => new Pair<int>(r, m.Cols - c - 1),
                (r,c)=>new Pair<int>(r,c)),
            REFLECT_VERTICAL = new CoordTransform<T>(
                (m, r, c) => new Pair<int>(m.Rows - r - 1, c),
                (r,c)=>new Pair<int>(r,c)),
            REFLECT_CENTRAL = new CoordTransform<T>(
                (m, r, c) => new Pair<int>(m.Rows - r - 1, m.Cols - c - 1),
                (r,c)=>new Pair<int>(r,c)),
            IDENTITY = new CoordTransform<T>(
                (m, r, c) => new Pair<int>(r,c),
                (r,c)=>new Pair<int>(r,c));

        public CoordTransform<T> GetTransform(ReflectDirection r)
        {
            switch(r)
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

        // Row/column operations **WILL CHANGE THE CURRENT Matrix<T>**
        public Matrix<T> SwapRows(int r1, int r2)
        {
            T temp;
            for (int c = 0; c < Cols; c++)
            {
                temp = this[r1, c];
                this[r1, c] = this[r2, c];
                this[r2, c] = temp;
            }
            return this;
        }
        public Matrix<T> ScaleRow(int i, T d)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[i, c] *= (dynamic)d;
            }
            return this;
        }
        public Matrix<T> AddRows(int source, int dest)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] += (dynamic)this[source, c];
            }
            return this;
        }
        // Scales source by a and adds to dest
        public Matrix<T> ScaleAndAdd(int source, T a, int dest)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] += (dynamic)a*this[source, c];
            }
            return this;
        }
        public Matrix<T> LCombRows(int source, T a, int dest, T b)
        {
            for (int c = 0; c < Cols; c++)
            {
                this[dest, c] = (dynamic)b*this[dest,c]+(dynamic)a*this[source, c];
            }
            return this;
        }

        // Echelon Form stuff currently no in-place version of MakeEchelonForm...
        public Matrix<T> MakeEchelonForm()
        {
            Pair<int>[] rows = new Pair<int>[Rows];
            for (int r = 0; r < Rows; r++)
            {
                int lc = 0;
                while ((dynamic)this[r, lc] == default(T)) lc++;
                rows[r] = new Pair<int>(r,lc);
            }

            rows.OrderBy(x => x.y);

            Matrix<T> ans = new Matrix<T>(this.Rows, this.Cols);

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
        public Matrix<T> RREF()
        {
            //Matrix<T> ans = MakeEchelonForm();
            
            // Loop vars
            int curRow=0, nextRow=0, rowStart = Cols+1, lrstart=-1;

            while (curRow < Rows) // might repeat at most Rows times
            {
                // Search rows for next longest
                for (int r = curRow; r < Rows; r++)
                {
                    int lc = lrstart+1; // first nonzero pos has to be further than lrstart
                    while (lc < Cols && this[r, lc] == (dynamic)default(T)) lc++; // Find first nonzero pos in row

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
                this.ScaleRow(curRow, 1 / (dynamic)this[curRow, rowStart]);

                // Cancel with all other rows
                for (int r = 0; r < Rows; r++)
                {
                    if (r != curRow)
                    {
                        this.ScaleAndAdd(curRow, -(dynamic)this[r, rowStart], r);
                    }
                }

                lrstart = rowStart; // last row start is now rowStart
                curRow++; // move on to next row
            }
            return this;
        }

        // Validation
        private static Matrix<S> MakeOrValidate<S>(int r, int c, ref Matrix<S> m)
        {
            if (m == null)
            {
                m = new Matrix<S>(r, c);
            }
            else
            {
                ValidateDimensions(m, r, c, "Matrix<T>.MakeOrValidate(int r, int c, ref Matrix<S> m)", "m");
            }
            return m;
        }
        private static Matrix<S> MakeOrValidate<S>(Pair<int> dims, ref Matrix<S> m)
        {
            return MakeOrValidate(dims.x, dims.y, ref m);
        }
        private static void ValidateDimensions<Q,S>(Matrix<Q> m1, Matrix<S> m2, string method, string pName1, string pName2)
        {
            if ((m1.Cols != m2.Cols) || (m1.Rows != m2.Rows))
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: {1}({2},{3}) {4}({5},{6})", method, pName1, m1.Rows, m1.Cols, pName2, m2.Rows, m2.Cols));
            }
        }
        private static void ValidateDimensions<S>(Matrix<S> m1, int r, int c, string method, string pName1)
        {
            if ((m1.Cols != c) || (m1.Rows != r))
            {
                throw new InvalidOperationException(string.Format("Error - {0} - Dimension mismatch: expected(({1},{2}) given {3}({4},{5})", method, r, c, pName1, m1.Rows, m1.Cols));
            }
        }
        private static void ValidateDimensions<S>(Matrix<S> m1, Pair<int> dims, string method, string pName1)
        {
            ValidateDimensions(m1, dims.x, dims.y, method, pName1);
        }
        
        // Construct other matrices
        public Matrix<T> SubMatrix(bool copy = false, int offC = 0, int offR = 0, int rows = -1, int cols = -1)
        {
            return new Matrix<T>(this, copy, offC, offR, rows, cols);
        }
        public static Matrix<S> Identity<S>(int n)
        {
            Matrix<S> ans = new Matrix<S>(n, n);
            for (int i = 0; i < n; i++)
            {
                ans[i, i] = (dynamic)1;
            }
            return ans;
        }
        
        // Calculate the determinant
        private T RecalcDeterminant()
        {
            determinant = default(T);
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
                    return (dynamic)this[0, 0] * this[1, 1] - (dynamic)this[0, 1] * this[1, 0];
                }
                int c = 0;
                Matrix<T> m = SubMatrix(true,1,1,-1,-1);
                for(;c<Cols;c++)
                {
                    determinant += (dynamic)this[0, c] * (1 - 2 * (c % 2)) * m.Determinant;
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
            StringBuilder ans=new StringBuilder();
            ans.AppendLine("[");
            for (int r = 0; r < Rows; r++)
            {
                ans.Append("[ ");
                for (int c = 0; c < Cols - 1; c++)
                {
                    ans.AppendFormat("{0}, ", this[r, c]);
                }
                ans.AppendFormat("{0} ]", this[r,Cols-1]);
                ans.AppendLine();
            }
            ans.Append("]");
            return ans.ToString();
        }

        // Misc. properties and the indexer
        public T Determinant
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

        public T this[Pair<int> coords]
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

        public T this[int row,int col]
        {
            get
            {
                if (offsets)
                {
                    if(row<Rows && col<Cols)
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
        
        public Vector<T>[] AsRows
        {
            get
            {
                Vector<T>[] rows = new Vector<T>[Rows];
                for (int r = 0; r < Rows; r++)
                {
                    rows[r] = new Vector<T>(Cols);
                    for (int c = 0; c < Cols; c++)
                    {
                        rows[r][c] = this[r, c];
                    }
                }
                return rows;
            }
        }
        public Vector<T>[] AsCols
        {
            get
            {
                Vector<T>[] cols = new Vector<T>[Cols];
                for(int c = 0; c<Cols;c++)
                {
                    cols[c]=new Vector<T>(Rows);
                    for(int r = 0; r < Rows; r++)
                    {
                        cols[c][r]=this[r,c];
                    }
                }
                return cols;
            }
        }
    }
}
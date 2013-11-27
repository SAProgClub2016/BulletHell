#define USING_EXPRESSIONS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.MathLib.Function
{
    public class PolyFunc<TC, TP> : IntegrableFunction<TP, TC>
    {
        Vector<TC> coeffs;
        PolyFunc<TC, TP> integral, derivative;

        public int Degree
        {
            get
            {
                return coeffs.Dimension - 1;
            }
        }

        public override Func<TP, TC> F
        {
            get
            {
                return this.Evaluate;
            }
        }

        public override Func<TP, TC> FI
        {
            get
            {
                return this.Integral.F;
            }
        }

        public override IntegrableFunction<TP, TC> StrongInt
        {
            get
            {
                return this.Integral;
            }
        }

        public PolyFunc<TC, TP> Derivative
        {
            get
            {
                if (derivative == null)
                {
                    if (Degree <= 0)
                    {
                        derivative = new PolyFunc<TC, TP>();
                    }
                    else
                    {
                        TC[] dCos = new TC[Degree];
                        for (int i = 1; i < coeffs.Dimension; i++)
                        {
                            dCos[i - 1] = (dynamic)i * coeffs[i]; // no choice since i must be int here.
                        }
                        derivative = new PolyFunc<TC, TP>(dCos);
                    }
                }
                return derivative;
            }
        }

        public PolyFunc<TC, TP> Integral
        {
            get
            {
                if (integral == null)
                {
                    if (Degree < 0)
                    {
                        integral = new PolyFunc<TC, TP>();
                    }
                    else
                    {
                        TC[] iCos = new TC[coeffs.Dimension + 1];
                        for (int i = 0; i < coeffs.Dimension; i++)
                        {
                            iCos[i + 1] = (dynamic)coeffs[i] / (i + 1); // no choice since i must be int here.
                        }
                        integral = new PolyFunc<TC, TP>(iCos);
                    }
                }
                return integral;
            }
        }

        public PolyFunc(params TC[] cos)
        {
            int count = 0;
            while (cos.Length - 1 - count >= 0 && Utils.IsZero((dynamic)cos[cos.Length - 1 - count])) count++;
            TC[] ans;
            if (count == 0)
            {
                ans = cos;
            }
            else
            {
                ans = new TC[cos.Length - count];
                for (int i = 0; i < ans.Length; i++)
                {
                    ans[i] = cos[i];
                }
            }
            coeffs = new Vector<TC>(ans);
        }

        public PolyFunc(Vector<TC> cos)
            : this(cos.AsArray)
        {
        }

        public TC Evaluate(TP t)
        {
            TC sum = default(TC);
            if (coeffs.Dimension == 0)
                return sum;
            for (int i = coeffs.Dimension - 1; i > 0; i--)
            {
#if(USING_EXPRESSIONS)
                sum = VectorScalar<TC,TP>.ScalarRight(Operations<TC>.Add(sum, coeffs[i]),t);
#else
                sum = (dynamic)t * ((dynamic)sum + coeffs[i]);
#endif
            }
#if(USING_EXPRESSIONS)
            sum = Operations<TC>.Add(sum,coeffs[0]);
#else
            sum += (dynamic)coeffs[0];
#endif
            return sum;
        }

        public TC this[int i]
        {
            get
            {
                return coeffs[i];
            }
            set
            {
                if (i >= coeffs.Dimension)
                {
                    coeffs = coeffs.MakeDim(i + 1);
                }
                coeffs[i] = value;
                integral = null; derivative = null;
            }
        }

        public static PolyFunc<TC, TP> operator +(PolyFunc<TC, TP> t1, PolyFunc<TC, TP> t2)
        {
            Vector<TC> c1 = t1.coeffs, c2 = t2.coeffs;
            if (c1.Dimension < c2.Dimension)
                c1 = c1.MakeDim(c2.Dimension);
            if (c1.Dimension > c2.Dimension)
                c2 = c2.MakeDim(c1.Dimension);
            return new PolyFunc<TC, TP>(c1 + c2);
        }

        public PolyFunc<TC, TP> Multiply(TP t2)
        {
            Vector<TC> ans = new Vector<TC>(coeffs.Dimension);
            for (int i = 0; i < coeffs.Dimension; i++)
            {
                ans[i] = VectorScalar<TC, TP>.ScalarRight(coeffs[i], t2);
            }

            return new PolyFunc<TC, TP>(ans);
        }

        public static PolyFunc<TC, TP> operator *(PolyFunc<TC, TP> t1, TP t2)
        {
            return t1.Multiply(t2);
        }

        public static PolyFunc<TC, TP> operator *(TP t2, PolyFunc<TC, TP> t1)
        {
            return t1.Multiply(t2);
        }

        public static PolyFunc<TC, TP> operator -(PolyFunc<TC, TP> t)
        {
            return new PolyFunc<TC, TP>(-t.coeffs);
        }

        public static PolyFunc<TC, TP> operator -(PolyFunc<TC, TP> t1, PolyFunc<TC, TP> t2)
        {
            return t1 + (-t2);
        }

        public static implicit operator PolyFunc<TC, TP>(TC t)
        {
            return new PolyFunc<TC, TP>(t);
        }
        public override string ToString()
        {
            StringBuilder ans = new StringBuilder();
            for (int i = 0; i < coeffs.Dimension; i++)
            {
                ans.Append(string.Format("{0}*x^{1} ", coeffs[i].ToString(), i));
                if (i != coeffs.Dimension - 1)
                    ans.Append("+ ");
            }
            return ans.ToString();
        }
    }
}

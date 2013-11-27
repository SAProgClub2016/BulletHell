using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib.Function;

namespace BulletHell.MathLib
{
    class FunctionTest
    {
        public static void Main()
        {
            PolyFunc<double,double> l1 = new PolyFunc<double,double>(1);
            PolyFunc<double,double> l2 = new PolyFunc<double,double>(2);
            IntegrableFunction<double, double> split = IntegrableFunction<double, double>.SplitAt(l1, 2, l2, false);

            for (double d = 0; d < 5; d += 0.1)
            {
                Console.WriteLine("<{0},{1},{2},{3},{4}>", d, l1.F(d), l2.F(d), split.F(d), split.FI(d));
            }
            Console.ReadKey();
        }
    }
}

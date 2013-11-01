using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell
{
    class Utils
    {
        public static S CastTo<R,S>

        public static S CastAndAssign<R, S>(R from, ref S to) where S : class
        {
            to = 
        }
        public static S CastAndAssign<R, S>(R from, ref S to) where S : struct
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell
{
    public class Utils
    {
        public static Func<S, T> MakeClosure<Q, S, T>(Q q, Func<Q, S, T> f)
        {
            return (S s) => f(q,s);
        }
    }
}

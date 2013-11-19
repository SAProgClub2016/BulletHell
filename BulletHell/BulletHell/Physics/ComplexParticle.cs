using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

namespace BulletHell.Physics
{
    class ComplexParticle : Particle
    {
        public void ChangeTrajectory(Func<double, Vector<double>> newPath, double t1, bool seal1 = true, double t2 = -1, bool seal2 = true)
        {

            Func<double, Vector<double>> f1 = (t =>
                {
                    if(t<t1)
                    {
                        return PosFunc(t);
                    }
                    if(t2<t1 || t<t2)
                    {
                        if(seal1)
                        {
                            return newPath(t)-newPath(t1)+PosFunc(t1);
                        }
                        else
                        {
                            return newPath(t);
                        }
                    }
                    if (seal2)
                    {
                        if (seal1)
                        {
                            return PosFunc(t) - PosFunc(t2) + newPath(t2) - newPath(t1) + PosFunc(t1);
                        }
                        else
                        {
                            return PosFunc(t) - PosFunc(t2) + newPath(t2);
                        }
                    }
                    else
                    {
                        return PosFunc(t);
                    }
                });
            PosFunc = f;
        }
    }
}

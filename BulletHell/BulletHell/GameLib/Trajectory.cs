using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell.GameLib
{
    public delegate Particle Trajectory(double t,double x, double y);
    public class TrajectoryFactory
    {
        public static Trajectory SimpleAcc(Pair<double> am, Pair<double> vm, Pair<double> r0 = default(Pair<double>))
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(
                    s =>
                    {
                        double tt = s - t;
                        return x + r0.x + vm.x * tt + am.x * tt * tt * .5;
                    },
                    s =>
                    {
                        double tt = s - t;
                        return y + r0.y + vm.y * tt + am.y * tt * tt * .5;
                    });
                return p;
            };
        }
        public static Trajectory SimpleVel(Pair<double> vm, Pair<double> r0 = default(Pair<double>))
        {
            return SimpleVel(vm.x,vm.y,r0.x,r0.y);
        }
        public static Trajectory SimpleVel(double vx, double vy, double x0=0,double y0=0)
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(s => x + x0 + vx * (s - t), s => y + y0 + vy * (s - t));
                return p;
            };
        }
        public static Trajectory AngleMagVel(double th, double m, double x0=0, double y0=0)
        {
            return SimpleVel(m * Utils.FastCos(th), m * Utils.FastSin(th),x0,y0);
        }
        // Because why not :P
        public static Trajectory SpinningLinearAMVel(double th, double m, double w, double r, double th0=0)
        {
            return SpinningLinearSimpleVel(m * Utils.FastCos(th), m * Utils.FastSin(th),w,r,th0);
        }
        public static Trajectory SpinningLinearSimpleVel(double vx, double vy, double w, double r, double th0=0)
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(s => x + vx * (s - t) + r * Utils.FastCos(th0 + w * (s - t)), s => y + vy * (s - t) + r * Math.Sin(th0 + w * (s - t)));
                return p;
            };
        }
    }
}

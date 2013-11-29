using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.GameLib
{
    public delegate Particle Trajectory(double t,double x, double y);
    public class TrajectoryFactory
    {

        public static Trajectory SimpleVel(double vx, double vy)
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(s => x + vx * (s - t), s => y + vy * (s - t));
                return p;
            };
        }
        public static Trajectory AngleMagVel(double th, double m)
        {
            return SimpleVel(m * Utils.FastCos(th), m * Utils.FastSin(th));
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

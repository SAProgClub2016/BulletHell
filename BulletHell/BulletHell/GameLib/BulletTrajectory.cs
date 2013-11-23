using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.GameLib
{
    public delegate Bullet BulletTrajectory(double t,double x, double y);
    public class BulletTrajectoryFactory
    {

        public static BulletTrajectory SimpleVel(Drawable d, double vx, double vy)
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(s => x + vx * (s - t), s => y + vy * (s - t));
                return new Bullet(p, DrawableFactory.MakeNullTo(d,t));
            };
        }
        public static BulletTrajectory AngleMagVel(Drawable d, double th, double m)
        {
            return SimpleVel(d, m * Math.Cos(th), m * Math.Sin(th));
        }
        // Because why not :P
        public static BulletTrajectory SpinningLinearAMVel(Drawable d, double th, double m, double w, double r, double th0=0)
        {
            return SpinningLinearSimpleVel(d, m * Math.Cos(th), m * Math.Sin(th),w,r,th0);
        }
        public static BulletTrajectory SpinningLinearSimpleVel(Drawable d, double vx, double vy, double w, double r, double th0=0)
        {
            return (t, x, y) =>
            {
                Particle p = new Particle(s => x + vx * (s - t) + r * Math.Cos(th0 + w * (s - t)), s => y + vy * (s - t) + r * Math.Sin(th0 + w * (s - t)));
                return new Bullet(p, DrawableFactory.MakeNullTo(d, t));
            };
        }
    }
}

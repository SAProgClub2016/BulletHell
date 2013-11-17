using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.Game
{
    public delegate Bullet BulletTrajectory(double t,double x, double y);
    public class BulletTrajectoryFactory
    {
        public static BulletTrajectory SimpleVel(GraphicsObject sh, double vx, double vy)
        {
            return (t, x, y) =>
            {
                GraphicsObject o = sh.Clone();
                o.Position = new Particle(s => x + vx * (s - t), s => y + vy * (s - t));
                return new Bullet(o);
            };
        }
        public static BulletTrajectory AngleMagVel(GraphicsObject sh, double th, double m)
        {
            return SimpleVel(sh, m * Math.Cos(th), m * Math.Sin(th));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.Game
{
    public delegate Bullet BulletTrajectory(double t);
    public class BulletTrajectoryFactory
    {
        public static BulletTrajectory SimpleVel(GraphicsObject sh, double vx, double vy)
        {
            return t =>
            {
                GraphicsObject o = sh.Clone();
                o.Position = new Particle(s=>vx*(s-t),s=>vy*(s-t));
                return new Bullet(o);
            };
        }
    }
}

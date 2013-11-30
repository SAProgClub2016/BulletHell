using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.GameLib
{
    public class Bullet : Entity
    {
        public Bullet(double cTime, Particle pos, Drawable d, PhysicsShape ps, PhysicsClass pc, BulletEmitter e = null)
            : base(cTime,pos,d,ps,pc,e)
        {
        }
    }
}

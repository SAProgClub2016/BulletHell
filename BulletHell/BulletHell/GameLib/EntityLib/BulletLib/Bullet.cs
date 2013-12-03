using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;

namespace BulletHell.GameLib.EntityLib.BulletLib
{
    public class Bullet : Entity
    {
        public Bullet(double cTime, Particle pos, Drawable d, PhysicsShape ps, EntityClass pc, BulletEmitter e = null)
            : base(cTime,pos,d,ps,pc,e)
        {
        }
    }
}

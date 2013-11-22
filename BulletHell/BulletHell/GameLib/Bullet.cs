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
        public Bullet(Particle pos, Drawable d)
            : base(pos, d)
        {
        }
        public Bullet(Particle pos, Drawable d, BulletEmitter e)
            : base(pos, d, e)
        {
        }
    }
}

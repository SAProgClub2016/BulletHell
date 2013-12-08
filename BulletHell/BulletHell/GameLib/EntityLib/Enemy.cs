using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
    class Enemy : Entity
    {
        int health;
        public const int DefaultHealth = 100;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public Enemy(double cTime, Particle pos, Drawable d, PhysicsShape ps, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null, int health = 100)
            : base(cTime,pos,d,ps,pc,e,g)
        {
            Health = health;
        }

        public static EntityBuilder MakeEnemy(int h = DefaultHealth)
        {
            return (t, p, d, s, c, e, g) => new Enemy(t, p, d, s, c, e, g, h);
        }
    }
}

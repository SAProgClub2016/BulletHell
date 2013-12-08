using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;

namespace BulletHell.GameLib.EntityLib.BulletLib
{
    public class Bullet : Entity
    {
        int damage;
        public const int DefaultDamage = 20;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public Bullet(double cTime, Particle pos, Drawable d, PhysicsShape ps, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null, int dmg = 20)
            : base(cTime,pos,d,ps,pc,e,g)
        {
            damage = dmg;
        }

        public static EntityBuilder MakeBullet(int dmg = DefaultDamage)
        {
            return (t, p, d, s, c, e, g) => new Bullet(t, p, d, s, c, e, g, dmg);
        }
    }
}

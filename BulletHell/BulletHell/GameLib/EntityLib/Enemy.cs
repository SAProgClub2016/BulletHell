using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.Gfx;
using BulletHell.MathLib;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{

    public delegate Func<Game, IEnumerable<Pickup>> DropFunction(Enemy e);
    public class Enemy : Entity
    {
        private int health;
        private int value;
        public const int DefaultHealth = 100;
        public const int DefaultValue = 100;
        private Func<Game, IEnumerable<Pickup>> drops;
        private DropFunction dropf;
        
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public Enemy(double cTime, Particle pos, Drawable d, PhysicsShape ps, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null, DropFunction df = null, int health = DefaultHealth, int value = DefaultValue)
            : base(cTime,pos,d,ps,pc,e,g)
        {
            Health = health;
            Value = value;
            dropf = df ?? Enemy.DefaultDropFunc;
        }

        public static Func<Game,IEnumerable<Pickup>> DefaultDropFunc(Enemy e)
        {
            return g => new LinkedList<Pickup>();
            /*int val = e.Value;
            int num = g.Random.Next(5, 8);
            val /= num;
            Vector<double> pos = e.Position.CurrentPosition;
            for (int i = 0; i < num; i++)
            {
                Pickup p = entSpawn.Build<Pickup>("Money", e.Time, pos[0], pos[1]);
                p.Effect = Effects.MakeQuantityChanger(new LambdaValue<int>(() => g.Money, (m) => g.Money = m), t => t + val);
                yield return p;
            }*/
        }

        public static EntityBuilder MakeEnemy(DropFunction f = null, int h = DefaultHealth, int v = DefaultValue)
        {
            return (t, p, d, s, c, e, g) => new Enemy(t, p, d, s, c, e, g, f, h, v);
        }

        public Func<Game, IEnumerable<Pickup>> Drop
        {
            get
            {
                if (drops == null)
                    drops = dropf(this);
                return drops;
            }
        }
    }
}

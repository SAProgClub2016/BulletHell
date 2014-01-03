using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.GameLib.LevelLib;
using BulletHell.Gfx;
using BulletHell.MathLib;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BulletHell.TestLevels
{
    public class TestLevel : List<Entity>, Level
    {
        Random rand;
        EntitySpawner entSpawn;

        public TestLevel()
        {
            entSpawn = new EntitySpawner();
            rand = new Random();

            InitializeBulletEntities(entSpawn);
            InitializeSimpleEntities(entSpawn);

            Ellipse entEl = new Ellipse(7);
            Drawable o1 = entEl.MakeDrawable(new GraphicsStyle(Brushes.Green));

            int vx = 1, vx2 = 2;
            int vy = 2, vy2 = 4;

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            Particle p2 = new Particle(Utils.MakeClosure<double, double, double>(this.Width, (w, x) => w - vx2 * x), y => vy2 * y);

            Particle q = new Particle(t => 7 * t + 10 * Utils.FastCos(t), t => 3 * t + 10 * Utils.FastSin(t));

            double FULL = 2 * Math.PI;
            double cd = 1;
            int perCirc = 12;
            int offsets = 6;
            BulletEmission[] bEms = new BulletEmission[offsets], bEms2 = new BulletEmission[offsets];
            double DOWN = Math.PI / 2;
            // Makes the spiral pattern with bullets of shape o2
            Trajectory[][] arrs = new Trajectory[offsets][];
            for (int i = 0; i < offsets; i++)
                arrs[i] = new Trajectory[perCirc];
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = TrajectoryFactory.AngleMagVel((i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 10);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms[i] = new BulletEmission(cd, 0, arrs[i], entSpawn["OrangeRed_5"]);
            // Same as above, but we're gonna change the shape
            // no need to regenerate the spiral
            for (int i = 0; i < offsets; i++)
                bEms2[i] = new BulletEmission(cd, 0, arrs[i], entSpawn["Azure_5"]);
            // Same as above, but we're gonna change the shape again and the path
            BulletEmission[] bEms3 = new BulletEmission[offsets];
            for (int i = 0; i < offsets; i++)
                arrs[i] = new Trajectory[perCirc];
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = TrajectoryFactory.SpinningLinearAMVel((i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 7/*3*/, 0.5, 20);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms3[i] = new BulletEmission(cd, 0, arrs[i], entSpawn["HotPink_5"]);

            EntityClass enemyBullet = new EntityClass("EnemyBullet", "Bullet");
            EntityClass enemy = new EntityClass("Enemy", "Character");

            BulletEmitter em = new BulletEmitter(new BulletPattern(bEms));
            BulletEmitter em2 = new BulletEmitter(new BulletPattern(bEms2));
            BulletEmitter em3 = new BulletEmitter(new BulletPattern(bEms3));

            entSpawn.MakeType("RedSpiral", null, o1, entEl, enemy, em, null, Enemy.MakeEnemy(this.DropFunc));
            
            entSpawn.AddTarget(this, entity => Add(entity));

            Entity e = entSpawn.Build("RedSpiral", 0, p1);
            Entity e2 = entSpawn.Build("RedSpiral", 0, p2);

            Particle p3 = new Particle(x => 0.5 * x + 500, y => 3 * y);
            Entity e3 = entSpawn.Build("RedSpiral", 0, p3);

            Particle p4 = new Particle(x => -0.25 * x + 300, y => 3.5 * y);
            Entity e4 = entSpawn.Build("RedSpiral", 0, p4);

            Particle p5 = new Particle(x => -0.4 * x + 800, y => 2 * y);
            Entity e5 = entSpawn.Build("RedSpiral", 0, p5);

            entSpawn["WhiteSpiral"] = entSpawn["RedSpiral"].ChangeEmitter(em2, true);
            entSpawn["PinkWaves"] = entSpawn["RedSpiral"].ChangeEmitter(em3, true);
            //entSpawn.MakeType("WhiteSpinningSpiral",null, o1, entEl, enemy, em2);

            Entity e6 = entSpawn.Build("WhiteSpiral", 0, q);

            Particle r = new Particle(Utils.MakeClosure<double, double, double>((double)this.Width / 3, (w, t) => w + 3 * t), t => 5 * t);
            Entity e7 = entSpawn.Build("PinkWaves", 0, r);

            entSpawn.RemoveTarget(this);

        }

        public Func<Game, IEnumerable<Pickup>> DropFunc(Enemy e)
        {
            return (Game g) =>
            {
                LinkedList<Pickup> ans = new LinkedList<Pickup>();
                int val = e.Value;
                int num = g.Random.Next(5, 8);
                val /= num;
                Vector<double> pos = e.Position.CurrentPosition;
                for (int i = 0; i < num; i++)
                {
                    Pickup p = entSpawn.Build<Pickup>("Money", e.Time, pos[0], pos[1]);
                    p.Effect = Effects.MakeQuantityChanger(new LambdaValue<int>(() => g.Money, (m) => g.Money = m), t => t + val);
                    ans.AddLast(p);
                }
                return ans;
            };
        }

        private void InitializeBulletEntities(EntitySpawner spawner)
        {
            Ellipse bulletEl = new Ellipse(5);
            EntityType bulletType = new EntityType(null, bulletEl, new GraphicsStyle(Brushes.OrangeRed), new EntityClass("EnemyBullet", "Bullet"), null, Bullet.MakeBullet(20));

            spawner["OrangeRed_5"] = bulletType;
            spawner["Azure_5"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.Azure), true);
            spawner["HotPink_5"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.HotPink), true);
            //spawner["MainCharBullet"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.Violet), true).ChangeClass(new EntityClass("MainCharBullet", "Bullet"));
        }

        private void InitializeSimpleEntities(EntitySpawner entSpawn)
        {
            PhysicsShape moneyShape = new Ellipse(6);
            entSpawn.MakeType("Money", (t, x, y) => TrajectoryFactory.SimpleAcc(new Pair<double>(0, 1), rand.RandomPairInBox(new Pair<double>(-5, -10), new Pair<double>(10, 10)))(t, x, y), moneyShape.MakeDrawable(new GraphicsStyle(Brushes.Gold)), moneyShape, new EntityClass("Pickup", "Pickup"), null, null, Pickup.MakePickup());
        }

        public double Width
        {
            get { return 1280; }
        }

        public double Height
        {
            get { return 720; }
        }

        public IEnumerable<Id> RenderOrder
        {
            get
            {
                LinkedList<Id> ids = new LinkedList<Id>();
                ids.AddLast("Character");
                ids.AddLast("Bullet");
                ids.AddLast("Pickup");
                return ids;
            }
        }
    }
}

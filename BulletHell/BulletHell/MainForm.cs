using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BulletHell.Time;
using BulletHell.Physics;
using BulletHell.Gfx;
using BulletHell.GameLib;
using BulletHell.MathLib;
using BulletHell.Collections;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.GameLib.EventLib;
using BulletHell.Physics.ShapeLib;

namespace BulletHell
{
    public partial class MainForm : Form
    {
        KeyManager keyMan;

        Drawable o1;
        Entity e, e2;

        Entity bg;
        Entity bgneg;

        BufferedGraphics buff;
        Game game;
        bool DisplayFrameRate = true, DisplayTime = true;
        private DefaultValueDictionary<Entity, bool> hitby;
        EntitySpawner entSpawn;

        public MainForm()
        {
            InitializeComponent();
            entSpawn = new EntitySpawner();

            hitby = new DefaultValueDictionary<Entity, bool>(false);

            double border = 40;

            Particle boxC = (Particle)(new Vector<double>((double)ClientRectangle.Width / 2, (double)ClientRectangle.Height / 2));
            Particle boxR = (Particle)(new Vector<double>((double)ClientRectangle.Width / 2+border, (double)ClientRectangle.Height / 2+border));
            Box bgbox = new Box(boxR);
            PartialBox boxNeg = new PartialBox(boxR, new Vector<bool>(true, false),new Vector<bool>(true, true));

            EntityClass bgClass = new EntityClass("Background");

            bg = new Entity(0, boxC, bgbox, new GraphicsStyle(Brushes.Black), bgClass);
            bgneg = new Entity(0, boxC, DrawableFactory.MakeNull(), boxNeg, new EntityClass("BackgroundNegative", "Null"), null);

            keyMan = new KeyManager();

            InitializeKeyManager();

            int vx = 1, vx2 = 2;
            int vy = 2, vy2 = 4;
            InitializeBulletEntities(entSpawn);
            InitializeSimpleEntities(entSpawn);

            Drawable mchar = DrawableFactory.MakeCircle(8, new GraphicsStyle(Brushes.Orange, Pens.Red));

            game = new Game(new MainChar(mchar,entSpawn["MainCharBullet"],ClientRectangle.Width/2,ClientRectangle.Height-20,40));
            InitializePhysicsManager(game.PhysicsManager);
            InitializeRenderManager(game.RenderManager);

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            Particle p2 = new Particle(Utils.MakeClosure<double,double,double>(ClientRectangle.Width,(w,x) => w - vx2 * x), y => vy2 * y);

            Particle q = new Particle(t => 7 * t + 10 * Utils.FastCos(t), t => 3 * t + 10 * Utils.FastSin(t));

            Ellipse entEl = new Ellipse(7);
            o1 = entEl.MakeDrawable(new GraphicsStyle(Brushes.Green));

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
                bEms[i] = new BulletEmission(cd, 0, arrs[i],entSpawn["OrangeRed_5"]);
            // Same as above, but we're gonna change the shape
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
                bEms2[i] = new BulletEmission(cd, 0, arrs[i],entSpawn["Azure_5"]);
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

            EntityClass enemyBullet = new EntityClass("EnemyBullet","Bullet");
            EntityClass enemy = new EntityClass("Enemy","Character");

            BulletEmitter em = new BulletEmitter(new BulletPattern(bEms));
            BulletEmitter em2 = new BulletEmitter(new BulletPattern(bEms2));
            BulletEmitter em3 = new BulletEmitter(new BulletPattern(bEms3));

            entSpawn.MakeType("RedSpiral", null, o1, entEl, enemy, em,null,Enemy.MakeEnemy());

            e = entSpawn.Build("RedSpiral", 0, p1);
            e2 = entSpawn.Build("RedSpiral", 0, p2);

            Particle p3 = new Particle(x => 0.5 * x + 500, y => 3 * y);
            Entity e3  = entSpawn.Build("RedSpiral", 0, p3);

            Particle p4 = new Particle(x => -0.25 * x + 300, y => 3.5 * y);
            Entity e4 = entSpawn.Build("RedSpiral", 0, p4);

            Particle p5 = new Particle(x => -0.4 * x + 800, y => 2 * y);
            Entity e5 = entSpawn.Build("RedSpiral", 0, p5);


            game += bg;
            game = game + e + e2 + e3 + e4 + e5;

            entSpawn["WhiteSpiral"] = entSpawn["RedSpiral"].ChangeEmitter(em2, true);
            entSpawn["PinkWaves"] = entSpawn["RedSpiral"].ChangeEmitter(em3, true);
            //entSpawn.MakeType("WhiteSpinningSpiral",null, o1, entEl, enemy, em2);

            Entity e6 = entSpawn.Build("WhiteSpiral", 0, q);
            game += e6;

            Particle r = new Particle(Utils.MakeClosure<double,double,double>((double)ClientRectangle.Width/3, (w,t)=>w+3*t), t=>5*t);
            Entity e7 = entSpawn.Build("PinkWaves",0, r);
            game += e7;
            game.ResetTime();
            BufferedGraphicsContext c = BufferedGraphicsManager.Current;
            buff = c.Allocate(CreateGraphics(), ClientRectangle);
        }

        private void InitializeSimpleEntities(EntitySpawner entSpawn)
        {
            PhysicsShape moneyShape= new Ellipse(6);
            entSpawn.MakeType("Money", (t, x, y) => TrajectoryFactory.SimpleAcc(new Pair<double>(0, 1), game.Random.RandomPairInBox(new Pair<double>(-5, -10), new Pair<double>(10, 10)))(t,x,y), moneyShape.MakeDrawable(new GraphicsStyle(Brushes.Gold)), moneyShape, new EntityClass("Pickup", "Pickup"), null, null, Pickup.MakePickup());
        }

        private void InitializePhysicsManager(PhysicsManager pman)
        {
            pman.AddCollisionHandler("EnemyBullet", "MainChar", this.CharHit);
            pman.AddCollisionHandler("MainCharBullet", "Enemy", this.EnemyHit);
            pman.AddDisconnectHandler("Enemy", "Background", this.KillOffscreen);
            pman.AddDisconnectHandler("EnemyBullet", "Background", this.KillOffscreen);
            pman.AddDisconnectHandler("Pickup", "BackgroundNegative", this.KillOffscreen);
            pman.RegisterClassShape("MainCharWhole", "MainChar", new Ellipse(7));
            pman.AddDisconnectHandler("MainCharBullet", "Background", this.KillOffscreen);
            pman.AddCollisionHandler("MainCharWhole", "Pickup", this.PickUp);
        }

        private GameEvent PickUp(Entity e1, Entity e2)
        {
            Pickup p = (e1 as Pickup) ?? (e2 as Pickup);
            double t = game.CurrentTime;
            return p.Effect(game, t) > p.Destroy(t);
        }

        private GameEvent EnemyHit(Entity e1, Entity e2)
        {
            Enemy e = (e1 as Enemy) ?? (e2 as Enemy);
            Bullet b = (e1 as Bullet) ?? (e2 as Bullet);
            e.Health -= b.Damage;
            b.DestructionTime = game.CurrentTime;
            return ((e.Health<=0 && Utils.IsZero(e.DestructionTime+1)) ? (e.Destroy(game.CurrentTime) > MakePickups(e) > b.Destruction) : b.Destruction);
        }

        private IEnumerable<Pickup> GeneratePickups(Enemy e)
        {
            int val = e.Value;
            int num = game.Random.Next(5, 8);
            val /= num;
            Vector<double> pos = e.Position.CurrentPosition;
            for (int i = 0; i < num; i++)
            {
                Pickup p = entSpawn.Build<Pickup>("Money", e.Time, pos[0], pos[1]);
                p.Effect = Effects.MakeQuantityChanger(new LambdaValue<int>(() => game.Money, (m) => game.Money = m), t => t + val);
                yield return p;
            }
        }

        private GameEvent MakePickups(Enemy e)
        {
            GameEvent ans = null;
            foreach (Pickup en in GeneratePickups(e))
                ans = ans > en.Creation;
            return ans;
        }

        private void InitializeRenderManager(RenderManager rman)
        {
            LinkedList<Id> ids = new LinkedList<Id>();
            ids.AddLast("Background");
            ids.AddLast("Character");
            ids.AddLast("Bullet");
            ids.AddLast("Pickup");
            rman.RenderOrder = ids;
        }

        private GameEvent KillOffscreen(Entity e1, Entity e2)
        {
            Entity e;
            if (e1 == bg)
                e = e2;
            else
                e = e1;
            if (e.InvisibilityTime > -0.5)
                return null;
            if (game.CurrentTime > e.CreationTime)
            {
                e.InvisibilityTime = game.CurrentTime;
                return e.Invisibility;
            }
            return null;
        }
        private GameEvent KillOffscreenNeg(Entity e1, Entity e2)
        {
            Entity e;
            if (e1 == bgneg)
                e = e2;
            else
                e = e1;
            if (e.InvisibilityTime > -0.5)
                return null;
            if (game.CurrentTime > e.CreationTime)
            {
                e.InvisibilityTime = game.CurrentTime;
                return e.Invisibility;
            }
            return null;
        }

        public GameEvent CharHit(Entity e1, Entity e2)
        {
            Entity me, bullet;
            me = e1; bullet = e2;
            if (e2 == game.Character)
            {
                me = e2;
                bullet = e1;
            }
            if (hitby[bullet])
                return null;
            GameEvent hitEvent = new GameEvent(e1.Time,
                (g, st) =>
                {
                    totalhits++;
                    hitby[bullet] = true;
                },
                (g, st) =>
                {
                    totalhits--;
                },
                (g, st) =>
                {
                    if (st == GameEventState.Processed)
                        totalhits--;
                    hitby[bullet] = false;
                });
            bullet.DestructionTime = e1.Time;
            return hitEvent > bullet.Destruction;
        }

        private void InitializeBulletEntities(EntitySpawner spawner)
        {
            Ellipse bulletEl = new Ellipse(5);
            EntityType bulletType = new EntityType(null,bulletEl,new GraphicsStyle(Brushes.OrangeRed),new EntityClass("EnemyBullet","Bullet"),null,Bullet.MakeBullet(20));

            spawner["OrangeRed_5"] = bulletType;
            spawner["Azure_5"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.Azure),true);
            spawner["HotPink_5"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.HotPink),true);
            spawner["MainCharBullet"] = bulletType.ChangeDrawPhysShape(new GraphicsStyle(Brushes.Violet), true).ChangeClass(new EntityClass("MainCharBullet", "Bullet"));
        }

        private void InitializeKeyManager()
        {
            keyMan.OnPress[Keys.Up] = this.OnKeyUpDown;
            keyMan.OnRelease[Keys.Up] = this.OnEndKeyUpDown;
            keyMan.OnPress[Keys.Down] = this.OnKeyUpDown;
            keyMan.OnRelease[Keys.Down] = this.OnEndKeyUpDown;
            keyMan.OnPress[Keys.Left] = this.OnKeyLeftRight;
            keyMan.OnRelease[Keys.Left] = this.OnEndKeyLeftRight;
            keyMan.OnPress[Keys.Right] = this.OnKeyLeftRight;
            keyMan.OnRelease[Keys.Right] = this.OnEndKeyLeftRight;
            keyMan.OnPress[Keys.T] = this.OnKeyT;
            keyMan.OnPress[Keys.P] = this.OnKeyP;
            keyMan.OnPress[Keys.F] = this.OnKeyF;
            keyMan.OnPress[Keys.Oemplus] = this.OnKeyPlus;
            keyMan.Repeat[Keys.Oemplus] = 10;
            keyMan.OnPress[Keys.OemMinus] = this.OnKeyMinus;
            keyMan.Repeat[Keys.OemMinus] = 10;
            keyMan.OnPress[Keys.Add] = this.OnKeyPlus;
            keyMan.Repeat[Keys.Add] = 10;
            keyMan.OnPress[Keys.Subtract] = this.OnKeyMinus;
            keyMan.Repeat[Keys.Subtract] = 10;
            keyMan.OnPress[Keys.Z] = this.OnFireKey;
            keyMan.OnRelease[Keys.Z] = this.OnRelFireKey;
        }

        private void OnFireKey(KeyManager km, Keys key, bool repeat = false)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
                game.Events.Add(game.Character.Emitter.ChangePattern(game.CurrentTime, 0));
        }

        private void OnRelFireKey(KeyManager km, Keys key)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
                game.Events.Add(game.Character.Emitter.ChangePattern(game.CurrentTime, -1));
        }

        BulletHell.Time.Timer timer, frameTimer, eventTimer;
        public void GameLoop()
        {
            timer = new BulletHell.Time.Timer();
            frameTimer = new BulletHell.Time.Timer();
            eventTimer = new BulletHell.Time.Timer();
            game.ResetTime();
            Thread renderThread = new Thread(this.ASynchGameLoop);
            renderThread.Start();
            while (this.Created)
            {
                eventTimer.Reset();
                Application.DoEvents();
                while (eventTimer.Time <= 30)
                    Thread.Yield();
            }
        }
        private void RenderScene()
        {
            if (buff != null)
            {
                Graphics g = buff.Graphics;
                //g.FillRectangle(Brushes.Black, this.ClientRectangle);
                game.Draw(g);
                if (DisplayFrameRate)
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(18, 18, 300, 48));
                    g.DrawString(string.Format("Framerate: {0}", fr), new Font("Arial", 12), Brushes.White, 20, 20);
                    g.DrawString(string.Format("Entities: {0}", entityCount), new Font("Arial", 12), Brushes.White, 20, 40);
                }
                if (DisplayTime)
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(18, 78, 300, 48));
                    g.DrawString(string.Format("Timerate: {0}", game.CurrentTimeRate), new Font("Arial", 12), Brushes.White, 20, 80);
                    g.DrawString(string.Format("Time: {0}", game.CurrentTime), new Font("Arial", 12), Brushes.White, 20, 100);
                }
                if (game.Paused)
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(18, 138, 300, 28));
                    g.DrawString("Paused", new Font("Arial", 12), Brushes.White, 20, 140);
                }
                g.FillRectangle(Brushes.Black, new Rectangle(18, 178, 300, 28));
                g.DrawString(string.Format("Hits: {0}", totalhits), new Font("Arial", 12), Brushes.White, 20, 180);
                if(this.Created)
                    buff.Render();
            }
        }
        double oldTime = 0;
        private void GameLogic()
        {
            double time = game.CurrentTime;

            game.Time = time;
            
            if (game.Time >= game.MostRenderedTime)
            {
                game.PhysicsManager.Collisions();
                LinkedList<Entity> newBullets = new LinkedList<Entity>();
                foreach (Entity o in game.BulletShooters)
                {
                    if (o.Emitter == null)
                        continue;
                    foreach (Bullet b in o.Emitter.BulletsBetween(o.Position, oldTime, time))
                    {
                        newBullets.AddLast(b);
                    }
                }
                foreach (Entity o in newBullets)
                    game.Add(o);
                oldTime = time; // there might be a bug here when stopping a rewind, haven't observed it yet.
            }
        }

        private void OnKeyMinus(KeyManager km, Keys key, bool repeat = false)
        {
            if (keyMan[Keys.ShiftKey])
                game.CurrentTimeRate -= 0.01;
            else
                game.CurrentTimeRate -= 0.1;
        }

        private void OnKeyPlus(KeyManager km, Keys key, bool repeat = false)
        {
            if (keyMan[Keys.ShiftKey])
                game.CurrentTimeRate += 0.01;
            else
                game.CurrentTimeRate += 0.1;
        }

        private void OnEndKeyLeftRight(KeyManager km, Keys key)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
            {
                game.Events.Add(new GameEvent(game.CurrentTime,
                    (g,state) => {
                        if(state == GameEventState.Unprocessed)
                            game.Character.XComp = ComputeXVel();
                    },
                    GameEvent.DoNothing,
                    (g,state) => {

                    }
                    ));
            }
        }

        private void OnEndKeyUpDown(KeyManager km, Keys key)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
            {
                game.Events.Add(new GameEvent(game.CurrentTime,
                    (g, state) =>
                    {
                        if (state == GameEventState.Unprocessed)
                            game.Character.YComp = ComputeYVel();
                    },
                    GameEvent.DoNothing,
                    (g, state) =>
                    {

                    }
                    ));
            }
        }
        private void OnKeyUpDown(KeyManager km, Keys key, bool repeat)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
            {
                game.Events.Add(new GameEvent(game.CurrentTime,
                    (g, state) =>
                    {
                        if (state == GameEventState.Unprocessed)
                            game.Character.YComp = ComputeYVel();
                    },
                    GameEvent.DoNothing,
                    (g, state) =>
                    {

                    }
                    ));
            }
        }
        private void OnKeyLeftRight(KeyManager km, Keys key, bool repeat)
        {
            if (game.CurrentTimeRate > Utils.TOLERANCE)
            {
                game.Events.Add(new GameEvent(game.CurrentTime,
                    (g, state) =>
                    {
                        if (state == GameEventState.Unprocessed)
                            game.Character.XComp = ComputeXVel();
                    },
                    GameEvent.DoNothing,
                    (g, state) =>
                    {

                    }
                    ));
            }
        }
        private void OnKeyF(KeyManager km, Keys key, bool repeat)
        {
            DisplayFrameRate = !DisplayFrameRate;
        }
        private void OnKeyP(KeyManager km, Keys key, bool repeat)
        {
            game.TogglePause();
        }
        private void OnKeyT(KeyManager km, Keys key, bool repeat)
        {
            DisplayTime = !DisplayTime;
        }

        private double ComputeXVel()
        {
            return (keyMan[Keys.Left] ? -1 : 0) + (keyMan[Keys.Right] ? 1 : 0);
        }
        private double ComputeYVel()
        {
            return (keyMan[Keys.Up] ? -1 : 0) + (keyMan[Keys.Down] ? 1 : 0);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            keyMan.KeyPressed(e.KeyCode);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            keyMan.KeyReleased(e.KeyCode);
        }

        double fr = 0;
        int entityCount = 0;
        private int totalhits;

        public void ASynchGameLoop()
        {
            int count = 0, FRAMES = 30;
            while (this.Created)
            {
                if (++count == FRAMES)
                {
                    fr = 1000 * (double)(FRAMES) / (double)frameTimer.Time;
                    //Console.WriteLine("FR: {0}", fr);
                    frameTimer.Reset();
                    count = 0;
                }
                entityCount = game.EntityCount;// game.Entities.Count();
                timer.Reset();
                GameLogic();
                RenderScene();
                while (timer.Time < 10)
                    Thread.Yield();
            }
        }
    }
}

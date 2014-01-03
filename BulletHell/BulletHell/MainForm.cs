using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
using BulletHell.CoordLib;
using System.Drawing.Drawing2D;
using BulletHell.GameLib.LevelLib;
using BulletHell.TestLevels;

namespace BulletHell
{

    public partial class MainForm : Form
    {
        KeyManager keyMan;

        Background bg;
        PartialBox boxNeg;

        BufferedGraphics buff, newbuff;
        BufferedGraphicsContext bgcon;

        Game game;
        bool DisplayFrameRate = true, DisplayTime = true;
        private DefaultValueDictionary<Entity, bool> hitby;

        ManagedCoords m;

        public MainForm()
        {
            InitializeComponent();

            SetClientSizeCore(1280, 720);

            oldWidth = this.Width;
            oldHeight = this.Height;

            TestLevel t = new TestLevel();

            m = new ManagedCoords(16, 9);

            m[true, 0] = 1280;
            m[false, 0] = 1280;

            hitby = new DefaultValueDictionary<Entity, bool>(false);

            double border = 40;

            Particle boxC = (Particle)(new Vector<double>((double)ClientRectangle.Width / 2, (double)ClientRectangle.Height / 2));
            Particle boxR = (Particle)(new Vector<double>((double)ClientRectangle.Width / 2 + border, (double)ClientRectangle.Height / 2 + border));
            Box bgbox = new Box(boxR);

            EntityClass bgClass = new EntityClass("Background");

            bg = new Background(1280, 720, border, Color.Black);
            boxNeg = new PartialBox(bg.Shape.BoundingBox, new Vector<bool>(true, false), new Vector<bool>(true, true));

            keyMan = new KeyManager();

            InitializeKeyManager();


            Drawable mchar = DrawableFactory.MakeCircle(8, new GraphicsStyle(Brushes.Orange, Pens.Red));

            EntityType mcb = new EntityType(null, new Ellipse(5), new GraphicsStyle(Brushes.Violet), new EntityClass("MainCharBullet", "Bullet"), null, Bullet.MakeBullet());

            MainChar mc = new MainChar(mchar, mcb, ClientRectangle.Width / 2, ClientRectangle.Height - 20, 40);

            game = Game.LoadFromLevel(mc, bg, t);

            game.CurrentTransform = m.Transform;

            InitializePhysicsManager(game.PhysicsManager);
            InitializeRenderManager(game.RenderManager);



            game.ResetTime();
            bgcon = BufferedGraphicsManager.Current;
            HandleResize();
        }

        private void HandleResize()
        {
            //Console.WriteLine("({0},{1})", ClientRectangle.Width, ClientRectangle.Height);
            this.m[false, 0] = ClientRectangle.Width;
            newbuff = bgcon.Allocate(CreateGraphics(), ClientRectangle);
        }


        private void InitializePhysicsManager(PhysicsManager pman)
        {
            pman.AddCollisionHandler("EnemyBullet", "MainChar", this.CharHit);
            pman.AddCollisionHandler("MainCharBullet", "Enemy", this.EnemyHit);
            pman.AddDisconnectHandler("Enemy", "Background", this.KillOffscreen);
            pman.AddDisconnectHandler("EnemyBullet", "Background", this.KillOffscreen);
            pman.RegisterClassShape("BackgroundNegative", "Background", this.boxNeg);
            pman.AddDisconnectHandler("Pickup", "BackgroundNegative", this.KillOffscreen);
            //pman.AddCollisionHandler("Pickup", "BackgroundNegative", this.PrintQ);
            pman.RegisterClassShape("MainCharWhole", "MainChar", new Ellipse(7));
            pman.AddDisconnectHandler("MainCharBullet", "Background", this.KillOffscreen);
            pman.AddCollisionHandler("MainCharWhole", "Pickup", this.PickUp);
        }

        private GameEvent PrintQ(Entity e1, Entity e2)
        {
            Console.WriteLine("Q");
            return null;
        }



        private GameEvent KillOffscreenPickup(Entity e1, Entity e2)
        {
            //PrintQ();
            return KillOffscreen(e1, e2);
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
            return ((e.Health <= 0 && Utils.IsZero(e.DestructionTime + 1)) ? (e.Destroy(game.CurrentTime) > MakePickups(e) > b.Destruction) : b.Destruction);
        }


        private GameEvent MakePickups(Enemy e)
        {
            GameEvent ans = null;
            foreach (Pickup en in e.Drop(game))
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
            keyMan.OnPress[Keys.R] = this.OnKeyR;
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
            if (newbuff != null)
            {
                buff = newbuff;
                newbuff = null;
            }
            if (buff != null)
            {
                Graphics g = buff.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
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
                if (this.Created)
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
            FullScreen = !FullScreen;
        }
        private void OnKeyR(KeyManager km, Keys key, bool repeat)
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

        private int oldWidth, oldHeight;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x214)
            {
                int wb = this.Width - ClientRectangle.Width;
                int hb = this.Height - ClientRectangle.Height;

                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                int w = rc.Right - rc.Left - wb;
                int h = rc.Bottom - rc.Top - hb;
                int z = 0;
                if (oldWidth == w)
                    z = 16 * h;
                else if (oldHeight == h)
                    z = w * 9;
                else
                    z = 9 * w > 16 * h ? 9 * w : 16 * h;
                w = z / 9;
                h = z / 16;
                rc.Bottom = rc.Top + h + hb;
                rc.Right = rc.Left + w + wb;
                Marshal.StructureToPtr(rc, m.LParam, false);
                m.Result = (IntPtr)1;
                oldWidth = w;
                oldHeight = h;
                HandleResize();
                return;
            }
            base.WndProc(ref m);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private bool fullscreen;
        public bool FullScreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                if (fullscreen == value)
                    return;
                fullscreen = value;
                if (fullscreen)
                {
                    this.TopMost = true;
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.TopMost = false;
                    this.SetClientSizeCore(1280, 720);
                }
                HandleResize();
            }
        }
    }
}

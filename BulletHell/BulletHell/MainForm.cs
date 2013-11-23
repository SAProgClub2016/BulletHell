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

namespace BulletHell
{
    public partial class MainForm : Form
    {
        int t = 0;
        int at = 0;
        //int it = 0;
        bool[] keys = new bool[256];

        Drawable o1, o2;
        Entity e, e2;

        BufferedGraphics buff;
        Game game;
        Particle MEEEEEEE;
        bool DisplayFrameRate = true;

        public MainForm()
        {
            InitializeComponent();
            int vx = 1, vx2 = 2;
            int vy = 2, vy2 = 4;
            game = new Game();

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            Particle p2 = new Particle(Utils.MakeClosure<double,double,double>(ClientRectangle.Width,(w,x) => w - vx2 * x), y => vy2 * y);

            Particle q = new Particle(t => 7 * t + 10 * Utils.FastCos(t), t => 3 * t + 10 * Utils.FastSin(t));

            o1 = DrawableFactory.MakeCircle(7, new GraphicsStyle(Brushes.Green));
            o2 = DrawableFactory.MakeCircle(5, new GraphicsStyle(Brushes.OrangeRed));
            Drawable o3 = DrawableFactory.ChangeDefStyles(o2, new GraphicsStyle(Brushes.Azure));

            double FULL = 2 * Math.PI;
            double cd = .5;
            int perCirc = 12;
            int offsets = 6;
            BulletEmission[] bEms = new BulletEmission[offsets], bEms2 = new BulletEmission[offsets];
            double DOWN = Math.PI / 2;
            // Makes the spiral pattern with bullets of shape o2
            BulletTrajectory[][] arrs = new BulletTrajectory[offsets][];
            for (int i = 0; i < offsets; i++)
                arrs[i] = new BulletTrajectory[perCirc];
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = BulletTrajectoryFactory.AngleMagVel(o2/*dcols[i%cols.Length]*/, (i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 10);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms[i] = new BulletEmission(cd, 0, arrs[i]);
            // Same as above, but we're gonna change the shape
            for (int i = 0; i < offsets; i++)
                arrs[i] = new BulletTrajectory[perCirc];
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = BulletTrajectoryFactory.AngleMagVel(o3/*dcols[i%cols.Length]*/, (i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 10);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms2[i] = new BulletEmission(cd, 0, arrs[i]);
            // Same as above, but we're gonna change the shape again and the path
            BulletEmission[] bEms3 = new BulletEmission[offsets];
            for (int i = 0; i < offsets; i++)
                arrs[i] = new BulletTrajectory[perCirc];
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = BulletTrajectoryFactory.SpinningLinearAMVel(DrawableFactory.ChangeDefStyles(o2,new GraphicsStyle(Brushes.HotPink)), (i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 7/*3*/, 0.5, 20);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms3[i] = new BulletEmission(cd, 0, arrs[i]);

            BulletEmitter em = new BulletEmitter(bEms);
            BulletEmitter em2 = new BulletEmitter(bEms2);
            BulletEmitter em3 = new BulletEmitter(bEms3);

            e = new Entity(p1, o1, em);
            e2 = new Entity(p2, o1, em);

            Particle p3 = new Particle(x => 0.5 * x + 500, y => 3 * y);
            Entity e3 = new Entity(p3, o1, em);

            Particle p4 = new Particle(x => -0.25 * x + 300, y => 3.5 * y);
            Entity e4 = new Entity(p4, o1, em);

            Particle p5 = new Particle(x => -0.4 * x + 800, y => 2 * y);
            Entity e5 = new Entity(p5, o1, em);

            game = game + e + e2 + e3 + e4 + e5;

            Entity e6 = new Entity(q, o1, em2);
            game += e6;

            Particle r = new Particle(Utils.MakeClosure<double,double,double>((double)ClientRectangle.Width/3, (w,t)=>w+3*t), t=>5*t);
            Entity e7 = new Entity(r,o1,em3);
            game += e7;

            BufferedGraphicsContext c = BufferedGraphicsManager.Current;
            buff = c.Allocate(CreateGraphics(), ClientRectangle);
            MEEEEEEE = new Particle(x => (double)ClientRectangle.Width / 2, y => (double)ClientRectangle.Height * 3 / 4);
        }
        BulletHell.Time.Timer timer, gameTime, frameTimer;
        public void GameLoop()
        {
            timer = new BulletHell.Time.Timer();
            gameTime = new BulletHell.Time.Timer();
            frameTimer = new BulletHell.Time.Timer();
            gameTime.Reset();
            Thread renderThread = new Thread(this.ASynchGameLoop);
            renderThread.Start();
            while (this.Created)
            {
                Application.DoEvents();
                //t += 1;
            }
        }
        private void RenderScene()
        {
            if (buff != null)
            {
                Graphics g = buff.Graphics;
                g.FillRectangle(Brushes.Black, this.ClientRectangle);
                game.Draw(g);
                if (DisplayFrameRate)
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(18, 18, 300, 48));
                    g.DrawString(string.Format("Framerate: {0}", fr), new Font("Arial", 12), Brushes.White, 20, 20);
                    g.DrawString(string.Format("Entities: {0}", entityCount), new Font("Arial", 12), Brushes.White, 20, 40);
                }
                if(this.Created)
                    buff.Render();
            }
        }
        double oldTime = 0;
        private void GameLogic()
        {
            int bounds = 20;
            double time = at;
            time /= 150;

            game.Time = time;
            List<Entity> removeList = new List<Entity>();
            foreach (Entity e in game.Entities)
            {
                Rectangle r = ClientRectangle;

                GenRect<double> gr = new GenRect<double>(new Vector<double>(r.X - bounds, r.Y - bounds), new Vector<double>(r.X + r.Width + bounds, r.Y + r.Height + bounds));
                if (!gr.Contains(e.Position.CurrentPosition))
                {
                    removeList.Add(e);
                }
            }
            foreach (Entity e in removeList)
                game -= e;
            List<Entity> newBullets = new List<Entity>();
            foreach (Entity o in game.Entities)
            {
                if (o.Emitter == null)
                    continue;
                foreach (Bullet b in o.Emitter.BulletsBetween(o.Position, oldTime, time))
                {
                    newBullets.Add(b);
                }
            }
            foreach (Entity o in newBullets)
                game.Add(o);
            oldTime = time;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && !keys[(int)e.KeyCode])
            {
                DisplayFrameRate = !DisplayFrameRate;
            }
            keys[(int)e.KeyCode] = true;
        }
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            keys[(int)e.KeyCode] = false;
        }
        double fr = 0;
        int entityCount = 0;

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
                entityCount = game.Entities.Count();
                timer.Reset();
                at = (int)gameTime.Time;
                GameLogic();
                RenderScene();
                while (timer.Time < 20) ;
            }
        }

        BulletHell.Time.Timer renderTimer=null;
        public void ArbitraryRender()
        {
            if (renderTimer == null)
            {
                renderTimer = new BulletHell.Time.Timer();
                ArbitraryRenderHelper();
            }
            else if (renderTimer.Time > 20)
            {
                renderTimer.Reset();
                ArbitraryRenderHelper();
            }
        }
        private void ArbitraryRenderHelper()
        {
            at = (int)gameTime.Time;
            GameLogic();
            RenderScene();
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            //ArbitraryRender();
        }
    }
}

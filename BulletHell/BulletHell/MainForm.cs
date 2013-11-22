using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulletHell.Time;
using BulletHell.Physics;
using BulletHell.Gfx;
using BulletHell.Gfx.Objects;
using BulletHell.Game;
using BulletHell.MathLib;

namespace BulletHell
{
    public partial class MainForm : Form
    {
        int t = 0;
        int at = 0;
        //int it = 0;
        bool[] keys = new bool[256];

        GraphicsObject o1, o2;
        Entity e,e2;
        BufferedGraphics buff;
        List<Entity> drawList;
        Particle MEEEEEEE;

        public MainForm()
        {
            InitializeComponent();
            int vx = 1,vx2=2;
            int vy = 2, vy2 = 4;
            drawList = new List<Entity>();

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            Particle p2 = new Particle(x => ClientRectangle.Width - vx2 * x, y => vy2 * y);
            
            //int radius = 10;
            //Particle p2 = new Particle(p1, x => radius * Math.Cos(x), y => radius * Math.Sin(y));
            o1 = new Ellipse(p1, 7, 7);
            o2 = new Ellipse(null, 5, 5);
            
            o1.ObjectBrush = Brushes.Aquamarine;
            o2.ObjectBrush = Brushes.OrangeRed;
            //double PIOSIX = Math.PI/6;
            double FULL = 2 * Math.PI;
            double cd = .5;
            int perCirc = 12;
            int offsets = 6;
            BulletEmission[] bEms = new BulletEmission[offsets];
            double DOWN = Math.PI / 2;
            BulletTrajectory[][] arrs = new BulletTrajectory[offsets][];
            for (int i = 0; i < offsets; i++)
                arrs[i] = new BulletTrajectory[perCirc];
            Brush[] cols = { Brushes.AliceBlue, Brushes.Orange, Brushes.PaleGoldenrod, Brushes.GreenYellow, Brushes.IndianRed, Brushes.Cyan, Brushes.Crimson, Brushes.Pink, Brushes.SeaGreen, Brushes.Silver, Brushes.Salmon, Brushes.Purple};
            GraphicsObject[] dcols = new GraphicsObject[cols.Length];
            for (int i = 0; i < dcols.Length; i++)
            {
                dcols[i] = o2.Clone();
                dcols[i].ObjectBrush = cols[i];
            }
            for (int i = 0; i < perCirc; i++)
            {
                for (int j = 0; j < offsets; j++)
                {
                    arrs[j][i] = BulletTrajectoryFactory.AngleMagVel(o2/*dcols[i%cols.Length]*/, (i * offsets + j) * (FULL / offsets / perCirc) + DOWN, 10);
                }
            }
            for (int i = 0; i < offsets; i++)
                bEms[i] = new BulletEmission(cd, 0, arrs[i]);

            //bEms[0]=new BulletEmission(5,0,new BulletTrajectory[2] {BulletTrajectoryFactory.AngleMagVel(o2,DOWN+PIOSIX,10), BulletTrajectoryFactory.AngleMagVel(o2,DOWN-PIOSIX,10)});
            //bEms[1]=new BulletEmission(5,0,new BulletTrajectory[2] {BulletTrajectoryFactory.AngleMagVel(o2,DOWN-Math.PI/12,10), BulletTrajectoryFactory.AngleMagVel(o2,DOWN+Math.PI/12,10)});
            int nbems=5;
            BulletEmitter[] ems = new BulletEmitter[nbems];
            for (int i = 0; i < ems.Length; i++)
            {
                ems[i] = new BulletEmitter(bEms);
            }

            e = new Entity(o1.Clone(), ems[0]);
            GraphicsObject ell = o1.Clone();
            ell.Position = p2;
            e2 = new Entity(ell, ems[1]);

            GraphicsObject ell2 = o1.Clone();
            ell2.Position = new Particle(x => 0.5 * x + 500, y => 3 * y);
            Entity e3 = new Entity(ell2, ems[2]);

            GraphicsObject ell3 = o1.Clone();
            ell3.Position = new Particle(x => -0.25 * x + 300, y => 3.5 * y);
            Entity e4 = new Entity(ell3, ems[3]);

            GraphicsObject ell4 = o1.Clone();
            ell4.Position = new Particle(x => -0.4 * x + 800, y => 2 * y);
            Entity e5 = new Entity(ell4, ems[4]);

            drawList.Add(e);
            drawList.Add(e2);
            drawList.Add(e3);
            drawList.Add(e4);
            drawList.Add(e5);

            BufferedGraphicsContext c = BufferedGraphicsManager.Current;
            buff = c.Allocate(CreateGraphics(), ClientRectangle);
            MEEEEEEE = new Particle(x => (double)ClientRectangle.Width / 2, y => (double)ClientRectangle.Height * 3 / 4);
        }
        public void GameLoop()
        {
            BulletHell.Time.Timer timer = new BulletHell.Time.Timer();
            BulletHell.Time.Timer gameTime = new BulletHell.Time.Timer();
            BulletHell.Time.Timer frameTimer = new BulletHell.Time.Timer();
            gameTime.Reset();
            int count = 0,FRAMES=30;
            while (this.Created)
            {
                if (++count == FRAMES)
                {
                    Console.WriteLine("FR: {0}", 1000*(double)(FRAMES) / (double)frameTimer.Time);
                    frameTimer.Reset();
                    count = 0;
                }
                timer.Reset();
                GameLogic();
                RenderScene();
                Application.DoEvents();
                t += 1;
                at = (int)gameTime.Time;
                while (timer.Time < 20) ;
            }
        }
        private void RenderScene()
        {
            if (buff != null)
            {
                Graphics g = buff.Graphics;
                g.FillRectangle(Brushes.Black, this.ClientRectangle);
                //Console.WriteLine(drawList.Count);
                for (int i = 0; i < drawList.Count; i++ )
                {
                    Entity o = drawList[i];
                    Vector<double> cpos = o.MyShape.Position.CurrentPosition;
                    Rectangle r = ClientRectangle;

                    if (cpos[0] < -20 || cpos[1] < -20 || cpos[0] > r.Width +20 || cpos[1] > r.Height+20)
                    {
                        drawList.Remove(o);
                        i--;
                    }

                    //Console.WriteLine(o);
                    o.MyShape.DrawObject(g);
                }
                //o2.DrawObject(g);
                buff.Render();
            }
        }
        double oldTime = 0;
        private void GameLogic()
        {
            
            double time = at;
            time /= 150;
            if (e != null)
            {
                List<Entity> newBullets = new List<Entity>();
                foreach (Entity o in drawList)
                {
                    if (o.Emitter == null)
                        continue;
                    foreach (Bullet b in o.Emitter.BulletsBetween(oldTime, time))
                    {
                        //b.MyShape.Position.Parent = o.MyShape.Position;
                        newBullets.Add(b);
                    }
                }
                foreach(Entity o in newBullets)
                    drawList.Add(o);
                oldTime = time;
                foreach (Entity o in drawList)
                {
                    o.MyShape.Position.Time = time;
                }
            }
            else {
                Console.WriteLine(e);
            }
            //Console.OpenStandardOutput();
            //Console.WriteLine("Help"+o1.Position);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            keys[(int)e.KeyCode] = true;
        }
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            keys[(int)e.KeyCode] = false;
        }
    }
}

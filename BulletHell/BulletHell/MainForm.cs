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

        GraphicsObject o1, o2;
        Entity e,e2;
        BufferedGraphics buff;
        List<Entity> drawList;

        public MainForm()
        {
            InitializeComponent();
            int vx = 3,vx2=4;
            int vy = 5, vy2 = 7;
            drawList = new List<Entity>();

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            Particle p2 = new Particle(x => ClientRectangle.Width - vx2 * x, y => vy2 * y);
            
            //int radius = 10;
            //Particle p2 = new Particle(p1, x => radius * Math.Cos(x), y => radius * Math.Sin(y));
            o1 = new Ellipse(p1, 5, 5);
            o2 = new Ellipse(null, 5, 5);
            
            o1.ObjectBrush = Brushes.Aquamarine;
            o2.ObjectBrush = Brushes.OrangeRed;
            BulletEmission[] bEms = new BulletEmission[2];
            bEms[0]=new BulletEmission(2.5,0,new BulletTrajectory[2] {BulletTrajectoryFactory.SimpleVel(o2,10,0), BulletTrajectoryFactory.SimpleVel(o2,-10,0)});
            bEms[1]=new BulletEmission(2.5,0,new BulletTrajectory[2] {BulletTrajectoryFactory.SimpleVel(o2,0,10), BulletTrajectoryFactory.SimpleVel(o2,0,-10)});
            BulletEmitter em = new BulletEmitter(bEms);
            e = new Entity(new Ellipse(p1, 7, 7), em);
            e2 = new Entity(new Ellipse(p2, 7, 7), em);
            drawList.Add(e);
            drawList.Add(e2);

            BufferedGraphicsContext c = BufferedGraphicsManager.Current;
            buff = c.Allocate(CreateGraphics(), ClientRectangle);
        }
        public void GameLoop()
        {
            BulletHell.Time.Timer timer = new BulletHell.Time.Timer();
            while (this.Created)
            {
                timer.Reset();
                GameLogic();
                RenderScene();
                Application.DoEvents();
                t += 1;
                while (timer.Time < 50) ;
            }
        }
        private void RenderScene()
        {
            if (buff != null)
            {
                Graphics g = buff.Graphics;
                g.FillRectangle(Brushes.White, this.ClientRectangle);
                //Console.WriteLine(drawList.Count);
                for (int i = 0; i < drawList.Count; i++ )
                {
                    Entity o = drawList[i];
                    Vector<double> cpos = o.MyShape.Position.CurrentPosition;
                    Rectangle r = ClientRectangle;

                    if (cpos[0] < 0 || cpos[1] < 0 || cpos[0] > r.Width || cpos[1] > r.Height)
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
            
            double time = t;
            time /= 3;
            if (e != null)
            {
                List<Entity> newBullets = new List<Entity>();
                foreach (Entity o in drawList)
                {
                    if (o.Emitter == null)
                        continue;
                    foreach (Bullet b in o.Emitter.BulletsBetween(oldTime, time))
                    {
                        b.MyShape.Position.Parent = o.MyShape.Position;
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
    }
}

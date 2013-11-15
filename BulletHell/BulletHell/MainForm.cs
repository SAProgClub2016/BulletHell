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
using System.Collections.Generic;

namespace BulletHell
{
    public partial class MainForm : Form
    {
        int t = 0;

        GraphicsObject o1, o2;
        BufferedGraphics buff;
        List<GraphicsObject> drawList;

        public MainForm()
        {
            InitializeComponent();
            int vx = 3;
            int vy = 5;
            drawList = new List<GraphicsObject>();

            Particle p1 = new Particle(x => vx * x, y => vy * y);
            int radius = 10;
            Particle p2 = new Particle(p1, x => radius * Math.Cos(x), y => radius * Math.Sin(y));
            o1 = new Ellipse(p1, 5, 5);
            o2 = new Ellipse(p2, 5, 5);
            
            o1.ObjectBrush = Brushes.Aquamarine;
            o2.ObjectBrush = Brushes.OrangeRed;
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
                g.FillRectangle(Brushes.Black, this.ClientRectangle);
                o1.DrawObject(g);
                for (int i = 0; i < 4; i++)
                {
                    o2.DrawObject(((double)t)/3+((double)i)/9,g);
                }
                //o2.DrawObject(g);
                buff.Render();
            }
        }
        private void GameLogic()
        {
            double time = t;
            time /= 3;
            o1.Position.Time = time;
            o2.Position.Time = time;
            Console.OpenStandardOutput();
            Console.WriteLine("Help"+o1.Position);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}

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

namespace BulletHell
{
    public partial class MainForm : Form
    {
        int t = 0;

        Particle p1;
        Particle p2;

        public MainForm()
        {
            InitializeComponent();
            int vx = 3;
            int vy = 5;
            p1 = new Particle(x => vx * x, y => vy * y);
            int radius = 10;
            p2 = new Particle(p1, x => radius * Math.Cos(x), y => radius * Math.Sin(y));
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
            Graphics g = CreateGraphics();
            g.FillRectangle(Brushes.White, this.ClientRectangle);
            int r = 20;
            g.FillEllipse(Brushes.Aquamarine, new Rectangle((int)p1.CurrentPosition[0], (int)p1.CurrentPosition[1], r, r));
            g.FillEllipse(Brushes.OrangeRed, new Rectangle((int)p2.CurrentPosition[0], (int)p2.CurrentPosition[1], r, r));
        }
        private void GameLogic()
        {
            double time = t;
            time /= 3;
            p1.Time = time;
            p2.Time = time;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}

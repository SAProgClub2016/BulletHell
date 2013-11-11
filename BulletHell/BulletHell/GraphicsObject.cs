using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell
{
    public interface GraphicsObject
    {
        public void DrawObject(double t, Graphics g);
        public Brush ObjectBrush { get; set; }
        public Pen ObjectPen { get; set; }
    }
    public class Ellipse : GraphicsObject
    {
        private Particle p, r;
        private bool rParented = false;
        public Brush ObjectBrush { get; set; }
        public Pen ObjectPen { get; set; }
        public Ellipse(Particle p, double rx, double ry)
            : this(p, new Particle(x => rx, y => ry), false)
        {
        }
        public Ellipse(Particle p, Particle rrad, bool radParented = false)
        {
            ObjectBrush = Brushes.Black;
            ObjectPen = null;
            this.p = p;
            this.r = rrad;
            rParented = radParented;
        }
        public void DrawObject(double t, Graphics g)
        {
            Vector<double> pos = p.Position(t);
            Vector<double> rrad = r.Position(t);
            if (rParented)
            {
                rrad -= pos;
            }
            Vector<double> apos = pos-rrad;
            Rectangle bounds = new Rectangle((int)apos[0],(int)apos[1],(int)(2*rrad[0]),(int)(2*rrad[1])));
            if(ObjectBrush!=null)
            {
                g.FillEllipse(ObjectBrush,bounds);
            }
            if(ObjectPen!=null)
            {
                g.DrawEllipse(ObjectPen,bounds);
            }
        }
    }
}

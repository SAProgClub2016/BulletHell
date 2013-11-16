using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell.Gfx
{
    public interface GraphicsObject
    {
        Particle Position { get; set; }
        void DrawObject(double t, Graphics g);
        Brush ObjectBrush { get; set; }
        Pen ObjectPen { get; set; }
        void DrawObject(System.Drawing.Graphics g);
        GraphicsObject Clone();
    }
}
namespace BulletHell.Gfx.Objects
{
    public class Ellipse : GraphicsObject
    {
        private Particle p, r;
        private bool rParented = false;
        public Brush ObjectBrush { get; set; }
        public Pen ObjectPen { get; set; }
        public Ellipse(Particle p, double rx, double ry)
            : this(p, new Particle(x => rx, y => ry))
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
            Rectangle bounds = new Rectangle((int)apos[0],(int)apos[1],(int)(2*rrad[0]),(int)(2*rrad[1]));
            if(ObjectBrush!=null)
            {
                g.FillEllipse(ObjectBrush,bounds);
            }
            if(ObjectPen!=null)
            {
                g.DrawEllipse(ObjectPen,bounds);
            }
        }
        public void DrawObject(Graphics g)
        {
            Vector<double> pos = p.CurrentPosition;
            Vector<double> rrad = r.CurrentPosition;
            if (rParented)
            {
                rrad -= pos;
            }
            //Console.WriteLine(rrad);
            Vector<double> apos = pos-rrad;
            //Console.WriteLine(apos);
            //Console.WriteLine(rrad - pos);
            Rectangle bounds = new Rectangle((int)apos[0],(int)apos[1],(int)(2*rrad[0]),(int)(2*rrad[1]));
            //Console.WriteLine(bounds);
            if(ObjectBrush!=null)
            {
                g.FillEllipse(ObjectBrush,bounds);
            }
            if(ObjectPen!=null)
            {
                g.DrawEllipse(ObjectPen,bounds);
            }
        }
        public Particle Position
        {
            get
            {
                return p;
            }
            set
            {
                p = value;
            }
        }
        public GraphicsObject Clone()
        {
            Ellipse ans;
            if (rParented)
            {
                Particle s = new Particle(x => r.Position(x) - p.Position(x));
                ans = new Ellipse(p, s, false);
            }
            else
                ans = new Ellipse(p, r, false);
            ans.ObjectBrush = ObjectBrush;
            ans.ObjectPen = ObjectPen;
            return ans;
        }
    }
}

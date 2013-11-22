using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell.Gfx
{
    public delegate void Drawable(Particle p, Graphics g, GraphicsStyle sty = null);
    public static class DrawableFactory
    {
        public static Drawable MakeEllipse(Particle rad, GraphicsStyle def = null)
        {
            def = def ?? GraphicsStyle.DEFAULT;
            return (Particle p, Graphics g, GraphicsStyle sty) =>
            {
                sty = sty ?? def;
                rad.Time = p.Time;
                Vector<double> ul = p.CurrentPosition - rad.CurrentPosition;
                Vector<double> lw = 2 * rad.CurrentPosition;
                Vector<int> uli = ul.Map(x => (int)x);
                Vector<int> lwi = lw.Map(x => (int)x);
                Rectangle r = new Rectangle(uli[0], uli[1], lwi[0], lwi[1]);
                if (sty.Brush != null)
                    g.FillEllipse(sty.Brush, r);
                if (sty.Pen != null)
                    g.DrawEllipse(sty.Pen, r);
            };
        }
        public static Drawable MakeCircle(double rad, GraphicsStyle def = null)
        {
            return MakeEllipse(new Particle(x => rad, y => rad), def);
        }
        public static Drawable MakeCenteredRectangle(Particle whr, GraphicsStyle def)
        {
            return (Particle p, Graphics g, GraphicsStyle sty) =>
            {
                sty = sty ?? def;
                whr.Time = p.Time;
                Vector<double> ul = p.CurrentPosition - whr.CurrentPosition;
                Vector<double> lw = 2 * whr.CurrentPosition;
                Vector<int> uli = ul.Map(x => (int)x);
                Vector<int> lwi = lw.Map(x => (int)x);
                Rectangle r = new Rectangle(uli[0], uli[1], lwi[0], lwi[1]);
                if (sty.Brush != null)
                    g.FillRectangle(sty.Brush, r);
                if (sty.Pen != null)
                    g.DrawRectangle(sty.Pen, r);
            };
        }
        public static Drawable MakeNull()
        {
            return (p, g, sty) => { };
        }
        public static Drawable MakeNullTo(Drawable d, double t)
        {
            return (p, g, sty) =>
            {
                if (p.Time > t)
                {
                    d(p, g, sty);
                }
            };
        }
        public static Drawable ChangeDefStyles(Drawable d, GraphicsStyle s)
        {
            return (p, g, sty) =>
            {
                sty = sty ?? s;
                d(p, g, sty);
            };
        }
    }
}

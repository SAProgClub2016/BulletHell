using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell.Gfx
{
    public delegate Vector<double> CoordTransform(Vector<double> v);
    public delegate void Drawable(Particle p, Graphics g, CoordTransform f = null, GraphicsStyle sty = null);


    public static class DrawableFactory
    {
        public static Drawable ManageCoordTransform(Drawable d)
        {
            return (p, g, f, sty) =>
            {
                f = f ?? Utils.Identity<Vector<double>>;
                d(p, g, f, sty);
            };
        }
        public static Drawable MakeEllipse(Particle rad, GraphicsStyle def = null)
        {
            def = def ?? GraphicsStyle.DEFAULT;
            return ManageCoordTransform((Particle p, Graphics g, CoordTransform f, GraphicsStyle sty) =>
            {
                sty = sty ?? def;
                rad.Time = p.Time;
                Vector<double> radp = f(rad.CurrentPosition);
                Vector<double> pp = f(p.CurrentPosition);
                Vector<double> ul = pp - radp;
                Vector<double> lw = 2 * radp;
                Vector<int> uli = ul.Map(x => (int)x);
                Vector<int> lwi = lw.Map(x => (int)x);
                Rectangle r = new Rectangle(uli[0], uli[1], lwi[0], lwi[1]);
                if (sty.Brush != null)
                    g.FillEllipse(sty.Brush, r);
                if (sty.Pen != null)
                    g.DrawEllipse(sty.Pen, r);
            });
        }
        public static Drawable MakeCircle(double rad, GraphicsStyle def = null)
        {
            return MakeEllipse(new Particle(x => rad, y => rad), def);
        }
        public static Drawable MakeCenteredRectangle(Particle whr, GraphicsStyle def)
        {
            return ManageCoordTransform((Particle p, Graphics g, CoordTransform f, GraphicsStyle sty) =>
            {
                sty = sty ?? def;
                whr.Time = p.Time;
                Vector<double> whrp = f(whr.CurrentPosition);
                Vector<double> pp = f(p.CurrentPosition);
                Vector<double> ul = pp-whrp;
                Vector<double> lw = 2 * whrp;
                Vector<int> uli = ul.Map(x => (int)x);
                Vector<int> lwi = lw.Map(x => (int)x);
                Rectangle r = new Rectangle(uli[0], uli[1], lwi[0], lwi[1]);
                if (sty.Brush != null)
                    g.FillRectangle(sty.Brush, r);
                if (sty.Pen != null)
                    g.DrawRectangle(sty.Pen, r);
            });
        }
        public static Drawable MakeNull()
        {
            return (p, g, f, sty) => { };
        }
        public static Drawable MakeNullTo(Drawable d, double t)
        {
            return (p, g, f, sty) =>
            {
                if (p.Time > t)
                {
                    d(p, g, f, sty);
                }
            };
        }
        public static Drawable ChangeDefStyles(Drawable d, GraphicsStyle s)
        {
            return (p, g, f, sty) =>
            {
                sty = sty ?? s;
                d(p, g, f, sty);
            };
        }
    }
}

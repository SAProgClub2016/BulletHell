using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using System.Drawing;
using BulletHell.MathLib;

namespace BulletHell.Physics
{
    public class Box : PhysicsShape
    {
        private Particle r;
        private Drawable myDraw;

        public Box(Particle rad)
        {
            r = rad;
            myDraw = DrawableFactory.MakeCenteredRectangle(r, new GraphicsStyle(null, Pens.Red));
        }


        protected override void Draw(Particle p, Graphics g, GraphicsStyle sty = null)
        {
            myDraw(p, g, sty);
        }

        protected override bool containsPoint(Particle p)
        {
            return ContainsPoint(Particle.Origin(p.Dimension));
        }

        protected override bool containsPoint(Particle pos, Particle p)
        {
            if(p.Dimension!=r.Dimension || p.Dimension!=pos.Dimension)
                return false;
            Vector<double> cp = p.CurrentPosition;
            Vector<double> mcp = pos.CurrentPosition;
            Vector<double> diff = cp-mcp;
            r.Time=pos.Time;
            Vector<double> rp = r.CurrentPosition;
            for (int i = 0; i < pos.Dimension; i++)
            {
                if (Math.Abs(diff[i]) > Math.Abs(rp[i]))
                    return false;
            }
            return true;
        }

        protected override bool meets(Particle p, PhysicsShape o, Particle oPos)
        {
            if(o.GetType()==typeof(Point))
            {
                return ContainsPoint(p,oPos);
            }
            if(o.GetType()==typeof(Box))
            {
                Box oth = o as Box;
                return new Box(r + oth.r).ContainsPoint(p, oPos);
            }
            if(o.GetType()==typeof(Ellipse))
            {
                Ellipse oth = o as Ellipse;
                if (!Meets(p, oth.BoundingBox, oPos))
                    return false;

                if (p.Dimension != r.Dimension || p.Dimension != oPos.Dimension)
                    return false;
                Vector<double> ocp = oPos.CurrentPosition;
                Vector<double> mcp = p.CurrentPosition;
                Vector<double> diff = ocp - mcp;
                diff = diff.Map(Math.Abs, diff);
                r.Time = p.Time;
                Vector<double> rp = r.CurrentPosition;
                rp.Map(Math.Abs, rp);
                for (int i = 0; i < p.Dimension; i++)
                {
                    if (Math.Abs(diff[i]) > Math.Abs(rp[i]))
                        return false;
                }
                return true;
            }
            return o.Meets(oPos, this, p);
        }

        public override Box BoundingBox
        {
            get { return this; }
        }

        protected override void UpdateTime()
        {
            r.Time = Time;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using System.Drawing;
using BulletHell.MathLib;

namespace BulletHell.Physics.ShapeLib
{
    public class PartialBox : PhysicsShape
    {
        private Particle r;
        private Vector<bool> firstBounded, secondBounded;
        private Drawable myDraw;

        public PartialBox(Particle rad, Vector<bool> firstBounded, Vector<bool> secondBounded)
        {
            r = rad;
            myDraw = DrawableFactory.MakeCenteredRectangle(r, new GraphicsStyle(null, Pens.Red));
            this.firstBounded = firstBounded; this.secondBounded = secondBounded;
        }


        protected override void Draw(Particle p, Graphics g, CoordTransform f = null, GraphicsStyle sty = null)
        {
            myDraw(p, g, f, sty);
        }

        protected override bool containsPoint(Particle pos, Particle p)
        {
            if (p.Dimension != r.Dimension || p.Dimension != pos.Dimension)
                return false;
            Vector<double> cp = p.CurrentPosition;
            Vector<double> mcp = pos.CurrentPosition;
            Vector<double> diff = cp - mcp;
            Vector<double> rp = r.CurrentPosition;
            diff.Map(Math.Abs,diff);
            for (int i = 0; i < pos.Dimension; i++)
            {
                if ((firstBounded[i] && -rp[i] > diff[i]) || (secondBounded[i] && rp[i] < diff[i]))
                    return false;
            }
            return true;
        }

        protected override bool meets(Particle p, PhysicsShape o, Particle oPos)
        {
            if (o.GetType() == typeof(Point))
            {
                return ContainsPoint(p, oPos);
            }
            if(o.GetType() == typeof(PartialBox))
            {
                PartialBox pb = o as PartialBox;
                return new PartialBox(r+pb.Radius, firstBounded.Map((x,y)=>x||y,pb.secondBounded), secondBounded.Map((x,y)=>x||y,pb.firstBounded)).ContainsPoint(p, oPos);
            }
            if (o.GetType() == typeof(Box))
            {
                Box oth = o as Box;
                return new PartialBox(r + oth.Radius,firstBounded,secondBounded).ContainsPoint(p, oPos);
            }
            if (o.GetType() == typeof(Ellipse))
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

                Vector<double> rp = r.CurrentPosition;
                rp.Map(Math.Abs, rp);

                for (int i = 0; i < p.Dimension; i++)
                {
                    if (diff[i] < rp[i])
                        return true;
                }
                return o.ContainsPoint(diff, rp);
            }
            return o.Meets(oPos, this, p);
        }

        public Particle Radius
        {
            get
            { return r; }
        }

        public override Box BoundingBox
        {
            get { return null; } // OH GODS THAT'S BAD
        }

        protected override void UpdateTime()
        {
            r.Time = Time;
        }
    }
}

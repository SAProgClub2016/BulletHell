using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Gfx;

namespace BulletHell.Physics.ShapeLib
{
    public class Point : PhysicsShape
    {
        private static Drawable pointShape;
        private int dimension;

        static Point()
        {
            pointShape = DrawableFactory.MakeCircle(1, new GraphicsStyle(Brushes.Red, null));
        }

        public Point(int d)
        {
            dimension = d;
        }

        protected override void Draw(Particle p, Graphics g, GraphicsStyle sty = null)
        {
            pointShape(p, g, sty);
        }


        protected override bool containsPoint(Particle pos, Particle p)
        {
            return ContainsPoint(p - pos);
        }

        protected override bool meets(Particle p, PhysicsShape o, Particle oPos)
        {
            return o.ContainsPoint(oPos, p);
        }

        public override Box BoundingBox
        {
            get
            {
                return new Box(Particle.Origin(dimension));
            }
        }

        protected override void UpdateTime()
        {
            return;
        }
    }
}

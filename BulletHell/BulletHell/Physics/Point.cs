using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.Gfx;

namespace BulletHell.Physics
{
    class Point : PhysicsShape
    {
        private static Drawable pointShape;

        static Point()
        {
            pointShape = DrawableFactory.MakeCircle(1, new GraphicsStyle(Brushes.Red, null));
        }

        protected override void Draw(Particle p, Graphics g, GraphicsStyle sty = null)
        {
            pointShape(p, g, sty);
        }

        public override bool ContainsPoint(Particle p)
        {
            return Utils.IsZero(p.CurrentPosition.Length2);
        }

        public override bool ContainsPoint(Particle pos, Particle p)
        {
            return ContainsPoint(p - pos);
        }

        public override bool Meets(Particle p, PhysicsShape o, Particle oPos)
        {
            return o.ContainsPoint(oPos, p);
        }
    }
}

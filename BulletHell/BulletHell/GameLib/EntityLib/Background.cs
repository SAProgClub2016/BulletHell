using BulletHell.Gfx;
using BulletHell.MathLib;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
    public class Background : Entity
    {
        public delegate Drawable RectDrawableMaker(double w, double h);

        public Background(double w, double h, double b, RectDrawableMaker rdm) : base(0, new Vector<double>(w/2,h/2),rdm(w,h),new Box(new Vector<double>(w/2+b,h/2+b)),new EntityClass("Background","Background"))
        {
        }
        public Background(double w, double h, double b, Color c) : this(w,h,b,MakeColoredRect(c))
        {
        }

        public static RectDrawableMaker MakeColoredRect(Color c)
        {
            return (w, h) =>
                {
                    return DrawableFactory.MakeCenteredRectangle(new Vector<double>(w / 2, h / 2), new GraphicsStyle(new SolidBrush(c)));
                };
        }
    }
}

using BulletHell.Gfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BulletHell.MathLib;

namespace BulletHell.Physics.ShapeLib
{
    public abstract class PhysicsShape
    {
        private double time;
        public Drawable DrawShape
        {
            get
            {
                return this.Draw;
            }
        }

        public PhysicsShape()
        {
            time = 0;

        }
        protected abstract void Draw(Particle p, Graphics g, GraphicsStyle sty = null);

        public Drawable MakeDrawable(GraphicsStyle defSty = null)
        {
            return (p, g, sty) => Draw(p, g, sty ?? defSty);
        }

        public bool ContainsPoint(Particle p)
        {
            return ContainsPoint(Particle.Origin(p.Dimension), p);
        }
        public bool ContainsPoint(Particle pos, Particle p)
        {
            Time = p.Time;
            return containsPoint(pos, p);
        }
        protected abstract bool containsPoint(Particle pos, Particle p);
        public bool Meets(Particle p, PhysicsShape o, Particle oPos)
        {
            Time = p.Time;
            return meets(p, o, oPos);
        }
        protected abstract bool meets(Particle p, PhysicsShape o, Particle oPos);
        public abstract Box BoundingBox {get;}
        protected abstract void UpdateTime();

        public double Time
        {
            get
            {
                return time;
            }
            set
            {
                if (Utils.IsZero(time - value))
                    return;
                time = value;
                UpdateTime();
            }
        }
    }
    public static class PhysicsShapeTest
    {
        public static void Main()
        {
            Box b = new Box(new Vector<double>(30, 30));
            Ellipse o = new Ellipse(20);
            Box s = o.BoundingBox;
            Point p = new Point(2);

            for(int i = -10;i<100;i+=10)
            {
                for(int j =-10;j<100;j+=10)
                {
                    Vector<double> pos = new Vector<double>(i,j);
                    Console.WriteLine("Box {0} ellipse at {1}", b.Meets(Particle.Origin(2), o, pos)?"meets":"does not meet",pos);
                }
            }
            Console.ReadKey();
        }
    }
}

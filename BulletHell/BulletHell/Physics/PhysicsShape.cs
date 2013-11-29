using BulletHell.Gfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BulletHell.Physics
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
}

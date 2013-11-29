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
        public Drawable DrawShape
        {
            get
            {
                return this.Draw;
            }
        }

        protected abstract void Draw(Particle p, Graphics g, GraphicsStyle sty = null);
        public abstract bool ContainsPoint(Particle p);
        public abstract bool ContainsPoint(Particle pos, Particle p);
        public abstract bool Meets(Particle p, PhysicsShape o, Particle oPos);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell;

namespace BulletHell.GameLib
{
    public class Entity
    {
        Particle pos;
        Drawable d;
        BulletEmitter e;

        public double Time
        {
            get
            {
                return pos.Time;
            }
            set
            {
                pos.Time = value;
            }
        }
        public Particle Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }
        public Drawable Draw
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
            }
        }
        
        public BulletEmitter Emitter
        {
            get
            {
                return e;
            }
            set
            {
                e = value;
            }
        }

        public Entity(Particle pos, Drawable draw, BulletEmitter e)
        {
            this.pos = pos;
            d = draw;
            this.e = e;
        }
        public Entity(Particle pos, Drawable draw)
            : this(pos, draw, null)
        {

        }

    }
}

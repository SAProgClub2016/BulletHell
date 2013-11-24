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
        GraphicsStyle gs;
        public readonly double CreationTime;
        private double iTime=-1,dTime=-1;
        public double InvisibilityTime { get { return iTime; } set { iTime = value; } }
        public double DestructionTime { get { return dTime; } set { dTime = value; } }

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

        public GraphicsStyle Style
        {
            get
            {
                return gs;
            }
            set
            {
                gs = value;
            }
        }

        public Entity(double cTime, Particle pos, Drawable draw, BulletEmitter e = null, GraphicsStyle g = null)
        {
            CreationTime = cTime;
            this.pos = pos;
            d = draw;
            this.e = e;
            gs = g;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell;
using BulletHell.GameLib.EntityLib.BulletLib;

namespace BulletHell.GameLib.EntityLib
{
    public class Entity
    {
        public EntityManager Manager { get; set; }
        private Particle pos;
        private Drawable d;
        private PhysicsShape ps;
        private BulletEmitter e;
        private GraphicsStyle gs;
        public readonly double CreationTime;
        private double iTime=-1,dTime=-1;
        private PhysicsClass pc;

        public PhysicsShape Shape
        {
            get
            {
                return ps;
            }
            set
            {
                ps = value;
            }
        }

        public PhysicsClass Class
        {
            get
            {
                return pc;
            }
        }

        public double InvisibilityTime
        {
            get { return iTime; }
            set
            {
                // This is kinda sketch maybe add an update kind of method to EntityManager.
                // Looks like LLL needs a remove method :P. Actually wait that's not so bad.
                EntityManager man = Manager;
                man.Remove(this);
                iTime = value;
                man.Add(this);
            }
        }
        public double DestructionTime { get { return dTime; } set { dTime = value; } }

        public double Time
        {
            get
            {
                return pos.Time;
            }
            set
            {
                ps.Time = value;
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

        public Entity(double cTime, Particle pos, Drawable draw, PhysicsShape physS, PhysicsClass pc, BulletEmitter e = null, GraphicsStyle g = null)
        {
            this.pc = pc;
            ps = physS ?? new Point(pos.Dimension);
            CreationTime = cTime;
            this.pos = pos;
            d = draw;
            this.e = e;
            gs = g;
        }
        public Entity(double cTime, Particle pos, PhysicsShape physS, GraphicsStyle g, PhysicsClass pc, BulletEmitter e = null)
            : this(cTime, pos, physS.MakeDrawable(g), physS, pc, e, g)
        {
        }

    }
}

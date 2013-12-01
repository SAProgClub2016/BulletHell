using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.GameLib.EventLib;

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

        private GameEvent creation, invisibility, destruction;

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
                iTime = value;
                if (invisibility==null)
                    invisibility = new GameEvent(iTime, this.MakeInvisible, this.RewindMakeInvisible, this.UndoMakeInvisible);
            }
        }
        public double DestructionTime {
            get { return dTime; }
            set {
                dTime = value;
                if (destruction==null)
                    destruction = new GameEvent(dTime, this.Destroy, this.RewindDestroy, this.UndoDestroy);
            }
        }

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

        public void Create(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            Console.WriteLine("Creating");
        }
        public void RewindCreate(Game g, GameEventState oldstate)
        {
            g.EntityManager.Remove(this);
            //Console.WriteLine(g.EntityManager.Contains(this));
            Console.WriteLine("Rewinding create");
        }
        public void UndoCreate(Game g, GameEventState oldstate)
        {
            g.EntityManager.RemovePermanently(this);
        }

        public void Destroy(Game g, GameEventState oldstate)
        {
            g.EntityManager.Remove(this);
            Console.WriteLine("Destroying");
        }
        public void RewindDestroy(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            Console.WriteLine("Rewinding destroy");
        }
        public void UndoDestroy(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            dTime = -1;
            destruction = null;
        }

        public void MakeInvisible(Game g, GameEventState oldstate)
        {
            g.EntityManager.Remove(this);
        }
        public void RewindMakeInvisible(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
        }
        public void UndoMakeInvisible(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            iTime = -1;
            invisibility = null;
        }


        public Entity(double cTime, Particle pos, Drawable draw, PhysicsShape physS, PhysicsClass pc, BulletEmitter e = null, GraphicsStyle g = null)
        {
            this.pc = pc;
            ps = physS ?? new Point(pos.Dimension);
            CreationTime = cTime;
            creation = new GameEvent(cTime, this.Create, this.RewindCreate, this.UndoCreate);
            this.pos = pos;
            d = draw;
            this.e = e;
            gs = g;
        }
        public Entity(double cTime, Particle pos, PhysicsShape physS, GraphicsStyle g, PhysicsClass pc, BulletEmitter e = null)
            : this(cTime, pos, physS.MakeDrawable(g), physS, pc, e, g)
        {
        }


        public GameEvent Creation { get { return creation; } }
        public GameEvent Destruction
        {
            get
            {
                if (destruction==null)
                    throw new InvalidOperationException();
                return destruction;
            }
        }
        public GameEvent Invisibility
        {
            get
            {
                if (invisibility==null)
                    throw new InvalidOperationException();
                return invisibility;
            }
        }

        public IEnumerable<GameEvent> LifetimeEvents
        {
            get
            {
                yield return creation;
                if (invisibility==null)
                    yield return invisibility;
                if (destruction==null)
                    yield return destruction;
            }
        }
    }
}

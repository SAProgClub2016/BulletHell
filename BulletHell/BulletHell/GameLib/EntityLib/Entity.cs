using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.GameLib.EventLib;
using BulletHell.MathLib;
using BulletHell.Physics.ShapeLib;

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

        private EntityClass pc;

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

        public EntityClass Class
        {
            get
            {
                return pc;
            }
        }

        public Id PhysicsClass
        {
            get
            {
                return pc.PhysicsClass;
            }
        }

        public Id RenderClass
        {
            get
            {
                return pc.RenderClass;
            }
        }

        private readonly double MIN_LIFE=0.01;

        public double InvisibilityTime
        {
            get { return iTime; }
            set
            {
                if (value <= CreationTime)
                    value = CreationTime + MIN_LIFE;
                iTime = value;

                if (invisibility==null)
                    invisibility = new GameEvent(iTime, this.MakeInvisible, this.RewindMakeInvisible, this.UndoMakeInvisible);
            }
        }
        public double DestructionTime {
            get { return dTime; }
            set
            {
                if (value <= CreationTime)
                    value = CreationTime + MIN_LIFE;
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
#if(DEBUG_EVENTS)
            Console.WriteLine("Create: {0}", pos.CurrentPosition);
#endif
            g.EntityManager.Add(this);
        }
        public void RewindCreate(Game g, GameEventState oldstate)
        {
#if(DEBUG_EVENTS)
            Console.WriteLine("RewindCreate");
#endif
            g.EntityManager.Remove(this);
        }
        public void UndoCreate(Game g, GameEventState oldstate)
        {
            g.EntityManager.RemovePermanently(this);
        }

        public void Destroy(Game g, GameEventState oldstate)
        {
#if(DEBUG_EVENTS)
            Console.WriteLine("Destroy: {0}", pos.CurrentPosition);
#endif
            g.EntityManager.Remove(this);
        }
        public void RewindDestroy(Game g, GameEventState oldstate)
        {
#if(DEBUG_EVENTS)
            Console.WriteLine("RewindDestroy");
#endif
            g.EntityManager.Add(this);
        }
        public void UndoDestroy(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            dTime = -1;
            destruction = null;
        }

        public void MakeInvisible(Game g, GameEventState oldstate)
        {
#if(DEBUG_EVENTS)
            Console.WriteLine("MakeInvisible: {0}",pos.CurrentPosition);
#endif
            g.EntityManager.Remove(this);
        }
        public void RewindMakeInvisible(Game g, GameEventState oldstate)
        {
#if(DEBUG_EVENTS)
            Console.WriteLine("RewindMakeInvisible");
#endif
            g.EntityManager.Add(this);
        }
        public void UndoMakeInvisible(Game g, GameEventState oldstate)
        {
            g.EntityManager.Add(this);
            iTime = -1;
            invisibility = null;
        }


        public Entity(double cTime, Particle pos, Drawable draw, PhysicsShape physS, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null)
        {
            this.pc = pc;
            ps = physS ?? new Point(pos.Dimension);
            CreationTime = cTime;
            creation = new GameEvent(cTime, this.Create, this.RewindCreate, this.UndoCreate);
            this.pos = pos;
            d = draw;
            this.e = e;
            gs = g;
            if(pos!=null)
                this.Time = CreationTime;
        }
        public Entity(double cTime, Particle pos, PhysicsShape physS, GraphicsStyle g, EntityClass pc, BulletEmitter e = null)
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

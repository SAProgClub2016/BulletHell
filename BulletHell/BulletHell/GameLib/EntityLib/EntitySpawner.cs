using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using BulletHell.Gfx;

namespace BulletHell.GameLib.EntityLib
{
    public delegate Entity EntityBuilder(double ctime, Particle pos, Drawable dr, PhysicsShape ps, EntityClass ec, BulletEmitter e=null, GraphicsStyle g=null);

    public class EntityType
    {
        Trajectory traj;
        BulletEmitter emitter;
        PhysicsShape bounds;
        EntityClass myClass;
        Drawable draw;
        GraphicsStyle gs;
        EntityBuilder builder;

        public Trajectory Trajectory
        {
            get { return traj; }
            set { traj = value; }
        }
        public BulletEmitter Emitter
        {
            get { return emitter; }
            set { emitter = value; }
        }
        public PhysicsShape Shape
        {
            get { return bounds; }
            set { bounds = value; }
        }
        public EntityClass Class
        {
            get { return myClass; }
            set { myClass = value; }
        }
        public Drawable Draw
        {
            get { return draw; }
            set { draw = value; }
        }
        public GraphicsStyle DrawStyle
        {
            get { return gs; }
            set { gs = value; }
        }
        public EntityBuilder Builder
        {
            get { return builder; }
            set { builder = value; }
        }

        public static readonly EntityBuilder DefaultEntityBuilder = (t, p, d, s, c, e, g) => { return new Entity(t, p, d, s, c, e, g); };

        public EntityType(Trajectory t, Drawable draw, PhysicsShape physS, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null,EntityBuilder b = null)
        {
            traj = t;
            this.draw = draw;
            this.bounds = physS;
            this.myClass = pc;
            emitter=e;
            gs=g;
            builder = b??DefaultEntityBuilder;
        }
        public EntityType(Trajectory t, PhysicsShape physS, GraphicsStyle g, EntityClass pc, BulletEmitter e = null, EntityBuilder b =null)
            : this(t, physS.MakeDrawable(g), physS, pc, e, null, b)
        {
        }
        public EntityType(EntityType orig)
            : this(orig.traj,orig.draw, orig.bounds, orig.myClass, orig.emitter, orig.gs,orig.builder)
        {
        }

        public Entity MakeEntity(double time, double x, double y)
        {
            return builder(time,traj(time,x,y),draw,bounds,myClass,emitter,gs);
        }

        public Entity MakeEntity(double t, Particle pos)
        {
            return builder(t, pos, draw, bounds, myClass, emitter, gs);
        }

        public EntityType ChangeEmitter(BulletEmitter e, bool clone = false)
        {
            EntityType ans=this;
            if (clone) ans = new EntityType(this);
            ans.Emitter = e;
            return ans;
        }
        public EntityType ChangeClass(EntityClass eClass, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.Class = eClass;
            return ans;
        }
        public EntityType ChangeDraw(Drawable ndraw, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.Draw = ndraw;
            return ans;
        }
        public EntityType ChangeDrawPhysShape(GraphicsStyle s, bool clone = false, PhysicsShape p = null)
        {
            p = p ?? this.Shape;
            return ChangeDraw(p.MakeDrawable(s), clone);
        }
        public EntityType ChangeShape(PhysicsShape sh, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.Shape = sh;
            return ans;
        }
        public EntityType ChangeTrajectory(Trajectory t, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.Trajectory = t;
            return ans;
        }
        public EntityType ChangeDrawStyle(GraphicsStyle sty, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.DrawStyle = sty;
            return ans;
        }
        public EntityType ChangeEntityBuilder(EntityBuilder builder, bool clone = false)
        {
            EntityType ans = this;
            if (clone) ans = new EntityType(this);
            ans.Builder = builder;
            return ans;
        }
    }
    public class EntitySpawner
    {
        private Dictionary<string, EntityType> namedTypes;

        public EntitySpawner()
        {
            namedTypes = new Dictionary<string, EntityType>();
        }

        public Entity Build(string name, double cTime, double x, double y)
        {
            return Build(namedTypes[name], cTime, x, y);
        }

        public Entity Build(EntityType type, double cTime, double x, double y)
        {
            return type.MakeEntity(cTime, x, y);
        }

        public Entity Build(string name, double cTime, Particle p)
        {
            return Build(namedTypes[name], cTime, p);
        }

        public Entity Build(EntityType type, double cTime, Particle p)
        {
            return type.MakeEntity(cTime, p);
        }

        public void MakeType(string name, Trajectory t, Drawable draw, PhysicsShape physS, EntityClass pc, BulletEmitter e = null, GraphicsStyle g = null, EntityBuilder b = null)
        {
            this[name] = new EntityType(t, draw, physS, pc, e, g, b);
        }

        public EntityType this[string name]
        {
            get
            {
                return namedTypes[name];
            }
            set
            {
                namedTypes[name] = value;
            }
        }

    }
}

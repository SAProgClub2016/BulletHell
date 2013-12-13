using BulletHell.Collections;
using BulletHell.GameLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EventLib;
using BulletHell.Physics.ShapeLib;

namespace BulletHell.Physics
{
    public delegate GameEvent OnCollision(Entity e1, Entity e2);
    public delegate GameEvent OnDisconnect(Entity e1, Entity e2);

    public struct PhysicsSet
    {
        public readonly Id Class1,Class2;
        public List<OnCollision> CollisionHandlers;
        public List<OnDisconnect> DisconnectHandlers;

        public PhysicsSet(Id c1, Id c2)
        {
            Class1 = c1;
            Class2 = c2;
            CollisionHandlers = new List<OnCollision>();
            DisconnectHandlers = new List<OnDisconnect>();
        }
    }
    public class PhysicsManager
    {
        private List<PhysicsSet> ps;
        private Dictionary<Id, LookupLinkedListSet<Entity>> ents;
        private Dictionary<Id, LinkedList<Id>> specialTypes;
        private Dictionary<Id, PhysicsShape> spTypeShapes;
        private Dictionary<Id, Id> spTypesBack;

        private Game game;

        public PhysicsManager(Game g)
        {
            game = g;
            ps = new List<PhysicsSet>();
            ents = new Dictionary<Id, LookupLinkedListSet<Entity>>();
            specialTypes = new Dictionary<Id, LinkedList<Id>>();
            spTypeShapes = new Dictionary<Id,PhysicsShape>();
            spTypesBack = new Dictionary<Id, Id>();
        }

        public void AddCollisionHandler(Id p1, Id p2, OnCollision ch)
        {
            foreach (PhysicsSet p in ps)
            {
                if ((p.Class1.Equals(p1) && p.Class2.Equals(p2)) || (p.Class2.Equals(p1) && p.Class1.Equals(p2)))
                {
                    p.CollisionHandlers.Add(ch);
                    return;
                }
            }
            if (!ents.ContainsKey(p1))
                ents[p1] = new LookupLinkedListSet<Entity>();
            if (!ents.ContainsKey(p2))
                ents[p2] = new LookupLinkedListSet<Entity>();
            PhysicsSet newset = new PhysicsSet(p1, p2);
            newset.CollisionHandlers.Add(ch);
            ps.Add(newset);
        }
        public void AddDisconnectHandler(Id p1, Id p2, OnDisconnect ch)
        {
            foreach (PhysicsSet p in ps)
            {
                if ((p.Class1.Equals(p1) && p.Class2.Equals(p2)) || (p.Class2.Equals(p1) && p.Class1.Equals(p2)))
                {
                    p.DisconnectHandlers.Add(ch);
                    return;
                }
            }
            if (!ents.ContainsKey(p1))
                ents[p1] = new LookupLinkedListSet<Entity>();
            if (!ents.ContainsKey(p2))
                ents[p2] = new LookupLinkedListSet<Entity>();
            PhysicsSet newset = new PhysicsSet(p1, p2);
            newset.DisconnectHandlers.Add(ch);
            ps.Add(newset);
        }

        public void Add(Entity e)
        {
            Id id = e.PhysicsClass;
            Add(id, e);
            if (specialTypes.ContainsKey(id))
            {
                foreach (Id i in specialTypes[id])
                {
                    Add(i, e);
                }
            }
            
            /*
            if(ents.ContainsKey(e.PhysicsClass))
            {
                ents[e.PhysicsClass].Add(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                lls.Add(e);
                ents[e.PhysicsClass] = lls;
            }*/
        }
        public void Add(Id id, Entity e)
        {
            try
            {
                ents[id].Add(e);
            }
            catch (Exception ex)
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                lls.Add(e);
                ents[id] = lls;
            }
        }

        public void Remove(Entity e)
        {
            Id id = e.PhysicsClass;
            Remove(id, e);
            if (specialTypes.ContainsKey(id))
            {
                foreach (Id i in specialTypes[id])
                {
                    Remove(i, e);
                }
            }
        }

        public void Remove(Id id, Entity e)
        {

            try
            {
                ents[id].Remove(e);
            }
            catch (Exception ex) // find the actual exception
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[id] = lls;
            }
            /*if(ents.ContainsKey(e.PhysicsClass))
            {
                ents[e.PhysicsClass].Remove(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.PhysicsClass] = lls;
            }*/
        }

        public void Collisions()
        {
            foreach(PhysicsSet p in ps)
            {
                IEnumerable<Tuple<Entity,PhysicsShape>> q1 = MakeEntityShapeTuple(p.Class1), q2 = MakeEntityShapeTuple(p.Class2);
                foreach(Tuple<Entity,PhysicsShape> e1 in q1)
                {
                    foreach (Tuple<Entity, PhysicsShape> e2 in q2)
                    {
                        if(e1.Item2.Meets(e1.Item1.Position,e2.Item2,e2.Item1.Position))
                        {
                            foreach(OnCollision o in p.CollisionHandlers)
                            {
                                GameEvent gev = o(e1.Item1, e2.Item1);
                                if (gev != null)
                                    game.Events.Add(gev);
                            }
                        }
                        else
                        {
                            foreach(OnDisconnect o in p.DisconnectHandlers)
                            {
                                GameEvent gev = o(e1.Item1, e2.Item1);
                                if (gev != null)
                                    game.Events.Add(gev);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Tuple<Entity,PhysicsShape>> MakeEntityShapeTuple(Id id)
        {
            if (spTypesBack.ContainsKey(id))
            {
                return PairEntitiesWithShape(ents[id],spTypeShapes[id]);
            }
            return PairEntitiesWithOwnShape(ents[id]);
        }

        private IEnumerable<Tuple<Entity, PhysicsShape>> PairEntitiesWithShape(IEnumerable<Entity> es, PhysicsShape physicsShape)
        {
            foreach (Entity e in es)
            {
                yield return new Tuple<Entity, PhysicsShape>(e, physicsShape);
            }
        }
        private IEnumerable<Tuple<Entity, PhysicsShape>> PairEntitiesWithOwnShape(IEnumerable<Entity> es)
        {
            foreach (Entity e in es)
            {
                yield return new Tuple<Entity, PhysicsShape>(e, e.Shape);
            }
        }

        public void RemovePermanently(Entity e)
        {
            Id id = e.PhysicsClass;
            RemovePermanently(id, e);
            if (specialTypes.ContainsKey(id))
            {
                foreach (Id i in specialTypes[id])
                {
                    RemovePermanently(i, e);
                }
            }
        }

        public void RemovePermanently(Id id,Entity e)
        {

            try
            {
                ents[id].RemovePermanently(e);
            }
            catch (Exception ex) // find the actual exception
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[id] = lls;
            }
        }

        internal void RegisterClassShape(Id newClass, Id oldClass, PhysicsShape classShape)
        {
            if (specialTypes.ContainsKey(oldClass))
                specialTypes[oldClass].AddLast(newClass);
            else
            {
                LinkedList<Id> ids = new LinkedList<Id>();
                ids.AddLast(newClass);
                specialTypes[oldClass] = ids;
            }
            spTypesBack[newClass] = oldClass;
            spTypeShapes[newClass] = classShape;
        }
    }
}

using BulletHell.Collections;
using BulletHell.GameLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EventLib;

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
        private Game game;

        public PhysicsManager(Game g)
        {
            game = g;
            ps = new List<PhysicsSet>();
            ents = new Dictionary<Id, LookupLinkedListSet<Entity>>();
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
            if(ents.ContainsKey(e.PhysicsClass))
            {
                ents[e.PhysicsClass].Add(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                lls.Add(e);
                ents[e.PhysicsClass] = lls;
            }
        }

        public void Remove(Entity e)
        {
            if(ents.ContainsKey(e.PhysicsClass))
            {
                ents[e.PhysicsClass].Remove(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.PhysicsClass] = lls;
            }
        }

        public void Collisions()
        {
            foreach(PhysicsSet p in ps)
            {
                LookupLinkedListSet<Entity> q1 = ents[p.Class1], q2 = ents[p.Class2];
                foreach(Entity e1 in q1)
                {
                    foreach(Entity e2 in q2)
                    {
                        if(e1.Shape.Meets(e1.Position,e2.Shape,e2.Position))
                        {
                            foreach(OnCollision o in p.CollisionHandlers)
                            {
                                GameEvent gev = o(e1, e2);
                                if (gev != null)
                                    game.Events.Add(gev);
                            }
                        }
                        else
                        {
                            foreach(OnDisconnect o in p.DisconnectHandlers)
                            {
                                GameEvent gev = o(e1, e2);
                                if (gev != null)
                                    game.Events.Add(gev);
                            }
                        }
                    }
                }
            }
        }

        public void RemovePermanently(Entity e)
        {

            if (ents.ContainsKey(e.PhysicsClass))
            {
                ents[e.PhysicsClass].RemovePermanently(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.PhysicsClass] = lls;
            }
        }
    }
}

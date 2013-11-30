using BulletHell.Collections;
using BulletHell.GameLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Physics
{
    public delegate void OnCollision(Entity e1, Entity e2);
    public delegate void OnDisconnect(Entity e1, Entity e2);

    public struct PhysicsSet
    {
        public readonly PhysicsClass Class1,Class2;
        public List<OnCollision> CollisionHandlers;
        public List<OnDisconnect> DisconnectHandlers;

        public PhysicsSet(PhysicsClass c1, PhysicsClass c2)
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
        private Dictionary<PhysicsClass, LookupLinkedListSet<Entity>> ents;

        public PhysicsManager()
        {
            ps = new List<PhysicsSet>();
            ents = new Dictionary<PhysicsClass, LookupLinkedListSet<Entity>>();
        }

        public void AddCollisionHandler(PhysicsClass p1, PhysicsClass p2, OnCollision ch)
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
        public void AddDisconnectHandler(PhysicsClass p1, PhysicsClass p2, OnDisconnect ch)
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
            if(ents.ContainsKey(e.Class))
            {
                ents[e.Class].Add(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                lls.Add(e);
                ents[e.Class] = lls;
            }
        }

        public void Remove(Entity e)
        {
            if(ents.ContainsKey(e.Class))
            {
                ents[e.Class].Remove(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.Class] = lls;
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
                                o(e1, e2);
                            }
                        }
                        else
                        {
                            foreach(OnDisconnect o in p.DisconnectHandlers)
                            {
                                o(e1, e2);
                            }
                        }
                    }
                }
            }
        }
    }
}

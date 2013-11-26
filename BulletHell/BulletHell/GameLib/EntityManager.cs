using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Collections;

namespace BulletHell.GameLib
{
    public interface EntityManager
    {
        void Add(Entity e);
        void Remove(Entity e);
        IEnumerable<Entity> Entities(double t);
        IEnumerable<Entity> BulletShooters(double Time);
    }
    public class ListEntityManager : EntityManager
    {
        List<Entity> entities;
        public ListEntityManager()
        {
            entities = new List<Entity>();
        }
        public void Add(Entity e)
        {
            e.Manager = this;
            entities.Add(e);
        }
        public void Remove(Entity e)
        {
            e.Manager = null;
            entities.Remove(e);
        }
        public IEnumerable<Entity> Entities(double t)
        {
            return entities;
        }

        public IEnumerable<Entity> BulletShooters(double t)
        {
            return entities;
        }
    }
    public class AdvancedEntityManager : EntityManager
    {
        private LayeredLinkedList<Entity> adds, removals;
        private Dictionary<Entity, Reference<LinkedListNode<Entity>>> map;
        private Dictionary<Entity, Reference<LinkedListNode<Entity>>> bsmap;
        //Dictionary<Entity, bool> contains;
        private LinkedList<Entity> entities;
        private LinkedList<Entity> bulletShooters;
        private double oldTime;

        public AdvancedEntityManager(params double[] ints)
        {
            oldTime = 0;
            adds = new LayeredLinkedList<Entity>(0, x => x.CreationTime, ints);
            removals = new LayeredLinkedList<Entity>(0, x => x.InvisibilityTime, ints);
            entities = new LinkedList<Entity>();
            bulletShooters = new LinkedList<Entity>();
            map = new Dictionary<Entity, Reference<LinkedListNode<Entity>>>();
            bsmap = new Dictionary<Entity, Reference<LinkedListNode<Entity>>>();
        }

        public void Add(Entity e)
        {
            if (e.Manager == this)
            {
                return;
            }
            e.Manager = this;
            Reference<LinkedListNode<Entity>> enode = new Reference<LinkedListNode<Entity>>(new LinkedListNode<Entity>(e));
            Reference<LinkedListNode<Entity>> enode2 = new Reference<LinkedListNode<Entity>>(new LinkedListNode<Entity>(e));
            if (!map.ContainsKey(e))
            {
                map.Add(e, enode);
                if(e.Emitter!=null)
                    bsmap.Add(e, enode2);
            }
            if (e.CreationTime < oldTime && (e.InvisibilityTime == -1 || e.InvisibilityTime > oldTime))
            {
                ProcessAddition(e);
            }
            adds.Add(e);
            removals.Add(e);
        }
        private void ProcessAddition(Entity e)
        {
            Reference<LinkedListNode<Entity>> enode, bnode=null;
            enode = map[e];
            if (e.Emitter != null)
            {
                bnode = bsmap[e];
            }
            //if(enode.Value.List)
            LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(e);
            //enode.Value = newNode;
            if(enode.Value.List==null)
                entities.AddLast(enode.Value);
            if (bnode != null && bnode.Value.List==null)
            {
                newNode = new LinkedListNode<Entity>(e);
                bnode.Value = newNode;
                bulletShooters.AddLast(bnode.Value);
            }
        }
        private void ProcessRemoval(Entity e)
        {
            LinkedListNode<Entity> enode, bnode=null;
            enode = map[e];
            if (e.Emitter != null)
            {
                bnode = bsmap[e];
            }
            if (enode.List == entities)
                entities.Remove(enode);
            if (bnode != null && bnode.List == bulletShooters)
                bulletShooters.Remove(bnode);
        }

        public void Remove(Entity e)
        {
            if (e.Manager != this)
                return;
            e.Manager = null;
            if (e.CreationTime > oldTime || (e.InvisibilityTime != -1 && e.InvisibilityTime < oldTime))
            {
                ProcessRemoval(e);
            }
            adds.Remove(e);
            removals.Remove(e);
        }

        private static readonly double TOLERANCE = 0.01;

        private void Update(double t)
        {
            if (Utils.IsZero(t - oldTime))
                return;
            if (t < oldTime)
            {
                foreach (Entity e in adds.ElementsBetween(t-TOLERANCE, oldTime+TOLERANCE))
                {
                    ProcessRemoval(e);
                }
                foreach (Entity e in removals.ElementsBetween(t-TOLERANCE, oldTime+TOLERANCE))
                {
                    ProcessAddition(e);
                }
            }
            else
            {
                foreach (Entity e in adds.ElementsBetween(oldTime-TOLERANCE, t+TOLERANCE))
                {
                    ProcessAddition(e);
                }
                foreach (Entity e in removals.ElementsBetween(oldTime-TOLERANCE, t+TOLERANCE))
                {
                    ProcessRemoval(e);
                }
            }
            oldTime = t;
        }

        private void DebugUpdate(double t)
        {
            BulletHell.Time.Timer testtimer = new BulletHell.Time.Timer();
            if (Utils.IsZero(t - oldTime))
                return;
            Console.WriteLine("AdvancedEntityManager.Update: oldTime={0}, newTime={1}", oldTime, t);
            int count = 0;
            if (t < oldTime)
            {
                Console.WriteLine("Going Back");
                testtimer.Reset();
                foreach (Entity e in adds.ElementsBetween(t - TOLERANCE, oldTime + TOLERANCE))
                {
                    count++;
                    ProcessRemoval(e);
                }
                Console.WriteLine("AdvancedEntityManager.Update: count={1}, time={0}", testtimer.Time, count);
                count = 0;
                testtimer.Reset();
                foreach (Entity e in removals.ElementsBetween(t - TOLERANCE, oldTime + TOLERANCE))
                {
                    ProcessAddition(e);
                    count++;
                }
                Console.WriteLine("AdvancedEntityManager.Update: count={1}, time={0}", testtimer.Time, count);
            }
            else
            {
                Console.WriteLine("Going forward");
                testtimer.Reset();

                foreach (Entity e in adds.ElementsBetween(oldTime - TOLERANCE, t + TOLERANCE))
                {
                    ProcessAddition(e);
                    count++;
                }
                foreach (Entity e in removals.ElementsBetween(oldTime - TOLERANCE, t + TOLERANCE))
                {
                    ProcessRemoval(e);
                    count++;
                }
                Console.WriteLine("AdvancedEntityManager.Update: count={1}, time={0}", testtimer.Time, count);
                count = 0;
                testtimer.Reset();
                Console.WriteLine("AdvancedEntityManager.Update: count={1}, time={0}", testtimer.Time, count);
            }
            oldTime = t;
            Console.WriteLine("AdvancedEntityManager.Update: Entities size={0}", entities.Count());
        }

        public IEnumerable<Entity> Entities(double t)
        {
            Update(t);
            return entities;
        }
        public IEnumerable<Entity> BulletShooters(double t)
        {
            Update(t);
            return bulletShooters;
        }
    }
}

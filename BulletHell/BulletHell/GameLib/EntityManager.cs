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
    }
    public class AdvancedEntityManager : EntityManager
    {
        LayeredLinkedList<Entity> adds, removals;
        Dictionary<Entity, Reference<LinkedListNode<Entity>>> map;
        System.Collections.Generic.LinkedList<Entity> entities;
        double oldTime;

        public AdvancedEntityManager(params double[] ints)
        {
            oldTime = 0;
            adds = new LayeredLinkedList<Entity>(0, x => x.CreationTime, ints);
            removals = new LayeredLinkedList<Entity>(0, x => x.InvisibilityTime, ints);
            entities = new System.Collections.Generic.LinkedList<Entity>();
            map = new Dictionary<Entity, Reference<LinkedListNode<Entity>>>();
        }

        public void Add(Entity e)
        {
            e.Manager = this;
            Reference<LinkedListNode<Entity>> enode = new Reference<LinkedListNode<Entity>>(new LinkedListNode<Entity>(e));
            if (!map.ContainsKey(e))
                map.Add(e, enode);
            if (e.CreationTime < oldTime && (e.InvisibilityTime == -1 || e.InvisibilityTime > oldTime))
            {
                entities.AddLast(enode);
            }
            adds.Add(e);
            removals.Add(e);
        }


        public void Remove(Entity e)
        {
            e.Manager = null;
            if (e.CreationTime > oldTime || (e.InvisibilityTime != -1 && e.InvisibilityTime < oldTime))
            {
                LinkedListNode<Entity> enode = map[e];
                if(enode.List==entities)
                    entities.Remove(enode);
            }
            adds.Remove(e);
            removals.Remove(e);
        }

        public IEnumerable<Entity> Entities(double t)
        {
            BulletHell.Time.Timer testtimer = new BulletHell.Time.Timer();
            if(Utils.IsZero(t-oldTime))
                return entities;
            int count = 0;
            if (t < oldTime)
            {
                Console.WriteLine("Going Back");
                testtimer.Reset();
                foreach (Entity e in adds.ElementsBetween(t, oldTime))
                {
                    count++;
                    LinkedListNode<Entity> enode = map[e];
                    if (enode.List == entities)
                        entities.Remove(enode);
                }
                Console.WriteLine("AdvancedEntityManager.Entities: count={1}, time={0}",testtimer.Time,count);
                count = 0;
                testtimer.Reset();
                foreach (Entity e in removals.ElementsBetween(t, oldTime))
                {
                    Reference<LinkedListNode<Entity>> enode = map[e];
                    LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(enode.Value.Value);
                    enode.Value = newNode;
                    entities.AddLast(enode.Value);
                    count++;
                }
                Console.WriteLine("AdvancedEntityManager.Entities: count={1}, time={0}", testtimer.Time, count);
            }
            else
            {
                Console.WriteLine("Going forward");
                testtimer.Reset();

                foreach (Entity e in adds.ElementsBetween(oldTime, t))
                {
                    Reference<LinkedListNode<Entity>> enode = map[e];
                    LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(e);
                    enode.Value = newNode;
                    entities.AddLast(enode.Value);
                    count++;
                }
                foreach (Entity e in removals.ElementsBetween(oldTime,t))
                {
                    LinkedListNode<Entity> enode = map[e];
                    if (enode.List == entities)
                        entities.Remove(enode);
                    count++;
                }
                Console.WriteLine("AdvancedEntityManager.Entities: count={1}, time={0}", testtimer.Time, count);
                count = 0;
                testtimer.Reset();
                /*foreach (LinkedListNode<LinkedListNode<Entity>> enode in adds.LLNElementsBetween(t, oldTime))
                {
                    LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(enode.Value.Value);
                    enode.Value = newNode;
                    entities.AddLast(newNode);
                    count++;
                }*/
                Console.WriteLine("AdvancedEntityManager.Entities: count={1}, time={0}", testtimer.Time, count);
            }
            oldTime = t;
            return entities;
        }
    }
}

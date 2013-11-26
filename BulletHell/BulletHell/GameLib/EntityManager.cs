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
            entities.Add(e);
        }
        public void Remove(Entity e)
        {
            entities.Remove(e);
        }
        public IEnumerable<Entity> Entities(double t)
        {
            return entities;
        }
    }
    public class AdvancedEntityManager : EntityManager
    {
        LayeredLinkedList<Reference<LinkedListNode<Entity>>> adds, removals;
        System.Collections.Generic.LinkedList<Entity> entities;
        double oldTime;

        public AdvancedEntityManager(params double[] ints)
        {
            oldTime = 0;
            adds = new LayeredLinkedList<Reference<LinkedListNode<Entity>>>(0, x => x.Value.Value.CreationTime, ints);
            removals = new LayeredLinkedList<Reference<LinkedListNode<Entity>>>(0, x => x.Value.Value.InvisibilityTime, ints);
            entities = new System.Collections.Generic.LinkedList<Entity>();
        }

        public void Add(Entity e)
        {
            //Console.WriteLine(e);
            LinkedListNode<Entity> enode = new LinkedListNode<Entity>(e);
            if (e.CreationTime < oldTime && (e.InvisibilityTime == -1) || e.InvisibilityTime > oldTime)
            {
                entities.AddLast(enode);
                //Console.WriteLine("Q");
            }
            adds.Add(enode);
            removals.Add(enode);
            //adds.Write();
            //removals.Write();
            //Console.ReadKey();
        }

        public void Remove(Entity e)
        {
            throw new NotImplementedException();
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
                foreach (LinkedListNode<Entity> enode in adds.ElementsBetween(t, oldTime))
                {
                    count++;
                    if (enode.List == entities)
                        entities.Remove(enode);
                }
                Console.WriteLine("AdvancedEntityManager.Entities: count={1}, time={0}",testtimer.Time,count);
                count = 0;
                testtimer.Reset();
                /*foreach (LinkedListNode<LinkedListNode<Entity>> enode in removals.LLNElementsBetween(t, oldTime))
                {
                    LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(enode.Value.Value);
                    enode.Value = newNode;
                    entities.AddLast(newNode);
                    count++;
                }*/
                foreach (Reference<LinkedListNode<Entity>> enode in removals.ElementsBetween(t, oldTime))
                {
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

                foreach (LinkedListNode<Entity> enode in adds.ElementsBetween(oldTime, t))
                {
                    //LinkedListNode<Entity> newNode = new LinkedListNode<Entity>(enode.Value.Value);
                    //enode.Value = newNode;
                    entities.AddLast(enode.Value);
                    count++;
                }
                foreach (LinkedListNode<Entity> enode in removals.ElementsBetween(oldTime,t))
                {
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

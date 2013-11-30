using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Collections;
using BulletHell.Physics;

namespace BulletHell.GameLib.EntityLib
{
    public interface EntityManager
    {
        void Add(Entity e);
        void RemovePermanently(Entity e);
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


        public void RemovePermanently(Entity e)
        {
            e.Manager = null;
            entities.Remove(e);
        }
    }
    public class AdvancedEntityManager : EntityManager
    {
        private LayeredLinkedList<Entity> adds, removals;
        //Dictionary<Entity, bool> contains;
        private LookupLinkedListSet<Entity> entities;
        private LookupLinkedListSet<Entity> bulletShooters;
        private PhysicsManager pm;

        private double oldTime;

        public AdvancedEntityManager(PhysicsManager phys,params double[] ints)
        {
            pm = phys;
            oldTime = 0;
            adds = new LayeredLinkedList<Entity>(0, x => x.CreationTime, ints);
            removals = new LayeredLinkedList<Entity>(0, x => x.InvisibilityTime, ints);
            entities = new LookupLinkedListSet<Entity>();
            bulletShooters = new LookupLinkedListSet<Entity>();
        }

        public void Add(Entity e)
        {
            if (e.Manager == this)
            {
                return;
            }
            e.Manager = this;
            if (e.CreationTime < oldTime && (e.InvisibilityTime == -1 || e.InvisibilityTime > oldTime))
            {
                ProcessAddition(e);
            }
            adds.Add(e);
            removals.Add(e);
        }
        private void ProcessAddition(Entity e)
        {
            pm.Add(e);
            entities.Add(e);
            if (e.Emitter != null)
            {
                bulletShooters.Add(e);
            }
        }
        private void ProcessRemoval(Entity e)
        {
            pm.Remove(e);
            entities.Remove(e);
            if(e.Emitter!=null)
                bulletShooters.Remove(e);
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


        public void RemovePermanently(Entity e)
        {
            if (e.Manager != this)
                return;
            e.Manager = null;
            if (e.CreationTime > oldTime || (e.InvisibilityTime != -1 && e.InvisibilityTime < oldTime))
            {
                ProcessPermanentRemoval(e);
            }
            adds.Remove(e);
            removals.Remove(e);
        }

        private void ProcessPermanentRemoval(Entity e)
        {
            pm.RemovePermanently(e);
            entities.RemovePermanently(e);
            if (e.Emitter != null)
                bulletShooters.RemovePermanently(e);
        }
    }
}

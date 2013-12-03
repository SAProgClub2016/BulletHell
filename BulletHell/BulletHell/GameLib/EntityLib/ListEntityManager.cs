using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
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
        public bool Contains(Entity entity)
        {
            return entities.Contains(entity);
        }
    }
}

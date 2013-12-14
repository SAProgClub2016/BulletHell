using BulletHell.Collections;
using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Gfx
{
    public class RenderManager : IEnumerable<Entity>
    {
        public LinkedList<Id> RenderOrder;

        private Dictionary<Id, LookupLinkedListSet<Entity>> ents;

        public RenderManager()
        {
            RenderOrder = new LinkedList<Id>();
            ents = new Dictionary<Id, LookupLinkedListSet<Entity>>();
        }
        public void Add(Entity e)
        {
            LookupLinkedListSet<Entity> es = null;
            if(ents.TryGetValue(e.RenderClass, out es))
            {
                es.Add(e);
            }
            else
            {
                es = new LookupLinkedListSet<Entity>();
                es.Add(e);
                ents[e.RenderClass] = es;
            }
        }
        public void Remove(Entity e)
        {
            LookupLinkedListSet<Entity> es = null;
            if (ents.TryGetValue(e.RenderClass, out es))
            {
                es.Remove(e);
            }
            else
            {
                es = new LookupLinkedListSet<Entity>();
                ents[e.RenderClass] = es;
            }
        }

        public void RemovePermanently(Entity e)
        {
            LookupLinkedListSet<Entity> es = null;
            if (ents.TryGetValue(e.RenderClass, out es))
            {
                es.RemovePermanently(e);
            }
            else
            {
                es = new LookupLinkedListSet<Entity>();
                ents[e.RenderClass] = es;
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            foreach(Id i in RenderOrder)
            {
                LookupLinkedListSet<Entity> es = null;
                if (ents.TryGetValue(i,out es))
                {
                    foreach (Entity e in es)
                        yield return e;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (Id i in RenderOrder)
            {
                LookupLinkedListSet<Entity> es = null;
                if (ents.TryGetValue(i, out es))
                {
                    foreach (Entity e in es)
                        yield return e;
                }
            }
        }
    }
}

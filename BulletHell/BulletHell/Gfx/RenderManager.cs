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
            //Console.WriteLine(e.RenderClass);
            if(ents.ContainsKey(e.RenderClass))
            {
                ents[e.RenderClass].Add(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                lls.Add(e);
                ents[e.RenderClass] = lls;
            }
        }
        public void Remove(Entity e)
        {
            if(ents.ContainsKey(e.RenderClass))
            {
                ents[e.RenderClass].Remove(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.RenderClass] = lls;
            }
        }

        public void RemovePermanently(Entity e)
        {

            if (ents.ContainsKey(e.RenderClass))
            {
                ents[e.RenderClass].RemovePermanently(e);
            }
            else
            {
                LookupLinkedListSet<Entity> lls = new LookupLinkedListSet<Entity>();
                ents[e.RenderClass] = lls;
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            foreach(Id i in RenderOrder)
            {
                if(ents.ContainsKey(i))
                {
                    foreach (Entity e in ents[i])
                        yield return e;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (Id i in RenderOrder)
            {
                if (ents.ContainsKey(i))
                {
                    foreach (Entity e in ents[i])
                        yield return e;
                }
            }
        }
    }
}

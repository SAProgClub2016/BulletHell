using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Collections
{
    class LookupLinkedListSet<T> : IEnumerable<T>
    {
        private LinkedList<T> ll;
        private Dictionary<T, LinkedListNode<T>> dict;

        public LookupLinkedListSet()
        {
            ll = new LinkedList<T>();
            dict = new Dictionary<T, LinkedListNode<T>>();
        }

        public void Add(T t)
        {
            LinkedListNode<T> node = null;
            if(!dict.TryGetValue(t, out node))
            {
                node = new LinkedListNode<T>(t);
                dict[t] = node;
            }
            if (node.List == null)
                ll.AddLast(node);
        }

        public bool Contains(T t)
        {
            return dict.ContainsKey(t) && dict[t].List == ll;
        }

        public void Remove(T t)
        {
            if(this.Contains(t))
            {
                ll.Remove(dict[t]);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ll.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ll.GetEnumerator();
        }

        public int Count()
        {
            return ll.Count();
        }

        public void RemovePermanently(T t)
        {
            LinkedListNode<T> lln = null;
            if(dict.TryGetValue(t, out lln))
            {
                if (lln.List != null)
                    ll.Remove(lln);
                dict.Remove(t);
            }
        }
    }

    public class LookupLinkedListSetTest
    {
        public static void Main()
        {
            LookupLinkedListSet<int> ints = new LookupLinkedListSet<int>();

            long thous = 1000;
            long million = thous * thous;
            long billion = thous * million;
            long trillion = million * million;
            Random r = new Random();
            int range = 10;
            for (long i = 0; i < 20 * thous; i++)
            {
                int rand = r.Next(range);
                int rand2 = r.Next(range);
                //Pair<int> ri = new Pair<int>(rand, rand2);
                ints.Add(rand);
                ints.Remove(rand2);
            }
            foreach (int i in ints)
                Console.WriteLine(i);
            Console.WriteLine("--------------------");
            Console.WriteLine(ints.Count());
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Needs a remove/reorganize method. Entities should know which entity manager they belong to, so they can update when properties are changed.

namespace BulletHell.Collections
{
    class LayeredLinkedList<T>
    {
        double start;
        LinkedList<T> vals;
        LinkedList<LayeredLinkedList<T>> nextLevel;
        int layers;
        Func<T,double> evaluator;
        double interval;
        double[] nextInts;

        public LayeredLinkedList(double s, Func<T, double> evaluator, params double[] intervals)
        {
            this.evaluator = evaluator;
            start = s;
            layers = intervals.Length;
            interval = 0;
            if (layers == 0)
            {
                vals = new LinkedList<T>();
                nextLevel = null;
            }
            else
            {
                vals = null;
                interval = intervals[0];
                nextInts = new double[layers - 1];
                for(int i = 1; i< layers; i++)
                {
                    nextInts[i-1] = intervals[i];
                }
                nextLevel = new LinkedList<LayeredLinkedList<T>>();
            }
        }

        public bool Remove(T t)
        {
            return Remove(evaluator(t), t);
        }
        private bool Remove(double d, T t)
        {
            if (layers == 0)
                return vals.Remove(t);
            else
            {
                if (d < start)
                {
                    return false;
                }
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;
                if (node == null)
                {
                    return false;
                }
                while (d > node.Value.start + interval)
                {
                    if (node.Next == null)
                    {
                        return false;
                    }
                    node = node.Next;
                }
                return node.Value.Remove(d, t);
            }
        }

        public void Add(T t)
        {
            Add(evaluator(t), t);
        }
        private void Add(double d, T t)
        {
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.First;
                if (node == null)
                {
                    vals.AddLast(t);
                }
                else
                {
                    double cur = evaluator(node.Value);
                    if (d <= cur)
                        vals.AddFirst(t);
                    else
                    {
                        while (d > cur)
                        {
                            node = node.Next;
                            if (node == null)
                                break;
                            cur = evaluator(node.Value);
                        }
                        if (node == null)
                        {
                            vals.AddLast(t);
                        }
                        else
                        {
                            vals.AddBefore(node,t);
                        }
                    }
                }
            }
            else
            {
                while (d < start)
                {
                    start-=interval;
                    nextLevel.AddFirst(new LayeredLinkedList<T>(start, evaluator, nextInts));
                }
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;
                if (node == null)
                {
                    nextLevel.AddLast(new LayeredLinkedList<T>(start, evaluator, nextInts));
                    node = nextLevel.First;
                }
                while (d > node.Value.start+interval)
                {
                    if(node.Next == null)
                    {
                        nextLevel.AddLast(new LayeredLinkedList<T>(node.Value.start+interval,evaluator,nextInts));
                    }
                    node = node.Next;
                }
                node.Value.Add(d, t);
            }
        }
        public IEnumerable<T> ElementsBetween(double t1, double t2)
        {
            //System.Environment.Exit(0);
            if (t2 < start)
            {
                yield break;
            }
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.First;
                while (node != null && evaluator(node.Value) < t1)
                {
                    node = node.Next;
                }
                if (node == null)
                {
                    yield break;
                }
                while (node != null && evaluator(node.Value) < t2)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;

                while (node != null && node.Value.start+interval < t1)
                {
                    node = node.Next;
                }
                if (node == null)
                {
                    yield break;
                }
                while (node != null && node.Value.start < t2)
                {
                    foreach(T val in node.Value.ElementsBetween(t1,t2))
                        yield return val;
                    node = node.Next;
                }
            }
        }
        public void Write()
        {
            Console.WriteLine("LLL: layers={0}, start={1}, interval={2}", layers, start, interval);
            if (layers == 0)
            {
                foreach (T t in vals)
                {
                    Console.Write("{0}, ", t);
                }
                Console.WriteLine();
            }
            else
            {
                foreach (LayeredLinkedList<T> l in nextLevel)
                {
                    l.Write();
                }
            }
        }

        internal IEnumerable<T> ElementsAfter(double time)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<T> ElementsBetweenBackwards(double t1, double t2)
        {
            //System.Environment.Exit(0);
            if (t2 < start)
            {
                yield break;
            }
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.Last;
                while (node != null && evaluator(node.Value) > t2)
                {
                    node = node.Previous;
                }
                if (node == null)
                {
                    yield break;
                }
                while (node != null && evaluator(node.Value) > t1)
                {
                    yield return node.Value;
                    node = node.Previous;
                }
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.Last;

                while (node != null && node.Value.start > t2)
                {
                    node = node.Previous;
                }
                if (node == null)
                {
                    yield break;
                }
                while (node != null && node.Value.start + interval > t1)
                {
                    foreach (T val in node.Value.ElementsBetweenBackwards(t1, t2))
                        yield return val;
                    node = node.Previous;
                }
            }
        }
    }
    public class LayeredLinkedListTest
    {
        public static void Main()
        {
            LayeredLinkedList<double> lll = new LayeredLinkedList<double>(0, Utils.Identity<double>);//,1000,100,10,1);

            BulletHell.Time.Timer time = new BulletHell.Time.Timer();
            Random r= new Random();

            time.Reset();
            long thous = 1000;
            long million = thous*thous;
            long billion = thous*million;
            long trillion = million * million;
            for (long i = 0; i < 20* thous; i++)
            {
                lll.Add(1000*r.NextDouble());
            }
            Console.WriteLine(time.Time);
            //lll.Write();
            Console.WriteLine("-------------------------------------------------");
            time.Reset();
            int count = 0;
            foreach (double d in lll.ElementsBetween(30, 40))
            {
                count++;
            }
            Console.WriteLine(count);
            count = 0;
            foreach (double d in lll.ElementsBetween(-100, 5000))
            {
                if(30<d&&d<40) count++;
            }
            Console.WriteLine(count);
            Console.WriteLine(time.Time);
            Console.ReadKey();
        }
    }
}

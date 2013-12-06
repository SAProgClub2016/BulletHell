using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// Needs a remove/reorganize method. Entities should know which entity manager they belong to, so they can update when properties are changed.

namespace BulletHell.Collections
{

    public class LayeredLinkedList<T>
    {
        double start;
        LinkedList<T> vals;
        LinkedList<LayeredLinkedList<T>> nextLevel;
        int layers;
        Func<T, double> evaluator;
        double interval;
        double[] nextInts;
        private int count = 0;

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
                for (int i = 1; i < layers; i++)
                {
                    nextInts[i - 1] = intervals[i];
                }
                nextLevel = new LinkedList<LayeredLinkedList<T>>();
            }
        }

        public int Layers
        {
            get
            {
                return layers;
            }
        }

        public bool Remove(T t)
        {
            return Remove(evaluator(t), t);
        }
        private bool RemoveHelper(double d, T t)
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
        private bool Remove(double d, T t)
        {
            bool ans = RemoveHelper(d, t);
            if (ans) count--;
            return ans;
        }

        public void Add(T t)
        {
            Add(evaluator(t), t);
        }
        private void Add(double d, T t)
        {
            count++;
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
                            vals.AddBefore(node, t);
                        }
                    }
                }
            }
            else
            {
                while (d < start)
                {
                    start -= interval;
                    nextLevel.AddFirst(new LayeredLinkedList<T>(start, evaluator, nextInts));
                }
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;
                if (node == null)
                {
                    nextLevel.AddLast(new LayeredLinkedList<T>(start, evaluator, nextInts));
                    node = nextLevel.First;
                }
                while (d > node.Value.start + interval)
                {
                    if (node.Next == null)
                    {
                        nextLevel.AddLast(new LayeredLinkedList<T>(node.Value.start + interval, evaluator, nextInts));
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

                while (node != null && node.Value.start + interval < t1)
                {
                    node = node.Next;
                }
                if (node == null)
                {
                    yield break;
                }
                while (node != null && node.Value.start < t2)
                {
                    foreach (T val in node.Value.ElementsBetween(t1, t2))
                        yield return val;
                    node = node.Next;
                }
            }
        }

        // if there is nothing before d, returns the first element. might have it throw an exception soon, then you check it and get the first element yourself.
        public T LastBefore(double d)
        {
            if (Count() == 0)
                throw new InvalidOperationException("No elements to return");
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.Last;
                while (node != null && evaluator(node.Value) > d) node = node.Previous;
                if (node == null)
                    return First();
                return node.Value;
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.Last;
                while (node != null && (node.Value.start > d || node.Value.Count() == 0)) node = node.Previous;
                if (node == null)
                    return First();
                return node.Value.LastBefore(d);
            }
        }

        public T FirstAfter(double d)
        {
            if (Count() == 0)
                throw new InvalidOperationException("No elements to return");
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.First;
                while (node != null && evaluator(node.Value) < d) node = node.Next;
                if (node == null)
                    return Last();
                return node.Value;
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;
                while (node != null && (node.Value.start + interval < d || node.Value.Count() == 0)) node = node.Next;
                if (node == null)
                    return Last();
                return node.Value.FirstAfter(d);
            }
        }

        public int Count()
        {
            return count;
        }

        public T First()
        {
            if (Count() == 0)
                throw new InvalidOperationException("No first element to return");
            if (layers == 0)
            {
                return vals.First.Value;
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;
                while (node != null && node.Value.Count() == 0) node = node.Next;
                return node.Value.First();
            }
        }
        public T Last()
        {

            if (Count() == 0)
                throw new InvalidOperationException("No last element to return");
            if (layers == 0)
            {
                return vals.Last.Value;
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.Last;
                while (node != null && node.Value.Count() == 0) node = node.Previous;
                return node.Value.Last();
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

        public IEnumerable<T> ElementsAfter(double time)
        {
            if (layers == 0)
            {
                LinkedListNode<T> node = vals.First;
                while (node != null && evaluator(node.Value) < time)
                {
                    node = node.Next;
                }
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
            else
            {
                LinkedListNode<LayeredLinkedList<T>> node = nextLevel.First;

                while (node != null && node.Value.start + interval < time)
                {
                    node = node.Next;
                }
                while (node != null)
                {
                    foreach (T val in node.Value.ElementsAfter(time))
                        yield return val;
                    node = node.Next;
                }
            }
        }

        public IEnumerable<T> ElementsBetweenBackwards(double t1, double t2)
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

        public LLLGuide<T> Guide(bool beginning = true)
        {
            return new LLLGuide<T>(this, beginning);
        }

        public class LLLGuide<S>
        {
            SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>> trail;
            LinkedListNode<S> current;
            public LLLGuide(LayeredLinkedList<S> list, bool beginning = true)
            {
                trail = new SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>>();
                if (list.Count() == 0)
                    throw new InvalidOperationException("List has no elements to guide through");
                LinkedListNode<LayeredLinkedList<S>> lists;
                if (beginning)
                {
                    while (list.layers != 0)
                    {
                        lists = list.nextLevel.First;
                        while(lists.Value.Count()==0)
                            lists = lists.Next;
                        trail = trail.Push(lists);
                        list = lists.Value;
                    }
                    current = list.vals.First;
                }
                else
                {
                    while (list.layers != 0)
                    {
                        lists = list.nextLevel.Last;
                        while (lists.Value.Count() == 0)
                            lists = lists.Previous;
                        trail = trail.Push(lists);
                        list = lists.Value;
                    }
                    current = list.vals.Last;
                }
            }
            public S CurrentValue()
            {
                return current.Value;
            }
            public bool HasNext()
            {
                if (current.Next != null)
                    return true;
                SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>> tc = trail;
                LinkedListNode<LayeredLinkedList<S>> cur=null;
                while (tc!=null && tc.Count() > 0)
                {
                    cur = tc.Value;
                    tc = tc.Tail;
                    while (cur.Next != null && cur.Value.Count() == 0) cur = cur.Next;
                }
                return cur == null;
            }
            public bool HasPrevious()
            {
                if (current.Previous != null)
                    return true;
                SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>> tc = trail;
                LinkedListNode<LayeredLinkedList<S>> cur = null;
                while (tc != null && tc.Count() > 0)
                {
                    cur = tc.Value;
                    tc = tc.Tail;
                    while (cur.Next != null && cur.Value.Count() == 0) cur = cur.Previous;
                }
                return cur == null;
            }
            public S MoveNext()
            {
                if (current.Next != null)
                {
                    current = current.Next;
                    return current.Value;
                }
                SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>> tc = trail;
                LinkedListNode<LayeredLinkedList<S>> cur = null;
                while (cur==null && tc != null && tc.Count() > 0)
                {
                    cur = tc.Value;
                    tc = tc.Tail;
                    while (cur != null && cur.Value.Count() == 0) cur = cur.Next;
                }
                if (cur == null)
                    throw new InvalidOperationException("Can't move forwards");
                else
                {
                    trail = tc;
                    LayeredLinkedList<S> list = cur.Value;
                    LinkedListNode<LayeredLinkedList<S>> lists;
                    while (list.layers != 0)
                    {
                        lists = list.nextLevel.First;
                        while (lists.Value.Count() == 0)
                            lists = lists.Next;
                        trail = trail.Push(lists);
                        list = lists.Value;
                    }
                    current = list.vals.First;
                }
                return current.Value;
            }
            public S MovePrevious()
            {
                if (current.Previous != null)
                {
                    current = current.Previous;
                    return current.Value;
                }
                SimpleLinkedList<LinkedListNode<LayeredLinkedList<S>>> tc = trail;
                LinkedListNode<LayeredLinkedList<S>> cur = null;
                while (cur==null && tc != null && tc.Count() > 0)
                {
                    cur = tc.Value;
                    tc = tc.Tail;
                    while (cur != null && cur.Value.Count() == 0) cur = cur.Previous;
                }
                if (cur == null)
                    throw new InvalidOperationException("Can't move backwards");
                else
                {
                    trail = tc;
                    LayeredLinkedList<S> list = cur.Value;
                    LinkedListNode<LayeredLinkedList<S>> lists;
                    while (list.layers != 0)
                    {
                        lists = list.nextLevel.Last;
                        while (lists.Value.Count() == 0)
                            lists = lists.Previous;
                        trail = trail.Push(lists);
                        list = lists.Value;
                    }
                    current = list.vals.Last;
                }
                return current.Value;
            }
        }

    }
    public class LayeredLinkedListTest
    {
        public static void Main()
        {
            LayeredLinkedList<int> lll = new LayeredLinkedList<int>(0, x=>(double)x,1000,100,10,1);

            BulletHell.Time.Timer time = new BulletHell.Time.Timer();
            Random r = new Random();

            time.Reset();
            long thous = 1000;
            long million = thous * thous;
            long billion = thous * million;
            long trillion = million * million;
            for (long i = 0; i < 20 * thous; i++)
            {
                int j = r.Next(10);
                lll.Add(j);
                lll.Remove(j);
                //lll.Remove(r.Next(10));
                //lll.Add(1000 * r.NextDouble());
            }
            Console.WriteLine(lll.Count());
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
                if (30 < d && d < 40) count++;
            }
            Console.WriteLine(count);
            Console.WriteLine(time.Time);
            Console.ReadKey();
        }
    }
}

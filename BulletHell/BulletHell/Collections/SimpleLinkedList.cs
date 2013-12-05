using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Collections
{
    public class SimpleLinkedList<T> : IEnumerable<T>
    {
        T value;
        SimpleLinkedList<T> tail;
        int length;
        bool empty;

        public SimpleLinkedList(T t, SimpleLinkedList<T> tail = null)
        {
            empty = false;
            value = t;
            this.tail = tail;
            if (tail == null)
                length = 1;
            else
                length = tail.length + 1;
        }

        public SimpleLinkedList()
        {
            tail = null;
            value = default(T);
            length = 0;
            empty = true;
        }

        public int Count()
        {
            return length;
        }

        public T Value
        {
            get
            {
                if (empty)
                    throw new InvalidOperationException();
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public SimpleLinkedList<T> Tail
        {
            get
            {
                return tail;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (empty)
                yield break;
            yield return value;
            if(tail!=null)
            {
                foreach (T t in tail)
                    yield return t;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (empty)
                yield break;
            yield return value;
            if (tail != null)
            {
                foreach (T t in tail)
                    yield return t;
            }
        }

        public SimpleLinkedList<T> Push(T t)
        {
            return new SimpleLinkedList<T>(t, this);
        }
    }
}

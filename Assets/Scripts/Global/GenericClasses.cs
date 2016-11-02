using System;
using System.Collections;
using System.Collections.Generic;

public class MyLinkedList<T> : LinkedList<T>
{
    public void RemoveAll(Predicate<T> match)
    {
        var it = First;
        while (it != null)
        {
            if (match(it.Value))
            {
                Remove(it);
            }
            it = it.Next;
        }
    }

    public void ForEach(Action<T> action)
    {
        foreach (var e in this)
        {
            action(e);
        }
    }
}

public class MyComparison<T> : IComparer<T>
{
    Comparison<T> cmpFunc;

    public MyComparison(Comparison<T> cmpFunc)
    {
        this.cmpFunc = cmpFunc;
    }

    public int Compare(T lhs, T rhs)
    {
        return cmpFunc(lhs, rhs);
    }
}
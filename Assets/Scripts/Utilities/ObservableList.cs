using System;
using System.Collections.Generic;

[Serializable]
public class ObservableList<T> : List<T>
{
    public Action OnListChanged;

    public ObservableList() : base() { }
    
    public ObservableList(Action observer) : base()
    {
        OnListChanged += observer;
    }
    
    public ObservableList(IEnumerable<T> collection) : base(collection)
    { }
    
    public ObservableList(IEnumerable<T> collection, Action observer) : base(collection)
    {
        OnListChanged += observer;
        OnListChanged?.Invoke();
    }

    public new void Add(T item)
    {
        base.Add(item);
        OnListChanged?.Invoke();
    }
    
    public new void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        OnListChanged?.Invoke();
    }

    public new void Remove(T item)
    {
        base.Remove(item);
        OnListChanged?.Invoke();
    }

    public new void RemoveAt(int index)
    {
        base.RemoveAt(index);
        OnListChanged?.Invoke();
    }

    public new void Clear()
    {
        base.Clear();
        OnListChanged?.Invoke();
    }
    
    public new void RemoveAll(Predicate<T> match)
    {
        base.RemoveAll(match);
        OnListChanged?.Invoke();
    }
    
    public new void RemoveRange(int index, int count)
    {
        base.RemoveRange(index, count);
        OnListChanged?.Invoke();
    }

    public override string ToString()
    {
        string info = string.Empty;

        if (Count <= 0)
        {
            info = TranslationKeys.Empty;
            return info;
        }
        
        foreach (var item in this)
        {
            info += item.ToString();
        }

        return info;
    }
}

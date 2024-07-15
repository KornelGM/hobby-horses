using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

[System.Serializable]
public class WeightedListItem<T>
{
    public T item;
    [Range(0,100)]public float weight;
    [ReadOnly] public float DebugProbability;
    public WeightedListItem(T item, float weight)
    {
        this.item = item;
        this.weight = weight;
    }
    public override string ToString()
    {
        return "Weighted List item " + item.ToString() + " weight " + weight + " ";
    }
}

[System.Serializable]
public class WeightedList<T>: ISerializationCallbackReceiver 
{
    [SerializeField] public List<WeightedListItem<T>> items;

    public int Count => items.Count;

    public WeightedListItem<T> this[int i]
    {
        get { return items[i]; }
        set { items[i] = value; }
    }

    public WeightedList()
    {
        items = new();
    }

    public WeightedListItem<T> GetItem(T item)
    {
        return items.Find(x => x.item.Equals(item));
    }

    public List<WeightedListItem<T>> GetItems(T item)
    {
        return items.FindAll(x => x.item.Equals(item));
    }

    public WeightedListItem<T> GetItem(System.Predicate<T> predicate)
    {
        foreach (var item in items)
        {
            if (predicate(item.item)) return item;
        }
        return null;
    }

    public WeightedList<T> GetItems(System.Predicate<T> predicate)
    {
        WeightedList<T> weightedList = new();
        foreach (var item in items)
        {
            if (predicate(item.item)) weightedList.items.Add(item);
        }
        return weightedList;
    }

    public float WeightSum()
    {
        return items.Sum(x => x.weight);
    }

    public List<T> GetItemList()
    {
        List<T> onlyItemList = new();

        foreach (var item in items)
        {
            onlyItemList.Add(item.item);
        }

        return onlyItemList;
    }

    public List<float> GetWeightList()
    {
        List<float> weights = new();

        foreach (var item in items)
        {
            weights.Add(item.weight);
        }

        return weights;
    }

    public WeightedList<T> CreateDisposableCopy()
    {
        WeightedList<T> newList = new();

        foreach (var item in items)
        {
            newList.items.Add(new WeightedListItem<T>(item.item, item.weight));
        }
        return newList;
    }

    public void OnValidate() 
    {
        float sum = WeightSum();
        foreach(var item in items)
        {
            item.DebugProbability = item.weight/sum;
        }
    }
    public void OnBeforeSerialize() => OnValidate();
    public void OnAfterDeserialize(){}
}

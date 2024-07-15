using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utilities
{
    public static bool TryCast<T>(object obj, out T result)
    {
        if (obj is T)
        {
            result = (T)obj;
            return true;
        }

        result = default;
        return false;
    }

    public static void CastListToOtherType<T1, T2>(List<T1> listOfObjectsToCast, List<T2> listOfObjectsAfterCast)
    {
        foreach (T1 manager in listOfObjectsToCast)
        {
            if(manager == null)
                continue;

            if (TryCast(manager, out T2 updateable))
            {
                if (updateable != null)
                    listOfObjectsAfterCast.Add(updateable);        
            }
        }
    }

    public static List<T2> CastListToOtherType<T1, T2>(List<T1> listOfObjectsToCast)
    {
        List<T2> result = new();

        foreach (T1 manager in listOfObjectsToCast)
        {
            if (manager == null)
                continue;

            if (TryCast(manager, out T2 updateable)) 
            {
                if (updateable != null)
                    result.Add(updateable);
            }
        }

        return result;
    }

    public static void CopyTransform(this Transform originalTransform, Transform transformToCopy)
    {
        Transform tempParent = originalTransform.parent;
        originalTransform.SetParent(transformToCopy);

        originalTransform.localPosition = Vector3.zero;
        originalTransform.transform.localRotation = Quaternion.identity;
        originalTransform.localScale = Vector3.one;

        originalTransform.SetParent(tempParent);
    }

    public static T CopyCollider <T> (this T original, GameObject destination) where T: Component
    {
        Type type = typeof(T);
        T copy = destination.AddComponent<T>();
        PropertyInfo[] fields = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo field in fields)
        {
            if (field != null && field.SetMethod == null) continue;
            field.SetValue(copy, field.GetValue(original));
        }

        return copy;
    }

    public static float SnapToGrid(float value, float gridSize)
    {
        return Mathf.Round(value / gridSize) * gridSize;
    }

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    public static float GetPercentageOf(this float value, float maxValue)
    {
        return value / maxValue * 100;
    }

    public static float GetValueFromPercentage(this float percentage, float maxValue)
    {
        return percentage / 100 * maxValue;
    }

    public static float GetValueFromPercentageClamped(this float value, float percentage, float minValue, 
        float maxValue)
    {
        return Mathf.Clamp(value.GetValueFromPercentage(percentage), minValue, maxValue);
    }
    
    public static int GetPercentageOf(this int value, int maxValue)
    {
        float result = (float)value / maxValue * 100;
        
        return Mathf.RoundToInt(result);
    }

    public static int GetValueFromPercentage(this int percentage, int maxValue)
    {
        float result = (float)percentage / 100 * maxValue;
        
        return Mathf.RoundToInt(result);
    }

    public static int GetValueFromPercentageClamped(this int value, int percentage, int minValue, 
        int maxValue)
    {
        return Mathf.Clamp(value.GetValueFromPercentage(percentage), minValue, maxValue);
    }

    // excludeFirstValue - if true, first value which almost always is "None" will be excluded
    public static T GetRandomEnumValue<T>(bool excludeFirstValue = true)
    {
        Array enumValues = Enum.GetValues(typeof(T));
        int randomValue = Random.Range(excludeFirstValue ? 1 : 0, enumValues.Length);
        return (T)enumValues.GetValue(randomValue);
    }

    public static T GetRandomItem<T>(this IEnumerable<T> collection)
    {
        if (collection == null) return default;
        if (collection.Count() == 0) return default;
        return collection.ElementAt(Random.Range(0, collection.Count()));
    }

    public static Dictionary<int, T> ConvertToDictionary<T>(this List<T> list, bool sort = true)
    {
        if (list == null) return null;
        if (sort) list = list.OrderBy(item => item).ToList();
        
        return list.Select((value, index) => new { index, value })
            .ToDictionary(pair => pair.index, pair => pair.value);
    }
}
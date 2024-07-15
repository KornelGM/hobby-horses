using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TestingWindow : OdinMenuEditorWindow
{
    [MenuItem("Tools/AnimalShelter2/Testing/Testing Window")]
    private static void OpenWindow() => GetWindow<TestingWindow>().Show();

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();
        tree.Add("Rng Test", new RNGTesting());

        return tree;
    }
    public class RNGTesting
    {
        [SerializeField]WeightedList<int> _weightedList;

        [Button("Weighted List Test")]
        public void WeightedListTest(int numberOfItems)
        {
            var items = RNG.WeightedLists.GetRandomDifferentItems(numberOfItems,_weightedList);
            string s ="";
            foreach(var item in items)
            {
                s+=item.ToString()+" ";
            }
            Debug.Log(s);
        }

        [SerializeField] int _numberOfItems;
        [SerializeField] float _intervalLength = 0.1f;

        [Button("normal distribution test")]
        public void NormalDistTest(int mean, float stdDeviation)
        {

            if (_numberOfItems == 0) return;

            List<float> floats = new();
            for (int i = 0; i < _numberOfItems; i++)
            {
                floats.Add(RNG.Numbers.GetRandomNormalDist(mean, stdDeviation));
            }

            floats.Sort();
            distributionCurve = new AnimationCurve();

            List<float> cutted = new();
            List<float> values = new();
            values.Add(floats[0]);
            float topValue = floats[0] + _intervalLength;
            float currentCount = 0;
            foreach (var item in floats)
            {
                currentCount++;

                if (item > topValue)
                {
                    cutted.Add(currentCount / _numberOfItems);
                    currentCount = 0;
                    topValue = item + _intervalLength;
                    values.Add(item);
                }
            }
            if (cutted.Count > 500)
            {
                Debug.LogWarning("The curve size is too big, try again with lower number of items or higher interval length");
                distributionCurve = new();
                return;
            }
            for (int i = 0; i < cutted.Count; i++)
            {
                distributionCurve.AddKey(values[i], cutted[i]);
            }
        }
        [SerializeField] AnimationCurve distributionCurve;
    }
}

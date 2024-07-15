using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace RNG
{

    public static class Numbers
    {
        /// <summary>
        /// returns a random number using normal distribution
        /// </summary>
        /// <param name="mean">average value</param>
        /// <param name="stdDeviation">deviation from the average</param>
        /// <returns></returns>
        public static float GetRandomNormalDist(float mean, float stdDeviation = 1)//Box-Muller transform see https://en.wikipedia.org/wiki/Box%E2%80%93Muller_transform
        {
            float random1 = 1.0f - UnityEngine.Random.Range(0, 1f);
            float random2 = 1.0f - UnityEngine.Random.Range(0, 1f);
            float standardNormalDistRandom = Mathf.Sqrt(-2.0f * Mathf.Log(random1)) * Mathf.Sin(2.0f * Mathf.PI * random2);
            float randNormal = mean + stdDeviation * standardNormalDistRandom;

            return randNormal;
        }

        /// <summary>
        /// returns number of different int values in given interval
        /// Max not inclusive!!!!!!
        /// </summary>
        public static List<int> GetRandomDifferentInts(int number, int min, int max)//TODO dodaj algorytm Igora XD
        {
            if (number > ((max - min)))
            {
                Debug.LogWarning("Asking for more different integers, than there are in range");
                return new List<int>();
            }

            List<int> randomInts = new();

            for (int i = 0; i < number; i++)
            {
                while (true)
                {
                    int randomInt = UnityEngine.Random.Range(min, max);

                    if (randomInts.Contains(randomInt)) continue;

                    randomInts.Add(randomInt);
                    break;
                }

            }

            return randomInts;
        }
    }


    public static class Vectors
    {
        /// <summary>
        /// returns vector with values in given interval
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomVector3(float min, float max)
        {
            return new Vector3(UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max), UnityEngine.Random.Range(min, max));
        }

        /// <summary>
        /// returns vector 3 with values randomly chosen by normal distribution with given parameters
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomVector3NormalDist(float mean, float stdDeviation)
        {
            return new Vector3(RNG.Numbers.GetRandomNormalDist(mean, stdDeviation),
                               RNG.Numbers.GetRandomNormalDist(mean, stdDeviation),
                                RNG.Numbers.GetRandomNormalDist(mean, stdDeviation));
        }
        /// <summary>
        /// returns vector 3 with values randomly chosen by normal distribution with given parameters and clamps value between two given values
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomVector3NormalDistClamp(float mean, float stdDeviation, float min, float max)
        {
            var vector = GetRandomVector3NormalDist(mean, stdDeviation);

            vector.x = Mathf.Clamp(vector.x, min, max);
            vector.y = Mathf.Clamp(vector.y, min, max);
            vector.z = Mathf.Clamp(vector.z, min, max);

            return vector;
        }
    }

    public static class WeightedLists
    {

        public static T GetRandomItemFromWeightedList<T>(WeightedList<T> weightedList)
        {
            float weightSum = weightedList.WeightSum();
            float random = UnityEngine.Random.Range(0, weightSum);
            float accumulated = 0;
            for (int i = 0; i < weightedList.Count; i++)
            {
                float prevAccumulated = accumulated;
                accumulated += weightedList[i].weight;
                if (random > accumulated || random < prevAccumulated) continue;

                return weightedList[i].item;
            }
            return default;
        }
        private static WeightedListItem<T> GetRandomItemFromWeightedList<T>(WeightedList<T> weightedList, float weightSum)
        {

            float random = UnityEngine.Random.Range(0, weightSum);
            float accumulated = 0;
            for (int i = 0; i < weightedList.Count; i++)
            {
                float prevAccumulated = accumulated;
                accumulated += weightedList[i].weight;
                if (random > accumulated || random < prevAccumulated) continue;

                return weightedList[i];
            }
            return default;
        }


        public static List<T> GetRandomDifferentItems<T>(int number, WeightedList<T> weightedList)
        {
            if(number<=0)
            {
                Debug.LogWarning("Asking for 0 or less items in from weighted list");
                return new List<T>();
            }

            var copied = weightedList.CreateDisposableCopy();

            if (number > copied.Count)
            {
                Debug.LogWarning("Asking for more items, than there are in range");
                return new List<T>();
            }

            List<T> randomItems = new();

            float weightSum = weightedList.WeightSum();

            while (randomItems.Count != number)
            {
                var item = GetRandomItemFromWeightedList<T>(copied,weightSum);

                randomItems.Add(item.item);
                weightSum-=item.weight;                
                item.weight=0;
            }

            return randomItems;
        }
    }

}

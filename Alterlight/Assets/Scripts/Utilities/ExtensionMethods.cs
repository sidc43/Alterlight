using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void Print<T>(T obj)
        {
            Debug.Log(obj);
        }
        public static void PrintList<T>(this List<T> list)
        {
            string res = "[";
            for (int i = 0; i < list.Count - 1; i++)
                res += list[i] + ", ";

            res += list.Last() + "]";
            Print(res);
        }
        public static bool ContainsAny(this string tileName, List<string> list)
        {
            List<bool> contains = new List<bool>();

            foreach (string s in list)
                contains.Add(tileName.Contains(s));

            return contains.Contains(true);
        }
        public static int LastIndexOf<T>(this List<T> list, T target)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < list.Count; i++)
                if (EqualityComparer<T>.Default.Equals(list[i], target))
                    indexes.Add(i);
                
            if (indexes.Count < 1)
                return -1;

            return indexes.Last();
        }
        public static List<int> AllIndexesOf<T>(this List<T> list, T target)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < list.Count; i++)
                if (EqualityComparer<T>.Default.Equals(list[i], target))
                    indexes.Add(i);
                
            return indexes; // Count == 0 if no match
        }
        public static bool EqualsIgnoringSequence<T>(IEnumerable<T> list1, IEnumerable<T> list2) 
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1) 
            {
                if (cnt.ContainsKey(s))
                    cnt[s]++; 
                else
                    cnt.Add(s, 1);
            }
            foreach (T s in list2) 
            {
                if (cnt.ContainsKey(s)) 
                    cnt[s]--;
                else 
                    return false;
            }
            return cnt.Values.All(c => c == 0);
        }
        public static bool DictionaryEquals(Dictionary<Item, int> playerItems, Dictionary<Item, int> recipe)
        {
            bool countEqual = playerItems.Count == recipe.Count;

            List<bool> itemsEqual = new List<bool>();
            List<bool> hasCount = new List<bool>();

            if (!countEqual)
                return false;

            for (int i = 0; i < playerItems.Count; i++)
            {
                bool items = playerItems.ElementAt(i).Key.name == recipe.ElementAt(i).Key.name;
                bool count = playerItems.ElementAt(i).Value >= recipe.ElementAt(i).Value;

                itemsEqual.Add(items);
                hasCount.Add(count);
            }
            
            return (!itemsEqual.Contains(false) && !hasCount.Contains(false));
        }

        public static float Magnitude(this Vector3 vector) { return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

        public static Vector3 Normalize(this Vector3 value)
        {
            const float kEpsilon = 0.00001f;
            float mag = value.Magnitude();
            if (mag > kEpsilon)
                return value / mag;
            else
                return Vector3.zero;
        }
    }
}
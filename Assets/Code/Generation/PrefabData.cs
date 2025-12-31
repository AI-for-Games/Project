using System;
using UnityEngine;

namespace Code.Generation
{
    public enum PrefabCategory
    {
        Room,
        Corridor
    }

    public enum PrefabType
    {
        Corner,
        Crossover,
        End,
        Straight,
        T
    }

    [Flags]
    public enum Directions
    {
        None = 0,
        North = 1 << 0,
        East = 1 << 1,
        South = 1 << 2,
        West = 1 << 3
    }

    public class PrefabData : MonoBehaviour
    {
        [Tooltip("Which category is the prefab in")]
        public PrefabCategory category;

        [Tooltip("Type of piece the prefab is (its connections)")]
        public PrefabType type;

        [Tooltip("Relative spawn weight (higher = more often")]
        public int spawnWeight;
        
        [HideInInspector]
        public Directions openSides;
    }
}

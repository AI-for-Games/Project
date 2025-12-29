using UnityEngine;

public enum PrefabType
{
    Room,
    Corridor,
    Joint
}

[System.Flags]
public enum OpenSides
{
    None = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3
}

public class PrefabData : MonoBehaviour
{
    public PrefabType type;
    
    [Tooltip("Which sides are open (in default rotation)")]
    public OpenSides openSides;
    
    [Tooltip("Relative spawn weight (higher = more often")]
    public int spawnWeight = 1;
}

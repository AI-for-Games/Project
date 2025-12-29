using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float cellSize = 20f;

    public List<PrefabData> rooms;
    public List<PrefabData> corridors;

    private PrefabData[,] grid;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        grid = new PrefabData[width, height];

        Vector2Int start = new Vector2Int(width / 2, height / 2);
        Place(start.x, start.y, GetRandomRoom());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null) continue;

                TryPlaceAt(x, y);
            }
        }
    }

    void TryPlaceAt(int x, int y)
    {
        var neighbors = GetNeighbors(x, y);

        if (neighbors.Count == 0) return;

        // If ANY neighbor is a room → we MUST place a corridor
        bool mustBeCorridor = false;
        foreach (var n in neighbors)
        {
            if (n.type == PrefabType.Room)
            {
                mustBeCorridor = true;
                break;
            }
        }

        PrefabData prefab = mustBeCorridor
            ? GetRandomCorridor()
            : GetWeightedRandom(rooms, corridors);

        Place(x, y, prefab);
    }

    void Place(int x, int y, PrefabData prefab)
    {
        Quaternion rotation = FindValidRotation(x, y, prefab);

        Vector3 pos = new Vector3(x * cellSize, 0, y * cellSize);
        Instantiate(prefab.gameObject, pos, rotation, transform);

        grid[x, y] = prefab;
    }

    Quaternion FindValidRotation(int x, int y, PrefabData prefab)
    {
        foreach (var rot in new[] { 0, 90, 180, 270 })
        {
            var rotatedDirs = Rotate(prefab.openSides, rot);

            if (MatchesNeighbors(x, y, rotatedDirs))
                return Quaternion.Euler(0, rot, 0);
        }

        return Quaternion.identity;
    }

    bool MatchesNeighbors(int x, int y, OpenSides rotatedDirs)
    {
        // West neighbor
        if (x > 0 && grid[x - 1, y] != null)
        {
            if (!HasConnection(rotatedDirs, OpenSides.West) ||
                !HasConnection(grid[x - 1, y].openSides, OpenSides.East))
                return false;
        }

        // East neighbor
        if (x < width - 1 && grid[x + 1, y] != null)
        {
            if (!HasConnection(rotatedDirs, OpenSides.East) ||
                !HasConnection(grid[x + 1, y].openSides, OpenSides.West))
                return false;
        }

        // South neighbor
        if (y > 0 && grid[x, y - 1] != null)
        {
            if (!HasConnection(rotatedDirs, OpenSides.South) ||
                !HasConnection(grid[x, y - 1].openSides, OpenSides.North))
                return false;
        }

        // North neighbor
        if (y < height - 1 && grid[x, y + 1] != null)
        {
            if (!HasConnection(rotatedDirs, OpenSides.North) ||
                !HasConnection(grid[x, y + 1].openSides, OpenSides.South))
                return false;
        }

        return true;
    }
    
    bool HasConnection(OpenSides set, OpenSides test)
    {
        return (set & test) != 0;
    }

    List<PrefabData> GetNeighbors(int x, int y)
    {
        var list = new List<PrefabData>();

        if (x > 0 && grid[x - 1, y]) list.Add(grid[x - 1, y]);
        if (x < width - 1 && grid[x + 1, y]) list.Add(grid[x + 1, y]);
        if (y > 0 && grid[x, y - 1]) list.Add(grid[x, y - 1]);
        if (y < height - 1 && grid[x, y + 1]) list.Add(grid[x, y + 1]);

        return list;
    }

    PrefabData GetRandomRoom() => GetWeightedRandom(rooms);
    PrefabData GetRandomCorridor() => GetWeightedRandom(corridors);

    PrefabData GetWeightedRandom(params List<PrefabData>[] lists)
    {
        int total = 0;
        foreach (var list in lists)
            foreach (var p in list)
                total += p.spawnWeight;

        int roll = Random.Range(0, total);

        foreach (var list in lists)
        {
            foreach (var p in list)
            {
                roll -= p.spawnWeight;
                if (roll < 0)
                    return p;
            }
        }

        return lists[0][0];
    }

    OpenSides Rotate(OpenSides dir, int angle)
    {
        int steps = angle / 90;
        for (int i = 0; i < steps; i++)
        {
            dir = ((dir & OpenSides.North) != 0 ? OpenSides.East : 0)
                | ((dir & OpenSides.East) != 0 ? OpenSides.South : 0)
                | ((dir & OpenSides.South) != 0 ? OpenSides.West : 0)
                | ((dir & OpenSides.West) != 0 ? OpenSides.North : 0);
        }
        return dir;
    }
}

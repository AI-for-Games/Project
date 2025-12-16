using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // [Header("Level Size")]
    public int width = 30;
    public int height = 5;
    public int depth = 30;

    // [Header("Rooms"]]
    public int roomCount = 10;
    public Vector2Int roomSizeMin = new Vector2Int(4, 4);
    public Vector2Int roomSizeMax = new Vector2Int(10, 10);

    // [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject ceilingPrefab;

    private int[,,] map;
    private List<Room> rooms = new List<Room>();

    void Start()
    {
        GenerateMap();
        BuildMap();
        Debug.Log("Rooms created: " + rooms.Count);

    }

    void GenerateMap()
    {
        map = new int[width, height, depth];  // Clear map

        for (int x = 0; x < width; x++)  // Fill with walls
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                    map[x, y, z] = 1;

        int attempts = 0;
        int maxAttempts = roomCount * 5;

        while (rooms.Count < roomCount && attempts < maxAttempts)
        {
            TryCreateRoom();
            attempts++;
        }


        ConnectRooms();  // Connect rooms
    }

    void TryCreateRoom()
    {
        int roomWidth = Random.Range(roomSizeMin.x, roomSizeMax.x);
        int roomDepth = Random.Range(roomSizeMin.y, roomSizeMax.y);

        int x = Random.Range(1, width - roomWidth - 1);
        int z = Random.Range(1, depth - roomDepth - 1);

        BoundsInt newRoom = new BoundsInt(
            x,
            1,
            z,
            roomWidth,
            height - 2,
            roomDepth
        );

        foreach (Room room in rooms)
        {
            if (RoomsOverlap(room.bounds, newRoom))
                return;
        }

        Room createdRoom = new Room { bounds = newRoom };
        rooms.Add(createdRoom);

        CarveRoom(createdRoom);
    }

    void CarveRoom(Room room)
    {
        BoundsInt b = room.bounds;

        for (int x = b.xMin; x < b.xMax; x++)
            for (int y = b.yMin; y < b.yMax; y++)
                for (int z = b.zMin; z < b.zMax; z++)
                    map[x, y, z] = 0;  // Empty
    }

    void CarveCorridor(Vector3Int a, Vector3Int b)
    {
        int x = a.x;
        int z = a.z;

        while (x != b.x)
        {
            map[x, a.y, z] = 0;
            x += x < b.x ? 1 : -1;
        }

        while (z != b.z)
        {
            map[x, a.y, z] = 0;
            z += z < b.z ? 1 : -1;
        }
    }

    void ConnectRooms()
    {
        for (int i = 1; i < rooms.Count; i++)
        {
            Vector3Int a = rooms[i - 1].Center;
            Vector3Int b = rooms[i].Center;

            CarveCorridor(a, b);
        }
    }

    void BuildMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    if (map[x, y, z] == 1)
                    {
                        Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                    }
                    else
                    {
                        if (y == 1)  // Floor
                            Instantiate(floorPrefab, pos, Quaternion.identity, transform);

                        if (y == height - 2)  // Ceiling
                            Instantiate(ceilingPrefab, pos, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    bool RoomsOverlap(BoundsInt a, BoundsInt b)
    {
        return
            a.xMin < b.xMax && a.xMax > b.xMin &&
            a.yMin < b.yMax && a.yMax > b.yMin &&
            a.zMin < b.zMax && a.zMax > b.zMin;
    }
}

class Room
{
    public BoundsInt bounds;

    public Vector3Int Center => new Vector3Int(
        bounds.x + bounds.size.x / 2,
        bounds.y + bounds.size.y / 2,
        bounds.z + bounds.size.z / 2
    );
}

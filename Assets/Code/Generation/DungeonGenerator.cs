using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    Empty,
    Floor,
    Wall
}

public class DungeonGenerator : MonoBehaviour
{
    public GameObject floorPrefab;

    public GameObject[] wallPreFabs;

    public int width = 50;
    public int height = 50;

    public float tileSize = 1f;
    public float wallHalfLength = 1.5f;
    public float wallThicknessOffset = 0.5f;

    private CellType[,] grid;
    private bool[,] xWalls;
    private bool[,] yWalls;

    private Quaternion RotNorth = Quaternion.Euler(0, 0, 0);
    private Quaternion RotEast = Quaternion.Euler(0, 90, 0);
    private Quaternion RotSouth = Quaternion.Euler(0, 180, 0);
    private Quaternion RotWest = Quaternion.Euler(0, 270, 0);

    private Vector3 OffsetNorth = new Vector3(0, 0, 0);
    private Vector3 OffsetSouth = new Vector3(0, 0, 0);
    private Vector3 OffsetEast = new Vector3(0, 0, 0);
    private Vector3 OffsetWest = new Vector3(0, 0, 0);

    void Start()
    {
        OffsetNorth = new Vector3(0, 0, wallThicknessOffset);
        OffsetSouth = new Vector3(0, 0, -wallThicknessOffset);
        OffsetEast = new Vector3(wallThicknessOffset, 0, 0);
        OffsetWest = new Vector3(-wallThicknessOffset, 0, 0);

        Generate();

        SpawnFloors();

        BuildWallMaps();
        SpawnXWalls();
        SpawnYWalls();
    }

    void Generate()
    {
        grid = new CellType[width, height];

        int roomCount = 10;

        for (int i = 0; i < roomCount; i++)
        {
            int roomW = Random.Range(6, 10);
            int roomH = Random.Range(6, 10);
            int x = Random.Range(1, width - roomW - 1);
            int y = Random.Range(1, width - roomH - 1);

            for (int rx = x; rx < x + roomW; rx++)
            {
                for (int ry = y; ry < y + roomH; ry++)
                {
                    grid[rx, ry] = CellType.Floor;
                }
            }
        }
    }

    void BuildWallMaps()
    {
        xWalls = new bool[width, height];
        yWalls = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!IsFloor(x, y)) continue;

                if (!IsFloor(x, y + 1))
                    yWalls[x, y] = true;

                if (!IsFloor(x + 1, y))
                    xWalls[x, y] = true;
            }
        }
    }

    void SpawnXWalls()
    {
        for (int x = 0; x < width; x++)
        {
            int runStart = -1;

            for (int y = 0; y <= height; y++)
            {
                bool hasWall = (y < height) && xWalls[x, y];

                if (hasWall && runStart == -1)
                {
                    runStart = y;
                }
                else if (!hasWall && runStart != -1)
                {
                    SpawnXWall(x, runStart, y - 1);
                    runStart = -1;
                }
            }
        }
    }

    void SpawnYWalls()
    {
        for (int y = 0; y < height; y++)
        {
            int runStart = -1;

            for (int x = 0; x <= width; x++)
            {
                bool hasWall = (x < width) && yWalls[x, y];

                if (hasWall && runStart == -1)
                {
                    runStart = x;
                }
                else if (!hasWall && runStart != -1)
                {
                    SpawnYWall(runStart, x - 1, y);
                    runStart = -1;
                }
            }
        }
    }

    void SpawnXWall(int startX, int endX, int y)
    {
        int length = endX - startX + 1;
        int wallCount = length / 3;

        for (int i = 0; i < wallCount; i++)
        {
            int x = startX + i * 3 + 1; // center of 3 tiles

            Vector3 pos = new Vector3(
                x * tileSize,
                0,
                (y * tileSize) + wallThicknessOffset
            );

            Instantiate(GetRandWall(), pos, RotNorth, transform);
        }
    }

    void SpawnYWall(int x, int startY, int endY)
    {
        int length = endY - startY + 1;
        int wallCount = length / 3;

        for (int i = 0; i < wallCount; i++)
        {
            int y = startY + i * 3 + 1;

            Vector3 pos = new Vector3(
                (x * tileSize) - wallThicknessOffset,
                0,
                y * tileSize
            );

            Instantiate(GetRandWall(), pos, RotEast, transform);
        }
    }

    void SpawnFloors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == CellType.Floor)
                {
                    Instantiate(
                        floorPrefab,
                        new Vector3(x, 0, y),
                        Quaternion.identity,
                        transform
                    );
                }
            }
        }
    }

    GameObject GetRandWall()
    {
        return wallPreFabs[Random.Range(0, wallPreFabs.Length)];
    }

    void SpawnWall(Vector3 position, Quaternion rotation)
    {
        Instantiate(GetRandWall(), position, rotation, transform);
    }

    Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x * tileSize, 0, y * tileSize);
    }

    bool IsFloor(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return false;
    
        return grid[x, y] == CellType.Floor;
    }
}

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

    public GameObject wallNorth;
    public GameObject wallSouth;
    public GameObject wallEast;
    public GameObject wallWest;

    public int width = 50;
    public int height = 50;

    private CellType[,] grid;

    void Generate()
    {
        grid = new CellType[width, height];

        int roomCount = 10;

        for (int i = 0; i < roomCount; i++)
        {
            int roomW = Random.Range(4, 8);
            int roomH = Random.Range(4, 8);
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

    //bool IsFloor(int x, int y)
    //{
    //    if (x < 0 || y < 0 || x >= width || y >= height)
    //        return false;
    //
    //    return grid[x, y] == CellType.Floor;
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}
}

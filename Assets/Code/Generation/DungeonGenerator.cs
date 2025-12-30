using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Generation
{
    public class DungeonGenerator : MonoBehaviour
    {
        public int gridWidth = 20;
        public int gridHeight = 20;
        public float cellSize = 20f;

        public List<PrefabData> rooms;
        public List<PrefabData> corridors;

        private List<PrefabData> _roomAlls;
        private List<PrefabData> _roomCorners;
        private List<PrefabData> _roomStraights;
        private List<PrefabData> _roomTs;
        private List<PrefabData> _roomEnds;

        private List<PrefabData> _corridorAlls;
        private List<PrefabData> _corridorCorners;
        private List<PrefabData> _corridorStraights;
        private List<PrefabData> _corridorTs;

        private PrefabData[,] _grid;
        
        private Vector3 _startOffset;

        void Start()
        {
            Generate();
        }

        void Generate()
        {
            _grid = new PrefabData[gridWidth, gridHeight];  // Create blank 2D array
            _startOffset = transform.position;  // Fetch start offset (generator pos)

            SpawnCell(0, 0, true);  // Start cell in center (catalyst for generation)

            SpawnAdjacent(0, 0);
        }

        void SpawnAdjacent(int gridX, int gridY, int maxDepth = 5, int currentDepth = 0)
        {
            var existingPrefab = _grid[gridX, gridY];
            if ((existingPrefab.openSides & Directions.North) != 0)  // If current cell is open on top
            {
                SpawnCell(gridX, gridY - 1, existingPrefab.type == PrefabType.Corridor);
            }
        }

        Directions GetRequiredDirections(int gridX, int gridY)  // Check adjacent cells for required directions
        {
            var requiredDirections = Directions.None;
            
            // Top cell
            if (gridY < gridHeight - 2 && (_grid[gridX, gridY - 1].openSides & Directions.South) != 0)  // If cell has South open
            {
                requiredDirections |= Directions.North;  // Require North direction (to join the open south)
            }
            
            // Right cell
            if (gridX < gridWidth - 2 && (_grid[gridX + 1, gridY].openSides & Directions.West) != 0)  // Check West of other
            {
                requiredDirections |= Directions.East;  // If valid set East of current to required
            }
            
            // Bottom cell
            if (gridY > 0 && (_grid[gridX, gridY - 1].openSides & Directions.North) != 0)  // Check North of other
            {
                requiredDirections |= Directions.South;  // If valid set South of current to required
            }
            
            // Left cell
            if (gridX <= 0 && (_grid[gridX - 1, gridY].openSides & Directions.East) != 0)  // Check East of other
            {
                requiredDirections |= Directions.West;  // If valid set West of current to required
            }

            return requiredDirections;  // Return complete required directions
        }

        bool IsValidPrefab(PrefabData data, Directions requiredDirections)
        {
            return (data.openSides & requiredDirections) ==  requiredDirections;  // Prefab has at LEAST required dirs
        }

        void SpawnCell(int gridX, int gridY, bool isRoom)
        {
            var rot = Quaternion.Euler(0, 0, 0);
            Vector3 pos = new Vector3(gridX * cellSize, 0, gridY * cellSize);

            _grid[gridX, gridY] = isRoom ? GetRandomRoom() : GetRandomCorridor();
            
            Instantiate(_grid[gridX, gridY], pos, rot);
        }

        PrefabData GetRandomRoom() => GetWeightedRandom(rooms);
        PrefabData GetRandomCorridor() => GetWeightedRandom(corridors);

        PrefabData GetWeightedRandom(params List<PrefabData>[] lists)
        {
            var total = lists.SelectMany(list => list).Sum(p => p.spawnWeight);  // Total weight in list
            var roll = Random.Range(0, total);  // Random start

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
    }
}

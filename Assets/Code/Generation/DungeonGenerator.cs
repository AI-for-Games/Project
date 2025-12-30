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

        [Tooltip("All prefabs the generator can use (make sure PrefabData is set correctly!)")]
        public List<PrefabData> prefabs;
        
        [HideInInspector]
        public List<PrefabData> rooms;
        [HideInInspector]
        public List<PrefabData> corridors;
        
        private PrefabData[,] _grid;
        
        private Vector3 _startOffset;

        void Start()
        {
            LoadPrefabs();
            Generate();
        }

        void LoadPrefabs()
        {
            foreach (var prefab in prefabs)
            {
                switch (prefab.type)
                {
                    case PrefabType.Corner:
                        prefab.openSides = Directions.North | Directions.East;
                        break;
                    case PrefabType.Crossover:
                        prefab.openSides = Directions.North | Directions.East | Directions.South | Directions.West;
                        break;
                    case PrefabType.End:
                        prefab.openSides = Directions.North;
                        break;
                    case PrefabType.Straight:
                        prefab.openSides = Directions.North | Directions.South;
                        break;
                    case PrefabType.T:
                        prefab.openSides = Directions.North | Directions.East | Directions.West;
                        break;
                }
            }
            
            rooms = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Room });
            corridors = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Corridor });
        }

        void Generate()
        {
            _grid = new PrefabData[gridWidth, gridHeight]; // Create blank 2D array
            _startOffset = transform.position; // Fetch start offset (generator pos)

            SpawnCell(0, 0, GetWeightedRandom(rooms)); // Start cell in center (catalyst for generation)

            SpawnAdjacent(0, 0, 2000);
        }

        void SpawnAdjacent(int gridX, int gridY, int maxDepth = 5, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth)
            {
                return;
            }
            
            var existingPrefab = _grid[gridX, gridY];
            if ((existingPrefab.openSides & Directions.North) != 0 && gridY + 1 < gridHeight && _grid[gridX, gridY + 1] == null)  // If current cell is open on top
            {
                // TODO: Create actual room decision based on adjacent cells not just "room or corridor"
                bool isRoom = existingPrefab.category == PrefabCategory.Corridor;
                
                var currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                while (!IsValidPrefab(gridX, gridY + 1, currFab))
                {
                    currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                }
                
                SpawnCell(gridX, gridY + 1, currFab);
                SpawnAdjacent(gridX, gridY + 1, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.openSides & Directions.East) != 0 && gridX + 1 < gridWidth && _grid[gridX + 1, gridY] == null)  // If current cell is open on top
            {
                // TODO: Create actual room decision based on adjacent cells not just "room or corridor"
                bool isRoom = existingPrefab.category == PrefabCategory.Corridor;
                
                var currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                while (!IsValidPrefab(gridX + 1, gridY, currFab))
                {
                    currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                }
                
                SpawnCell(gridX + 1, gridY, currFab);
                
                SpawnAdjacent(gridX + 1, gridY, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.openSides & Directions.South) != 0 && gridY - 1 >= 0 && _grid[gridX, gridY - 1] == null)  // If current cell is open on top
            {
                // TODO: Create actual room decision based on adjacent cells not just "room or corridor"
                bool isRoom = existingPrefab.category == PrefabCategory.Corridor;
                
                var currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                while (!IsValidPrefab(gridX, gridY - 1, currFab))
                {
                    currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                }
                
                SpawnCell(gridX, gridY - 1, currFab);
                
                SpawnAdjacent(gridX, gridY - 1, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.openSides & Directions.West) != 0 && gridX - 1 >= 0 && _grid[gridX - 1, gridY] == null)  // If current cell is open on top
            {
                // TODO: Create actual room decision based on adjacent cells not just "room or corridor"
                bool isRoom = existingPrefab.category == PrefabCategory.Corridor;
                
                var currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                while (!IsValidPrefab(gridX - 1, gridY, currFab))
                {
                    currFab = GetWeightedRandom((isRoom ? rooms : corridors));
                }
                
                SpawnCell(gridX - 1, gridY, currFab);
                
                SpawnAdjacent(gridX - 1, gridY, maxDepth, currentDepth + 1);
            }
        }

        bool IsValidPrefab(int gridX, int gridY, PrefabData prefab)
        {
            return MeetsRequiredDir(prefab.openSides, GetRequiredDirections(gridX, gridY)) && AvoidsBlockedDir(prefab.openSides, GetBlockedDirections(gridX, gridY));
        }

        Directions GetRequiredDirections(int gridX, int gridY)  // Check adjacent cells for required directions
        {
            var requiredDirections = Directions.None;
            
            // Top cell
            if (gridY < gridHeight - 2 && _grid[gridX, gridY - 1] != null && (_grid[gridX, gridY - 1].openSides & Directions.South) != 0)  // If cell has South open
            {
                requiredDirections |= Directions.North;  // Require North direction (to join the open south)
            }
            
            // Right cell
            if (gridX < gridWidth - 2 && _grid[gridX + 1, gridY] != null && (_grid[gridX + 1, gridY].openSides & Directions.West) != 0)  // Check West of other
            {
                requiredDirections |= Directions.East;  // If valid set East of current to required
            }
            
            // Bottom cell
            if (gridY > 0 && _grid[gridX, gridY - 1] != null && (_grid[gridX, gridY - 1].openSides & Directions.North) != 0)  // Check North of other
            {
                requiredDirections |= Directions.South;  // If valid set South of current to required
            }
            
            // Left cell
            if (gridX > 0 && _grid[gridX - 1, gridY] != null && (_grid[gridX - 1, gridY].openSides & Directions.East) != 0)  // Check East of other
            {
                requiredDirections |= Directions.West;  // If valid set West of current to required
            }

            return requiredDirections;  // Return complete required directions
        }

        Directions GetBlockedDirections(int gridX, int gridY)
        {
            var blockedDirections = Directions.None;
            
            // Top cell
            if (gridY > gridHeight || (gridY < gridHeight - 2 && _grid[gridX, gridY - 1] != null) &&
                (_grid[gridX, gridY].openSides & Directions.North) == 0)
            {
                blockedDirections |= Directions.North;  // North would be invalid
            }

            return blockedDirections;  // Return complete blocked directions
        }

        bool MeetsRequiredDir(Directions prefabDirections, Directions requiredDirections)
        {
            return (prefabDirections & requiredDirections) == requiredDirections;  // Prefab has at LEAST required dirs
        }

        bool AvoidsBlockedDir(Directions prefabDirections, Directions blockedDirections)
        {
            return (prefabDirections & blockedDirections) != blockedDirections;
        }

        void SpawnCell(int gridX, int gridY, PrefabData prefab)
        {
            var rot = Quaternion.Euler(0, 0, 0);
            Vector3 pos = new Vector3(gridX * cellSize, 0, gridY * cellSize);
            
            _grid[gridX, gridY] = prefab;
            Instantiate(prefab, pos, rot);
        }

        List<PrefabData> FilterPrefabs(List<PrefabCategory> categories = null, List<PrefabType> types = null)
        {
            IEnumerable<PrefabData> query = prefabs;

            if (categories != null && categories.Count > 0)  // Filter categories
                query = query.Where(p => categories.Contains(p.category));
            
            if (types != null && types.Count > 0)
                query = query.Where(p => types.Contains(p.type));
            
            return query.ToList();
        }

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

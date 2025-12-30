using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace Code.Generation
{
    public struct CookedPrefab
    {
        public PrefabData Original;
        public int Rotation;
        public Directions OpenSides;
    }
    public class DungeonGenerator : MonoBehaviour
    {
        public int gridWidth = 20;
        public int gridHeight = 20;
        public float cellSize = 20f;

        [Tooltip("All prefabs the generator can use (make sure PrefabData is set correctly!)")]
        public List<PrefabData> prefabs;  // Source prefabs (not directly spawned, used to cook)
        
        private List<CookedPrefab> _cookedPrefabs;  // Internally used prefabs (includes generated rotations)
        private List<CookedPrefab> _rooms;
        private List<CookedPrefab> _corridors;
        
        private CookedPrefab[,] _grid;

        void Start()
        {
            LoadPrefabs();
            Generate();
        }

        void LoadPrefabs()
        {
            _cookedPrefabs = new List<CookedPrefab>();  // Clear any previously generated prefabs
            _rooms = new List<CookedPrefab>();
            _corridors = new List<CookedPrefab>();
            
            foreach (var prefab in prefabs)
            {
                switch (prefab.type)  // Generate Directions based on type
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
                
                var sides = prefab.openSides;
                for (int i = 0; i < 4; i++)
                {
                    _cookedPrefabs.Add(new CookedPrefab
                    {
                        Original = prefab,
                        Rotation = i * 90,
                        OpenSides = sides
                    });

                    sides = RotateDir(sides);
                }
            }
            
            _rooms = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Room });
            _corridors = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Corridor });
        }

        void Generate()
        {
            _grid = new CookedPrefab[gridWidth, gridHeight]; // Create blank 2D array

            SpawnCell(0, 0, GetWeightedRandom(_rooms)); // Start cell in center (catalyst for generation)

            SpawnAdjacent(0, 0, 2000);
        }

        void SpawnAdjacent(int gridX, int gridY, int maxDepth = 5, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth)
            {
                return;
            }
            
            var existingPrefab = _grid[gridX, gridY];
            var isRoom = existingPrefab.Original.category == PrefabCategory.Corridor;  // TODO: Create actual room decision based on adjacent cells not just "room or corridor"
            
            if ((existingPrefab.OpenSides & Directions.North) != 0 && gridY + 1 < gridHeight && _grid[gridX, gridY + 1].Original == null)  // If current cell is open on top
            {
                SpawnCell(gridX, gridY + 1, FindValidPrefab(gridX, gridY + 1, isRoom));
                SpawnAdjacent(gridX, gridY + 1, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.OpenSides & Directions.East) != 0 && gridX + 1 < gridWidth && _grid[gridX + 1, gridY].Original == null)  // If current cell is open on top
            {
                SpawnCell(gridX + 1, gridY, FindValidPrefab(gridX + 1, gridY, isRoom));
                SpawnAdjacent(gridX + 1, gridY, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.OpenSides & Directions.South) != 0 && gridY - 1 >= 0 && _grid[gridX, gridY - 1].Original == null)  // If current cell is open on top
            {
                SpawnCell(gridX, gridY - 1, FindValidPrefab(gridX, gridY - 1, isRoom));
                SpawnAdjacent(gridX, gridY - 1, maxDepth, currentDepth + 1);
            }
            if ((existingPrefab.OpenSides & Directions.West) != 0 && gridX - 1 >= 0 && _grid[gridX - 1, gridY].Original == null)  // If current cell is open on top
            {
                SpawnCell(gridX - 1, gridY, FindValidPrefab(gridX - 1, gridY, isRoom));
                SpawnAdjacent(gridX - 1, gridY, maxDepth, currentDepth + 1);
            }
        }

        CookedPrefab FindValidPrefab(int gridX, int gridY, bool isRoom)
        {
            var pool = isRoom ? _rooms : _corridors;
            var required = GetRequiredDirections(gridX, gridY);
            var validOptions = new List<CookedPrefab>();

            foreach (var p in pool)
            {
                if ((p.OpenSides & required) == required)
                    validOptions.Add(p);
            }
            
            if (validOptions.Count == 0)
            {
#if UNITY_EDITOR
                throw new InvalidOperationException(
                    $"No valid prefabs found at ({gridX}, {gridY}) with required directions {required}");
#else
                Debug.LogWarning($"No valid prefabs found at ({gridX}, {gridY})");
                return _rooms[0];  // Fallback to prevent crash or empty cells (won't be right!)
#endif
            }

            return GetWeightedRandom(validOptions);
        }
        
        bool IsValidPrefab(int gridX, int gridY, CookedPrefab prefab)
        {
            return MeetsRequiredDir(prefab.OpenSides, GetRequiredDirections(gridX, gridY));  //  && AvoidsBlockedDir(prefab.openSides, GetBlockedDirections(gridX, gridY))
        }

        Directions GetRequiredDirections(int gridX, int gridY)  // Check adjacent cells for required directions
        {
            var requiredDirections = Directions.None;
            
            // Top cell
            if (gridY + 1 < gridHeight && _grid[gridX, gridY + 1].Original != null && (_grid[gridX, gridY + 1].OpenSides & Directions.South) != 0)  // If cell has South open
            {
                requiredDirections |= Directions.North;  // Require North direction (to join the open south)
            }
            
            // Right cell
            if (gridX + 1 < gridWidth && _grid[gridX + 1, gridY].Original != null && (_grid[gridX + 1, gridY].OpenSides & Directions.West) != 0)  // Check West of other
            {
                requiredDirections |= Directions.East;  // If valid set East of current to required
            }
            
            // Bottom cell
            if (gridY - 1 >= 0 && _grid[gridX, gridY - 1].Original != null && (_grid[gridX, gridY - 1].OpenSides & Directions.North) != 0)  // Check North of other
            {
                requiredDirections |= Directions.South;  // If valid set South of current to required
            }
            
            // Left cell
            if (gridX - 1 >= 0 && _grid[gridX - 1, gridY].Original != null && (_grid[gridX - 1, gridY].OpenSides & Directions.East) != 0)  // Check East of other
            {
                requiredDirections |= Directions.West;  // If valid set West of current to required
            }

            return requiredDirections;  // Return complete required directions
        }

        Directions GetBlockedDirections(int gridX, int gridY)
        {
            var blockedDirections = Directions.None;
            
            // Top cell
            if (gridY > gridHeight || _grid[gridX, gridY + 1].Original != null && (_grid[gridX, gridY + 1].OpenSides & Directions.South) == 0)
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
            return (prefabDirections & blockedDirections) == Directions.None;
        }

        void SpawnCell(int gridX, int gridY, CookedPrefab prefab)
        {
            var rot = Quaternion.Euler(0, prefab.Rotation, 0);
            Vector3 pos = transform.position + new Vector3(gridX * cellSize, 0, gridY * cellSize);
            
            _grid[gridX, gridY] = prefab;
            Instantiate(prefab.Original, pos, rot);
        }

        List<CookedPrefab> FilterPrefabs(List<PrefabCategory> categories = null, List<PrefabType> types = null)
        {
            IEnumerable<CookedPrefab> query = _cookedPrefabs;

            if (categories != null && categories.Count > 0)  // Filter categories
                query = query.Where(p => categories.Contains(p.Original.category));
            
            if (types != null && types.Count > 0)
                query = query.Where(p => types.Contains(p.Original.type));
            
            return query.ToList();
        }

        CookedPrefab GetWeightedRandom(params List<CookedPrefab>[] lists)
        {
            var total = lists.SelectMany(list => list).Sum(p => p.Original.spawnWeight);  // Total weight in list
            var roll = Random.Range(0, total);  // Random start

            foreach (var list in lists)
            {
                foreach (var p in list)
                {
                    roll -= p.Original.spawnWeight;
                    if (roll < 0)
                        return p;
                }
            }

            return lists[0][0];
        }

        private Directions RotateDir(Directions dirs)
        {
            Directions result = Directions.None;

            if ((dirs & Directions.North) != 0) result |= Directions.East;
            if ((dirs & Directions.East)  != 0) result |= Directions.South;
            if ((dirs & Directions.South) != 0) result |= Directions.West;
            if ((dirs & Directions.West)  != 0) result |= Directions.North;

            return result;
        }
    }
}

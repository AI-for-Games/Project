using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        public static event Action OnDungeonGenerated;  // Event that fires when generation is done
        
        public int gridWidth = 20;
        public int gridHeight = 20;
        public float cellSize = 20f;
        public float roomChance = 0.6f;

        [Tooltip("All prefabs the generator can use (make sure PrefabData is set correctly!)")]
        public List<PrefabData> prefabs;  // Source prefabs (not directly spawned, used to cook)
        
        private List<CookedPrefab> _cookedPrefabs;  // Internally used prefabs (includes generated rotations)
        private List<CookedPrefab> _rooms;
        private List<CookedPrefab> _corridors;
        
        private CookedPrefab[,] _grid;

        private void Start()
        {
            LoadPrefabs();
            Generate();
            OnDungeonGenerated?.Invoke();  // Trigger finished generation event
        }

        private void LoadPrefabs()
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var sides = prefab.openSides;
                for (var i = 0; i < 4; i++)
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
            
            // Split cooked fabs into rooms and corridors for future use
            _rooms = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Room });
            _corridors = FilterPrefabs(new List<PrefabCategory> { PrefabCategory.Corridor });
            
            // Sort the lists based on spawnWeight (we will check for more common ones first)
            _cookedPrefabs.Sort((a, b) => b.Original.spawnWeight.CompareTo(a.Original.spawnWeight));
            _rooms.Sort((a, b) => b.Original.spawnWeight.CompareTo(a.Original.spawnWeight));
            _corridors.Sort((a, b) => b.Original.spawnWeight.CompareTo(a.Original.spawnWeight));
        }

        private void Generate()
        {
            _grid = new CookedPrefab[gridWidth, gridHeight]; // Create blank 2D array

            SpawnCell(0, 0, FindValidPrefab(0, 0, true));  // Spawn a room at 0,0 to start at
            SpawnCell(gridWidth - 1, gridHeight - 1, FindValidPrefab(gridWidth - 1, gridHeight - 1, true));
            
            SpawnAdjacent(0, 0, 2000);
        }

        private void SpawnAdjacent(int gridX, int gridY, int maxDepth = 5, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth)
                return;

            var existing = _grid[gridX, gridY];

            // Corridors can spawn both types, rooms can only spawn corridors
            var isRoom = existing.Original.category == PrefabCategory.Corridor && RandBool(roomChance);  // TODO: Check adjacent cells, if any are a room (that require a connection) then must be corridor, set while checking?

            // North cell
            if ((existing.OpenSides & Directions.North) != 0 && InBounds(gridX, gridY + 1) &&  // If open and in bounds
                !IsOccupied(gridX, gridY + 1))  // And cell is free (empty)
            {
                SpawnCell(gridX, gridY + 1, FindValidPrefab(gridX, gridY + 1, isRoom));  // Spawn a new cell
                SpawnAdjacent(gridX, gridY + 1, maxDepth, currentDepth + 1);  // Recurse into new cell
            }

            // East cell
            if ((existing.OpenSides & Directions.East) != 0 && InBounds(gridX + 1, gridY) &&
                !IsOccupied(gridX + 1, gridY))
            {
                SpawnCell(gridX + 1, gridY, FindValidPrefab(gridX + 1, gridY, isRoom));
                SpawnAdjacent(gridX + 1, gridY, maxDepth, currentDepth + 1);
            }

            // South cell
            if ((existing.OpenSides & Directions.South) != 0 && InBounds(gridX, gridY - 1) &&
                !IsOccupied(gridX, gridY - 1))
            {
                SpawnCell(gridX, gridY - 1, FindValidPrefab(gridX, gridY - 1, isRoom));
                SpawnAdjacent(gridX, gridY - 1, maxDepth, currentDepth + 1);
            }

            // West cell
            if ((existing.OpenSides & Directions.West) != 0 && InBounds(gridX - 1, gridY) &&
                !IsOccupied(gridX - 1, gridY))
            {
                SpawnCell(gridX - 1, gridY, FindValidPrefab(gridX - 1, gridY, isRoom));
                SpawnAdjacent(gridX - 1, gridY, maxDepth, currentDepth + 1);
            }
        }

        private CookedPrefab FindValidPrefab(int gridX, int gridY, bool isRoom)
        {
            var pool = isRoom ? _rooms : _corridors;
            var validOptions = new List<CookedPrefab>();

            foreach (var p in pool)  // For every fab...
            {
                if (IsValidPrefab(gridX, gridY, p))  // If the fab is valid, add to pool
                {
                    Debug.Log(
                        $"ACCEPTED @ {gridX}, {gridY} | " +
                        $"Required={GetRequiredDirections(gridX, gridY)} | " +
                        $"Blocked={GetBlockedDirections(gridX, gridY)} | " +
                        $"Sides={p.OpenSides}"
                    );
                    validOptions.Add(p);
                }
                else
                {
                    Debug.Log(
                        $"REJECTED @ {gridX}, {gridY} | " +
                        $"Required={GetRequiredDirections(gridX, gridY)} | " +
                        $"Blocked={GetBlockedDirections(gridX, gridY)} | " +
                        $"Sides={p.OpenSides}"
                    );
                }
            }

            if (validOptions.Count != 0) return GetWeightedRandom(validOptions);
            
            // Log reports if there wasn't a valid fab at all.
#if UNITY_EDITOR
            Debug.LogWarning($"FALLBACK @ {gridX}, {gridY}");
#else
            Debug.LogWarning($"No valid prefabs found at ({gridX}, {gridY})");
#endif
            return _rooms[0];  // Fallback to prevent crash or empty cells (won't be right!)
        }

        private Directions GetRequiredDirections(int gridX, int gridY)
        {
            var required = Directions.None;

            // Top cell
            if (IsOccupied(gridX, gridY + 1) && (_grid[gridX, gridY + 1].OpenSides & Directions.South) != 0)
            {
                required |= Directions.North;  // If cell above has a south connection, require a north one to join
            }

            // Right cell
            if (IsOccupied(gridX + 1, gridY) && (_grid[gridX + 1, gridY].OpenSides & Directions.West) != 0)
            {
                required |= Directions.East;
            }

            // Bottom cell
            if (IsOccupied(gridX, gridY - 1) && (_grid[gridX, gridY - 1].OpenSides & Directions.North) != 0)
            {
                required |= Directions.South;
            }

            // Left cell
            if (IsOccupied(gridX - 1, gridY) && (_grid[gridX - 1, gridY].OpenSides & Directions.East) != 0)
            {
                required |= Directions.West;
            }

            return required;
        }


        private Directions GetBlockedDirections(int gridX, int gridY)
        {
            var blocked = Directions.None;

            // Top cell
            if (!InBounds(gridX, gridY + 1) ||  // If out of range then block automatically
                (IsOccupied(gridX, gridY + 1) && (_grid[gridX, gridY + 1].OpenSides & Directions.South) == 0))
            {
                blocked |= Directions.North;  // If cell above doesn't have a south, block a north connection
            }

            // Right cell
            if (!InBounds(gridX + 1, gridY) ||
                (IsOccupied(gridX + 1, gridY) && (_grid[gridX + 1, gridY].OpenSides & Directions.West) == 0))
            {
                blocked |= Directions.East;
            }

            // Bottom cell
            if (!InBounds(gridX, gridY - 1) ||
                (IsOccupied(gridX, gridY - 1) && (_grid[gridX, gridY - 1].OpenSides & Directions.North) == 0))
            {
                blocked |= Directions.South;
            }

            // Left cell
            if (!InBounds(gridX - 1, gridY) ||
                (IsOccupied(gridX - 1, gridY) && (_grid[gridX - 1, gridY].OpenSides & Directions.East) == 0))
            {
                blocked |= Directions.West;
            }

            return blocked;
        }

        private void SpawnCell(int gridX, int gridY, CookedPrefab prefab)
        {
            var rot = Quaternion.Euler(0, prefab.Rotation, 0);
            var pos = transform.position + new Vector3(gridX * cellSize, 0, gridY * cellSize);
            
            _grid[gridX, gridY] = prefab;
            Instantiate(prefab.Original, pos, rot);
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && x < gridWidth &&
                   y >= 0 && y < gridHeight;
        }

        private bool IsOccupied(int x, int y)
        {
            return InBounds(x, y) && _grid[x, y].Original != null;
        }
        
        private bool IsValidPrefab(int gridX, int gridY, CookedPrefab prefab)
        {
            return MeetsRequiredDir(prefab.OpenSides, GetRequiredDirections(gridX, gridY)) &&
                   AvoidsBlockedDir(prefab.OpenSides, GetBlockedDirections(gridX, gridY));
        }
        
        private List<CookedPrefab> FilterPrefabs(List<PrefabCategory> categories = null, List<PrefabType> types = null)
        {
            IEnumerable<CookedPrefab> query = _cookedPrefabs;

            if (categories != null && categories.Count > 0)  // Filter categories
                query = query.Where(p => categories.Contains(p.Original.category));
            
            if (types != null && types.Count > 0)
                query = query.Where(p => types.Contains(p.Original.type));
            
            return query.ToList();
        }
        
        private static bool RandBool(float trueWeight)
        {
            return Random.value < trueWeight;
        }
        
        private static Directions RotateDir(Directions dirs)
        {
            var result = Directions.None;

            if ((dirs & Directions.North) != 0) result |= Directions.East;
            if ((dirs & Directions.East)  != 0) result |= Directions.South;
            if ((dirs & Directions.South) != 0) result |= Directions.West;
            if ((dirs & Directions.West)  != 0) result |= Directions.North;

            return result;
        }
        
        private static bool MeetsRequiredDir(Directions prefabDirections, Directions requiredDirections)
        {
            return (prefabDirections & requiredDirections) == requiredDirections;  // Prefab has at LEAST required dirs
        }

        private static bool AvoidsBlockedDir(Directions prefabDirections, Directions blockedDirections)
        {
            return (prefabDirections & blockedDirections) == Directions.None;
        }
        
        private static CookedPrefab GetWeightedRandom(params List<CookedPrefab>[] lists)
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
    }
}

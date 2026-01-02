using UnityEngine;

namespace Code.Generation
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject enemyPrefab;

        [Header("Spawning")] 
        public float spawnRate;

        private void Start()
        {
        
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
        }
 
        private void SpawnEnemy()
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}

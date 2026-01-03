using UnityEngine;

namespace Code.Generation
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefab")]
        public GameObject enemyPrefab;

        [Header("Enemies")]
        public float spawnRate;
        public Transform enemyTarget;

        public void Init(GameObject enemiesPrefab, float spawnerRate, Transform enemiesTarget)
        {
            enemyPrefab = enemiesPrefab;
            spawnRate = spawnerRate;
            enemyTarget = enemiesTarget;
        }

        private void Start()
        {
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);
        }
 
        private void SpawnEnemy()
        {
            var enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            //enemy.GetComponent<AIMovement>().Init(enemyTarget);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}

using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 10;
    public float spawnRange = 5f;

    // 게임 시작 시 바로 스폰
    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            if (newEnemy.tag != "Enemy")
            {
                Debug.LogWarning($"경고: {enemyPrefab.name} 프리팹에 'Enemy' 태그가 설정되지 않았습니다!", newEnemy);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));
    }
}
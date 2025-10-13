using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyprefab;
    public float spawnIntervel = 3f;
    public float spawnRange = 5f;

    // 스폰 제한을 위한 변수 추가
    public int maxKillCount = 10; // 이 스포너에서 처치해야 할 최대 적의 수
    private int killCount = 0; // 현재까지 처치한 적의 수
    private bool isSpawningStopped = false; // 스폰 중지 여부

    private float timer = 0f;

    void Update()
    {
        // 스폰이 중지되었다면 더 이상 진행하지 않음
        if (isSpawningStopped)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnIntervel)
        {
            timer = 0f;

            Vector3 spawnPos = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );

            // 적을 생성하고, 생성된 적에게 이 스포너 정보를 넘겨줌
            GameObject newEnemyObj = Instantiate(enemyprefab, spawnPos, Quaternion.identity);
            Enemy enemy = newEnemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetSpawner(this);
            }
        }
    }

    // 적이 처치될 때마다 이 함수를 호출하여 카운트를 올림
   
    public void RecordEnemyKilled()
    {   
    if (isSpawningStopped) return;
    
    // GameManager에 처치 기록을 알림
    FindObjectOfType<GameManager>()?.RecordKill();

    killCount++;
    if (killCount >= maxKillCount)
    {
        isSpawningStopped = true;
    }
}
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 1));
    }
}
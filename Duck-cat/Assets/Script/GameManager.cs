using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI enemyCountText;
    public GameObject portalObject;

    private EnemySpawner[] spawners;
    private int totalEnemiesToKill = 0;
    private int killedEnemies = 0;

    void Start()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }

        spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            totalEnemiesToKill += spawner.numberOfEnemiesToSpawn; // 스포너가 생성할 적의 수를 합산
        }

        if (totalEnemiesToKill == 0)
        {
            ActivatePortal();
        }

        UpdateEnemyCountUI();
    }

    public void RecordKill()
    {
        killedEnemies++;
        UpdateEnemyCountUI();

        if (killedEnemies >= totalEnemiesToKill)
        {
            ActivatePortal();
        }
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            int remainingEnemies = totalEnemiesToKill - killedEnemies;
            enemyCountText.text = "Enemies Left: " + remainingEnemies;
        }
    }

    void ActivatePortal()
    {
        if (portalObject != null)
        {
            portalObject.SetActive(true);
            Debug.Log("모든 적을 처치했습니다! 포탈이 나타납니다.");
        }
        
        if (enemyCountText != null)
        {
            enemyCountText.text = "Stage Clear!";
        }
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
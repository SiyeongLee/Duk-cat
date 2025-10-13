using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 사용을 위해 필요
using TMPro; // TextMeshPro 사용을 위해 필요

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI enemyCountText; // 남은 적 수 텍스트 UI

    private EnemySpawner[] spawners;
    private int totalMaxKills = 0;
    private int totalCurrentKills = 0;

    void Start()
    {
        spawners = FindObjectsOfType<EnemySpawner>();
        foreach (var spawner in spawners)
        {
            totalMaxKills += spawner.maxKillCount;
        }
        UpdateEnemyCountUI();
    }

    public void RecordKill()
    {
        totalCurrentKills++;
        UpdateEnemyCountUI();
    }

    void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = "남은 적수: " + (totalMaxKills - totalCurrentKills);
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
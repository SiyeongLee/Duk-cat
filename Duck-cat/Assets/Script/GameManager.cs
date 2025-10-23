using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("게임 상태 UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI enemyCountText;
    // public GameObject portalObject; // 포탈 오브젝트 변수 삭제

    private bool gamePhaseStarted = false; // 튜토리얼 종료 여부 확인용

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 초기 적 수 UI 업데이트 (선택 사항)
        UpdateEnemyCountUIStart();
    }

    // TutorialManager가 튜토리얼 종료 시 호출
    public void StartGamePhase()
    {
        gamePhaseStarted = true;
        Debug.Log("GameManager: 튜토리얼 종료, 게임 단계 시작. 적 수 확인을 시작합니다.");
    }

    void Update()
    {
        // 게임 단계가 시작되었을 때만 실행
        if (gamePhaseStarted)
        {
            // "Enemy" 태그를 가진 오브젝트 수를 실시간으로 확인
            int remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            // UI 업데이트
            if (enemyCountText != null)
            {
                enemyCountText.text = "Enemies Left: " + remainingEnemies;
            }

            // 남은 적이 0이면 스테이지 클리어 로그만 남김 (포탈 활성화 로직 삭제)
            if (remainingEnemies == 0)
            {
                // ActivatePortal(); // 포탈 활성화 호출 삭제
                if (enemyCountText != null)
                {
                    enemyCountText.text = "Stage Clear!";
                }
                // 클리어 조건을 만족했으므로 GameManager의 Update를 멈춤 (선택 사항)
                // this.enabled = false;
            }
        }
    }

    // 게임 시작 시 적 수를 표시하기 위한 초기 함수
    void UpdateEnemyCountUIStart()
    {
        if (enemyCountText != null)
        {
            EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
            int totalEnemies = 0;
            foreach (var spawner in spawners)
            {
                totalEnemies += spawner.numberOfEnemiesToSpawn;
            }
            enemyCountText.text = "Enemies Left: " + totalEnemies;
        }
    }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
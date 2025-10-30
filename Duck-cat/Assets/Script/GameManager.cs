using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("게임 상태 UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI enemyCountText;
    public GameObject portalObject; // 포탈 오브젝트(key) 연결

    private bool portalActivated = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (portalObject != null)
        {
            portalObject.SetActive(false);
        }
        else
        {
            Debug.LogError("GameManager: Portal Object가 Inspector에 연결되지 않았습니다!");
        }
    }

    void Update()
    {
        if (!portalActivated)
        {
            int remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (enemyCountText != null)
            {
                enemyCountText.text = "Enemies Left: " + remainingEnemies;
            }

            if (Time.timeSinceLevelLoad > 0.5f && remainingEnemies == 0)
            {
                ActivatePortal();
            }
        }
    }

    void ActivatePortal()
    {
        if (portalActivated) return;
        portalActivated = true;

        if (portalObject != null)
        {
            portalObject.SetActive(true);

            // 자식 오브젝트와 Mesh Renderer도 강제로 켭니다.
            MeshRenderer[] renderers = portalObject.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = true;
            }

            Debug.Log("GameManager: 모든 적 처치 완료! 포탈 활성화.");
        }
        else
        {
            Debug.LogError("GameManager: 포탈을 활성화하려 했지만 Portal Object가 연결되지 않았습니다!");
        }

        if (enemyCountText != null)
        {
            enemyCountText.text = "Stage Clear!";
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
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneChanger : MonoBehaviour
{
    [Header("설정")]
    public string sceneNameToLoad;
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;

    [Header("UI (선택 사항)")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public string interactionMessage = "[E] 다음 스테이지로 이동"; // 표시할 메시지

    private Transform playerTransform;
    // private PlayerControoller playerController; // 열쇠 확인이 필요 없으므로 삭제

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            // playerController = playerObj.GetComponent<PlayerControoller>(); // 삭제
        }

        if (interactionUI != null) interactionUI.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
                if (interactionText != null)
                {
                    interactionText.text = interactionMessage; // 단순 메시지 표시
                }
            }

            // 열쇠 확인 로직(playerController.hasKey)을 삭제
            if (Input.GetKeyDown(interactionKey))
            {
                LoadNextScene();
            }
        }
        else
        {
            if (interactionUI != null) interactionUI.SetActive(false);
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
        else
        {
            Debug.LogWarning("SceneChanger: 이동할 씬 이름이 설정되지 않았습니다!");
        }
    }
}
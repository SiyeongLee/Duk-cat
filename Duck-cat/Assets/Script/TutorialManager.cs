using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class TutorialManager : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;
    public Button skipButton;

    [Header("튜토리얼 제어 대상")]
    public PlayerControoller playerController;
    public PlayerShooting playerShooting;
    // public EnemySpawner[] enemySpawners; // EnemySpawner 참조 삭제

    [Header("튜토리얼 설정")]
    public float delayAfterAction = 1.5f;

    private int currentStep = 0;
    private bool stepCompleted = false;
    private string[] instructions = {
        "W, A, S, D 키를 사용하여 이동해 보세요.",
        "마우스를 움직여 주변을 둘러보세요.",
        "마우스 왼쪽 버튼을 클릭하여 공격해 보세요.",
        "튜토리얼 완료! 곧 게임이 시작됩니다."
    };

    // GameManager 참조 삭제
    // private GameManager gameManager;

    void Start()
    {
        // GameManager 찾기 삭제
        // gameManager = FindObjectOfType<GameManager>();

        // 튜토리얼 씬 시작 시 플레이어 찾기 (씬에 Player 프리팹이 있어야 함)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerControoller>();
            playerShooting = playerObj.GetComponent<PlayerShooting>();
        }
        else
        {
            Debug.LogError("TutorialManager: 씬에서 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
            this.enabled = false; // 플레이어가 없으면 튜토리얼 진행 불가
            return;
        }

        tutorialPanel.SetActive(true);
        playerController.enabled = false;
        playerShooting.enabled = false;

        // EnemySpawner 비활성화 로직 삭제
        // foreach (var spawner in enemySpawners) ...

        skipButton.onClick.AddListener(SkipTutorial);
        ShowInstruction();
    }

    void Update()
    {
        if (stepCompleted) return;

        switch (currentStep)
        {
            case 0:
                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                { StartCoroutine(AdvanceToNextStep()); }
                break;
            case 1:
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                { StartCoroutine(AdvanceToNextStep()); }
                break;
            case 2:
                if (Input.GetMouseButtonDown(0))
                { StartCoroutine(AdvanceToNextStep()); }
                break;
        }
    }

    IEnumerator AdvanceToNextStep()
    {
        stepCompleted = true;
        yield return new WaitForSeconds(delayAfterAction);
        currentStep++;
        ShowInstruction();
    }

    void ShowInstruction()
    {
        if (currentStep >= instructions.Length)
        {
            EndTutorial();
            return;
        }

        instructionText.text = instructions[currentStep];
        stepCompleted = false;

        playerController.enabled = false;
        playerShooting.enabled = false;

        switch (currentStep)
        {
            case 0:
            case 1:
                playerController.enabled = true;
                break;
            case 2:
                playerShooting.enabled = true;
                break;
            case 3:
                stepCompleted = true;
                StartCoroutine(FinalStep());
                break;
        }
    }

    IEnumerator FinalStep()
    {
        yield return new WaitForSeconds(2f);
        EndTutorial();
    }

    void SkipTutorial()
    {
        EndTutorial();
    }

    void EndTutorial()
    {
        // 튜토리얼이 끝나면 Map 씬 로드
        SceneManager.LoadScene("Map2");
    }
}
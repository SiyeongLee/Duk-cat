using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;
    public Button skipButton;

    [Header("튜토리얼 제어 대상")]
    public PlayerControoller playerController;
    public PlayerShooting playerShooting;
    public EnemySpawner[] enemySpawners; // EnemySpawner 배열 참조 유지

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

    private GameManager gameManager; // GameManager 참조 추가

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // GameManager 찾기

        tutorialPanel.SetActive(true);
        playerController.enabled = false;
        playerShooting.enabled = false;

        // 시작 시 스포너를 비활성화 상태로 둡니다.
        foreach (var spawner in enemySpawners)
        {
            if (spawner != null) spawner.gameObject.SetActive(false); // 오브젝트 자체를 비활성화
        }

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
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (playerController != null) playerController.enabled = true;
        if (playerShooting != null) playerShooting.enabled = true;

        // 스포너 오브젝트들을 활성화하여 스폰 시작
        foreach (var spawner in enemySpawners)
        {
            if (spawner != null) spawner.gameObject.SetActive(true);
        }

        // GameManager에게 튜토리얼 종료를 알림
        if (gameManager != null) gameManager.StartGamePhase();

        this.enabled = false;
    }
}
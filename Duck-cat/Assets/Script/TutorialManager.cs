using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI instructionText;
    public Button confirmButton;
    public Button skipButton;
    public Button finishButton; // 새로 추가한 종료 버튼

    [Header("튜토리얼 제어 대상")]
    public PlayerControoller playerController;
    public PlayerShooting playerShooting;
    public EnemySpawner[] enemySpawners;

    private int currentStep = 0;
    private bool isActionDone = false;
    private string[] instructions = {
        "W, A, S, D 키를 사용하여 이동해 보세요.",
        "마우스 좌우로 움직여 보세요.",
        "마우스 왼쪽 버튼을 클릭하여 공격해 보세요.",
        "이동할수 있는 포탈은 모든 적을 처치해야 생성됩니다 E키를 눌러 포탈을 타고이동할수 있습니다",
        "튜토리얼 완료! 이제 크리스탈을 지키세요."
    };

    void Start()
    {
        tutorialPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        finishButton.gameObject.SetActive(false); // 시작 시 종료 버튼 비활성화

        playerController.enabled = false;
        playerShooting.enabled = false;

        foreach (var spawner in enemySpawners)
        {
            spawner.enabled = false;
        }

        // 각 버튼에 함수 연결
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        skipButton.onClick.AddListener(SkipTutorial);
        finishButton.onClick.AddListener(EndTutorial); // 종료 버튼에 EndTutorial 함수 연결

        ShowNextInstruction();
    }

    void Update()
    {
        if (isActionDone) return;

        switch (currentStep)
        {
            case 0: // 이동 튜토리얼
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    ActionCompleted();
                }
                break;
            case 1: // 공격 튜토리얼
                if (Input.GetMouseButtonDown(0))
                {
                    ActionCompleted();
                }
                break;
        }
    }

    void ShowNextInstruction()
    {
        isActionDone = false;
        confirmButton.gameObject.SetActive(false);
        
        instructionText.text = instructions[currentStep];

        // 마지막 튜토리얼 단계일 경우
        if (currentStep == instructions.Length - 1)
        {
            isActionDone = true; // 더 이상 Update에서 행동을 감지하지 않음
            finishButton.gameObject.SetActive(true); // 종료 버튼 활성화
            skipButton.gameObject.SetActive(false); // 건너뛰기 버튼 비활성화
        }
        else // 마지막 단계가 아닐 경우
        {
            switch (currentStep)
            {
                case 0:
                    playerController.enabled = true;
                    break;
                case 1:
                    playerShooting.enabled = true;
                    break;
            }
        }
    }

    void ActionCompleted()
    {
        isActionDone = true;
        confirmButton.gameObject.SetActive(true);
    }

    void OnConfirmButtonClicked()
    {
        currentStep++;
        ShowNextInstruction();
    }

    void SkipTutorial()
    {
        Debug.Log("튜토리얼을 건너뜁니다.");
        EndTutorial();
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        playerController.enabled = true;
        playerShooting.enabled = true;

        foreach (var spawner in enemySpawners)
        {
            spawner.enabled = true;
        }

        this.enabled = false;
    }
}
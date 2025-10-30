using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("설정 패널 UI 요소")]
    public GameObject settingsPanel; // Hierarchy에 있는 SettingsPanel 연결
    public Slider masterVolumeSlider;
    public Slider mouseSensitivitySlider;
    public Toggle fullscreenToggle;
    public Button backButton; // SettingsPanel의 뒤로가기 버튼

    void Start()
    {
        // SettingsManager가 없으면 생성
        if (SettingsManager.instance == null)
        {
            Instantiate(new GameObject("SettingsManager")).AddComponent<SettingsManager>();
        }

        // SettingsManager에 UI 요소들을 넘겨서 설정
        SettingsManager.instance.SetupUI(
            settingsPanel,
            masterVolumeSlider,
            mouseSensitivitySlider,
            fullscreenToggle,
            backButton
        );

        // 마우스 커서 활성화
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // '게임 시작' 버튼에 연결
    public void StartGame()
    {
        // 'Map' 또는 'Tutorial' 씬 이름
        SceneManager.LoadScene("Tutorial");
    }

    // '설정' 버튼에 연결
    public void OpenSettings()
    {
        SettingsManager.instance.ToggleSettingsPanel(true);
    }

    // '게임 종료' 버튼에 연결
    public void ExitGame()
    {
        Application.Quit();
    }
}
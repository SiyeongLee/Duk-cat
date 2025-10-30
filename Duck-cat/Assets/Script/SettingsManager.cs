using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    [Header("연결된 UI 요소")]
    private GameObject settingsPanel;
    private Slider masterVolumeSlider;
    private Slider mouseSensitivitySlider;
    private Toggle fullscreenToggle;

    // 설정값 변수
    private float masterVolume = 1f;
    private float mouseSensitivity = 10f;
    private bool isFullscreen = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings(); // 게임 시작 시 저장된 값 불러오기
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TitleManager가 UI 요소들을 연결해줄 때 호출됨
    public void SetupUI(GameObject panel, Slider volume, Slider sensitivity, Toggle fullscreen, Button backButton)
    {
        settingsPanel = panel;
        masterVolumeSlider = volume;
        mouseSensitivitySlider = sensitivity;
        fullscreenToggle = fullscreen;

        UpdateUIElements(); // UI에 현재 값 표시

        // 이벤트 리스너 연결
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        backButton.onClick.AddListener(() => ToggleSettingsPanel(false));
    }

    // 설정 창 열기/닫기
    public void ToggleSettingsPanel(bool state)
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(state);
            if (state) UpdateUIElements(); // 켤 때마다 값 동기화
        }
    }

    // --- 설정 변경 및 저장 함수 ---

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume;
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        this.isFullscreen = isFullscreen;
        Screen.fullScreen = this.isFullscreen;
        PlayerPrefs.SetInt("IsFullscreen", this.isFullscreen ? 1 : 0);
    }

    // --- 데이터 불러오기 ---

    private void LoadSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 10f);
        isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;

        AudioListener.volume = masterVolume;
        Screen.fullScreen = isFullscreen;
    }

    private void UpdateUIElements()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.value = masterVolume;
        if (mouseSensitivitySlider != null) mouseSensitivitySlider.value = mouseSensitivity;
        if (fullscreenToggle != null) fullscreenToggle.isOn = isFullscreen;
    }
}
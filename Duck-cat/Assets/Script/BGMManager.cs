using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // 이 매니저의 인스턴스를 저장할 static 변수
    public static BGMManager instance;

    private AudioSource bgmSource;

    void Awake()
    {
        // --- 싱글톤(Singleton) 패턴 ---
        // 씬에 BGMManager 인스턴스가 아직 없는지 확인
        if (instance == null)
        {
            // 이 인스턴스를 static 변수에 할당
            instance = this;

            // 씬이 변경되어도 이 게임 오브젝트가 파괴되지 않도록 설정
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 만약 씬에 BGMManager가 이미 존재한다면 (예: 타이틀 씬으로 돌아온 경우)
            // 이 오브젝트는 중복이므로 파괴합니다.
            Destroy(gameObject);
            return;
        }

        // 이 오브젝트에 붙어있는 AudioSource 컴포넌트를 가져옵니다.
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            Debug.LogError("BGMManager에 AudioSource 컴포넌트가 없습니다!");
        }
    }

    // (참고) 나중에 다른 스크립트에서 BGM을 바꾸고 싶을 때 이 함수를 사용할 수 있습니다.
    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmSource.clip == newClip) return; // 같은 음악이면 무시

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
    }
}
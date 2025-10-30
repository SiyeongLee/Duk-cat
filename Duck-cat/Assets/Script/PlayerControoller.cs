using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class PlayerControoller : MonoBehaviour
{
    public float speed = 5f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;
    public CinemachineVirtualCamera virtualCam;
    public float rotationSpeed = 10f;
    private CinemachinePOV pov;
    private CharacterController controller;
    private Vector3 velocity;
    public bool isGrounded;

    public int maxHP = 100;
    private int currentHp;
    public Slider hpSlider;

    [Header("사망 효과")]
    public GameObject deathEffectPrefab;
    public GameObject playerModel;

    private float originalSpeed;
    private bool isDead = false;

    // 열쇠 소지 여부 변수 추가
    [HideInInspector] // Inspector 창에는 보이지 않도록 함
    public bool hasKey = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();
        currentHp = maxHP;
        hpSlider.value = 1f;
        originalSpeed = speed;
        hasKey = false; // 게임 시작 시 열쇠 없음
    }

    void Update()
    {
        if (isDead) return;

        // PlayerPrefs에서 저장된 마우스 감도 값을 실시간으로 불러와 적용
        // SettingsManager가 저장한 값을 사용 (기본값 10f)
        float sensitivityMultiplier = PlayerPrefs.GetFloat("MouseSensitivity", 10f) / 10f;
        if (pov != null)
        {
            pov.m_HorizontalAxis.m_MaxSpeed = 300f * sensitivityMultiplier;
            pov.m_VerticalAxis.m_MaxSpeed = 2f * sensitivityMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
            pov.m_VerticalAxis.Value = 0f;
        }


        CinemacineSwitcher switcher = FindObjectOfType<CinemacineSwitcher>();
        isGrounded = controller.isGrounded;

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = 10f;
            virtualCam.m_Lens.FieldOfView = 80f;
        }
        else
        {
            virtualCam.m_Lens.FieldOfView = 60f;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 camForward = virtualCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = virtualCam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        if (switcher != null && switcher.usingFreeLook) // switcher가 null일 경우 대비
        {
            x = 0;
            z = 0;
        }

        Vector3 move = (camForward * z + camRight * x).normalized;
        controller.Move(move * currentSpeed * Time.deltaTime);

        float cameraYaw = pov.m_HorizontalAxis.Value;
        Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHp -= damage;
        hpSlider.value = (float)currentHp / maxHP;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        if (deathEffectPrefab != null) Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        if (playerModel != null) playerModel.SetActive(false);
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerShooting>().enabled = false;
        this.enabled = false;
        FindObjectOfType<GameManager>()?.GameOver(); // null 체크 추가
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHP) currentHp = maxHP;
        hpSlider.value = (float)currentHp / maxHP;
    }

    public void SpeedUp(float boostAmount, float duration)
    {
        StopCoroutine("SpeedBoostCoroutine");
        StartCoroutine(SpeedBoostCoroutine(boostAmount, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float duration)
    {
        speed += boostAmount;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    // Key 스크립트가 호출할 함수
    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("열쇠를 획득했습니다!");
        // 여기에 열쇠 획득 시 UI 메시지 표시 등의 로직 추가 가능
    }
}
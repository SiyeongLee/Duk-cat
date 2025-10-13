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

    // 사망 효과 프리팹을 연결할 변수 추가
    [Header("사망 효과")]
    public GameObject deathEffectPrefab;
    
    private float originalSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        currentHp = maxHP;
        hpSlider.value = 1f;
        
        originalSpeed = speed;
    }

    void Update()
    {
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

        if (switcher.usingFreeLook)
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
        currentHp -= damage;
        hpSlider.value = (float)currentHp / maxHP;
        if (currentHp <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // 사망 효과 프리팹이 할당되어 있다면, 플레이어 위치에 생성
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHP)
        {
            currentHp = maxHP;
        }
        hpSlider.value = (float)currentHp / maxHP;
        Debug.Log($"체력 {amount} 회복! 현재 HP: {currentHp}");
    }

    public void SpeedUp(float boostAmount, float duration)
    {
        StopCoroutine("SpeedBoostCoroutine");
        StartCoroutine(SpeedBoostCoroutine(boostAmount, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostAmount, float duration)
    {
        speed += boostAmount;
        Debug.Log($"스피드 업! {duration}초 동안 속도 증가!");

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        Debug.Log("스피드 버프 종료. 원래 속도로 돌아갑니다.");
    }
}
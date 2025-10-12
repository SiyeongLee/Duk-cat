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

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        currentHp = maxHP;
        hpSlider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
            pov.m_VerticalAxis.Value = 0f;
        }
        CinemacineSwitcher switcher = FindObjectOfType<CinemacineSwitcher>();
        isGrounded = controller.isGrounded;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 10f;
            virtualCam.m_Lens.FieldOfView = 80f;

        }
        else
        {
            speed = 5f;
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
        controller.Move(move * speed * Time.deltaTime);

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
        Destroy(gameObject);
    }

    


}





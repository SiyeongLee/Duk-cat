using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState State = EnemyState.Idle;
    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 6f;
    public float attackcooldown = 1.5f;

    public GameObject projectileprefab;
    public Transform firePoint;

    private float lastAttackTime;
    public int maxHp = 5;
    private int currentHp;
    public Slider hpSlider;

    [Header("플레이어 버프 설정")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float speedBoostAmount = 3f;
    [SerializeField] private float speedBoostDuration = 5f;
    
    [Header("사망 효과")]
    public GameObject deathEffectPrefab;

    private Transform player;
    private EnemySpawner spawner; // 자신을 생성한 스포너를 저장할 변수

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackcooldown;
        currentHp = maxHp;
        if (hpSlider != null)
        {
            hpSlider.value = 1f;
        }
    }

    void Update()
    {
        if (player == null) return;
        float dist = Vector3.Distance(player.position, transform.position);

        if (currentHp <= maxHp * 0.2f && State != EnemyState.Idle)
        {
            State = EnemyState.RunAway;
        }

        switch (State)
        {
            case EnemyState.Idle:
                if (dist < traceRange)
                    State = EnemyState.Trace;
                break;
            case EnemyState.Trace:
                if (dist < attackRange)
                    State = EnemyState.Attack;
                else if (dist > traceRange)
                    State = EnemyState.Idle;
                else
                    TracePlayer();
                break;
            case EnemyState.Attack:
                if (dist > attackRange)
                    State = EnemyState.Trace;
                else
                    AttackPlayer();
                break;
            case EnemyState.RunAway:
                if (dist > traceRange)
                    State = EnemyState.Idle;
                else
                    Runaway();
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = (float)currentHp / maxHp;
        }
        if (currentHp <= 0)
        {
            Die();
        }
    }
    
    // 스포너 정보를 설정하는 함수 추가
    public void SetSpawner(EnemySpawner _spawner)
    {
        spawner = _spawner;
    }

    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackcooldown)
        {
            lastAttackTime = Time.time;
            Shootprojectile();
        }
    }

    void Shootprojectile()
    {
        if (projectileprefab != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject proj = Instantiate(projectileprefab, firePoint.position, firePoint.rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                ep.SetDirection(dir);
            }
        }
    }

    void Runaway()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position -= dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // 자신을 생성한 스포너에게 처치되었음을 알림
        if (spawner != null)
        {
            spawner.RecordEnemyKilled();
        }

        PlayerControoller playerController = FindObjectOfType<PlayerControoller>();
        if (playerController != null)
        {
            ApplyRandomBuff(playerController);
        }

        Destroy(gameObject);
    }
    
    private void ApplyRandomBuff(PlayerControoller player)
    {
        if (Random.Range(0, 2) == 0)
        {
            Debug.Log("체력 회복 버프!");
            player.Heal(healAmount);
        }
        else
        {
            Debug.Log("스피드 업 버프!");
            player.SpeedUp(speedBoostAmount, speedBoostDuration);
        }
    }
}
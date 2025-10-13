using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState State = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 6f;
    public float attackCooldown = 1.5f;

    public GameObject projectileprefab;
    public Transform firePoint;

    private float lastAttackTime;
    public int maxHp = 5;
    private int currentHp;
    public Slider hpSlider;

    [Header("플레이어 버프/디버프 설정")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float speedBoostAmount = 3f;
    [SerializeField] private float speedBoostDuration = 5f;
    [SerializeField] private int penaltyDamage = 10;

    [Header("사망 효과")]
    public GameObject deathEffectPrefab;
    
    private Transform player;
    private Crystal crystal;
    private Transform currentTarget; // 현재 공격 목표

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        crystal = FindObjectOfType<Crystal>();

        lastAttackTime = -attackCooldown;
        currentHp = maxHp;
        if (hpSlider != null)
        {
            hpSlider.value = 1f;
        }
    }

    void Update()
    {
        // 매 프레임마다 공격 대상을 찾습니다.
        FindTarget();

        // 공격할 대상이 없으면 아무것도 하지 않습니다.
        if (currentTarget == null)
        {
            State = EnemyState.Idle;
            return;
        }

        float dist = Vector3.Distance(currentTarget.position, transform.position);
        
        // 체력이 낮으면 도망가는 로직은 그대로 유지합니다.
        if (currentHp <= maxHp * 0.2f)
        {
            State = EnemyState.RunAway;
        }

        switch (State)
        {
            case EnemyState.Idle:
                // 대기 상태에서도 목표가 추적 범위 안에 들어오면 바로 추적 시작
                if (dist < traceRange)
                    State = EnemyState.Trace;
                break;
            case EnemyState.Trace:
                if (dist < attackRange)
                    State = EnemyState.Attack;
                else if (dist > traceRange) // 추적 범위를 벗어나면 다시 대기
                    State = EnemyState.Idle;
                else
                    TraceTarget();
                break;
            case EnemyState.Attack:
                if (dist > attackRange)
                    State = EnemyState.Trace;
                else
                    AttackTarget();
                break;
            case EnemyState.RunAway:
                if (dist > traceRange * 1.2f)
                    State = EnemyState.Idle;
                else
                    Runaway();
                break;
        }
    }

    // AI 로직을 단순화: 플레이어와 크리스탈 중 더 가까운 대상을 공격
    void FindTarget()
    {
        float playerDist = float.MaxValue;
        float crystalDist = float.MaxValue;

        if (player != null)
            playerDist = Vector3.Distance(player.position, transform.position);

        if (crystal != null)
            crystalDist = Vector3.Distance(crystal.transform.position, transform.position);

        // 플레이어가 크리스탈보다 가까우면 플레이어를 목표로 설정
        if (playerDist < crystalDist)
        {
            currentTarget = player;
        }
        // 그렇지 않다면 크리스탈을 목표로 설정 (플레이어나 크리스탈이 없을 경우 null이 됨)
        else
        {
            currentTarget = (crystal != null) ? crystal.transform : player;
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
    
    void TraceTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(currentTarget.position);
    }

    void AttackTarget()
    {
        transform.LookAt(currentTarget.position);
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootProjectile();
        }
    }

    void ShootProjectile()
    {
        if (projectileprefab != null && firePoint != null && currentTarget != null)
        {
            GameObject proj = Instantiate(projectileprefab, firePoint.position, firePoint.rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                Vector3 dir = (currentTarget.position - firePoint.position).normalized;
                ep.SetDirection(dir);
            }
        }
    }

    void Runaway()
    {
        Vector3 dir = (transform.position - currentTarget.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        FindObjectOfType<GameManager>()?.RecordKill();

        PlayerControoller playerController = FindObjectOfType<PlayerControoller>();
        if (playerController != null)
        {
            ApplyRandomEffect(playerController);
        }

        Destroy(gameObject);
    }
    
    private void ApplyRandomEffect(PlayerControoller player)
    {
        int randomIndex = Random.Range(0, 3);

        if (randomIndex == 0)
        {
            player.Heal(healAmount);
        }
        else if (randomIndex == 1)
        {
            player.SpeedUp(speedBoostAmount, speedBoostDuration);
        }
        else
        {
            player.TakeDamage(penaltyDamage);
        }
    }
}
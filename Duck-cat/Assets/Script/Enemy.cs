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

    [Header("플레이어 버프 설정")]
    [SerializeField] private int healAmount = 20;
    [SerializeField] private float speedBoostAmount = 3f;
    [SerializeField] private float speedBoostDuration = 5f;

    [Header("사망 효과")]
    public GameObject deathEffectPrefab;
    
    // --- AI 개선을 위한 변수 ---
    [Header("AI 설정")]
    public float selfDefenseRange = 5f; // 이 거리 안으로 플레이어가 들어오면 자신을 방어하기 위해 공격합니다.
    public float aggroDuration = 5f;    // 공격받았을 때 플레이어에게 화가 나 있는 시간(초)입니다.
    
    private Transform player;
    private Crystal crystal;
    private Transform currentTarget; // 현재 공격 목표 (플레이어 또는 크리스탈)
    private EnemySpawner spawner;
    private bool isAggroed = false; // 플레이어에게 화가 나 있는지 여부

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
        // 매 프레임마다 가장 적절한 공격 대상을 찾습니다.
        if (!FindTarget())
        {
            State = EnemyState.Idle; // 공격할 대상이 없으면 대기 상태로 변경
            return;
        }

        float dist = Vector3.Distance(currentTarget.position, transform.position);

        // 체력이 낮으면 도망가는 로직 (현재 목표로부터 도망)
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
                else if (dist > traceRange && !isAggroed) // 화가 나있지 않을 때만 추적을 멈춤
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
                if (dist > traceRange * 1.2f) // 추적 범위보다 조금 더 멀어지면 대기 상태로
                    State = EnemyState.Idle;
                else
                    Runaway();
                break;
        }
    }

    // AI의 핵심: 공격할 목표를 결정하는 함수
    bool FindTarget()
    {
        // 플레이어와 크리스탈이 모두 없으면 목표를 찾지 않음
        if (player == null && crystal == null)
        {
            currentTarget = null;
            return false;
        }

        // 크리스탈이 파괴되었다면 플레이어만 공격
        if (crystal == null)
        {
            currentTarget = player;
            return player != null;
        }
        
        // 플레이어가 죽었다면 크리스탈만 공격
        if (player == null)
        {
            currentTarget = crystal.transform;
            return crystal != null;
        }

        float playerDist = Vector3.Distance(player.position, transform.position);

        // 우선순위 1: 공격받아서 화가 났다면 플레이어를 공격
        if (isAggroed)
        {
            currentTarget = player;
        }
        // 우선순위 2: 플레이어가 너무 가까우면 (자기 방어) 플레이어를 공격
        else if (playerDist < selfDefenseRange)
        {
            currentTarget = player;
        }
        // 우선순위 3: 그 외의 모든 경우, 기본 목표인 크리스탈을 공격
        else
        {
            currentTarget = crystal.transform;
        }

        return true;
    }
    
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (hpSlider != null)
        {
            hpSlider.value = (float)currentHp / maxHp;
        }

        // 플레이어에게 공격받으면 일정 시간 동안 화가 난 상태가 됨
        if (player != null)
        {
            StopCoroutine("AggroTimer"); // 이미 화난 상태라면 타이머 초기화
            StartCoroutine(AggroTimer());
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // 일정 시간 동안만 '화난' 상태를 유지하는 코루틴
    IEnumerator AggroTimer()
    {
        isAggroed = true;
        yield return new WaitForSeconds(aggroDuration);
        isAggroed = false;
    }
    
    public void SetSpawner(EnemySpawner _spawner)
    {
        spawner = _spawner;
    }

    void TraceTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(currentTarget.position);
    }

    void AttackTarget()
    {
        transform.LookAt(currentTarget.position); // 공격 시에도 목표를 바라보도록 수정
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
        // 현재 목표로부터 멀어지는 방향으로 이동 (뒤돌아서 도망)
        Vector3 dir = (transform.position - currentTarget.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir); // 도망가는 방향을 바라봄
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

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
            player.Heal(healAmount);
        }
        else
        {
            player.SpeedUp(speedBoostAmount, speedBoostDuration);
        }
    }
}
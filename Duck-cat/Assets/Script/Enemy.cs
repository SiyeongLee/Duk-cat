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

    [Header("AI 설정")]
    public float selfDefenseRange = 5f;
    public float aggroDuration = 5f;

    private Transform player;
    private Crystal crystal;
    private Transform currentTarget;
    private bool isAggroed = false;

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
        if (!FindTarget())
        {
            State = EnemyState.Idle;
            return;
        }

        float dist = Vector3.Distance(currentTarget.position, transform.position);

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
                else if (dist > traceRange && !isAggroed)
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

    bool FindTarget()
    {
        if (player == null && crystal == null)
        {
            currentTarget = null;
            return false;
        }

        if (crystal == null)
        {
            currentTarget = player;
            return player != null;
        }

        if (player == null)
        {
            currentTarget = crystal.transform;
            return crystal != null;
        }

        float playerDist = Vector3.Distance(player.position, transform.position);

        if (isAggroed)
        {
            currentTarget = player;
        }
        else if (playerDist < selfDefenseRange)
        {
            currentTarget = player;
        }
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

        if (player != null)
        {
            StopCoroutine("AggroTimer");
            StartCoroutine(AggroTimer());
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    IEnumerator AggroTimer()
    {
        isAggroed = true;
        yield return new WaitForSeconds(aggroDuration);
        isAggroed = false;
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

        // *** 이 부분이 오류의 원인이었습니다! ***
        // FindObjectOfType<GameManager>()?.RecordKill(); // 이 줄을 삭제하거나 주석 처리합니다.

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
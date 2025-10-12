using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway}
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
    public void TakeDamage(int damage)
    {
       
        currentHp -= damage;
        hpSlider.value = (float)currentHp / maxHp;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private Transform player; 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackcooldown;
        currentHp = maxHp;
        hpSlider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        float dist = Vector3.Distance(player.position, transform.position);
        
        if(currentHp <= maxHp * 0.2f && State != EnemyState.Idle)
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

        Destroy(gameObject);
    }
}

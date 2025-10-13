using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    public int Weapondamage = 1;


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(Weapondamage);
            Destroy(gameObject);
        }
        // 크리스탈에 부딪혔을 때의 로직 추가
        else if (other.CompareTag("Crystal"))
        {
            Crystal crystal = other.GetComponent<Crystal>();
            if (crystal != null) crystal.TakeDamage(Weapondamage);
            Destroy(gameObject);
        }
    }
}
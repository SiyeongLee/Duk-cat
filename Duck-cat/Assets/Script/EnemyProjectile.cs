using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 2;
    public float speed = 8f;
    public float lifeTime = 3f;

    private Vector3 moveDir;

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir.normalized;
    }
    
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어에게 부딪혔을 때
        if (other.CompareTag("Player"))
        {
            PlayerControoller pc = other.GetComponent<PlayerControoller>();
            if (pc != null) pc.TakeDamage(damage);
            
            Destroy(gameObject);
        }
        // 크리스탈에게 부딪혔을 때
        else if (other.CompareTag("Crystal"))
        {
            Crystal crystal = other.GetComponent<Crystal>();
            if (crystal != null) crystal.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
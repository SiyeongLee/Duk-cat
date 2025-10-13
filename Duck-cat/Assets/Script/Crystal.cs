using UnityEngine;
using UnityEngine.UI;

public class Crystal : MonoBehaviour
{
    public int maxHp = 1000;
    private int currentHp;
    public Slider hpSlider;
    public GameObject deathEffectPrefab;

    void Start()
    {
        currentHp = maxHp;
        if (hpSlider != null)
        {
            hpSlider.value = 1f;
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

   void Die()
{
    if (deathEffectPrefab != null)
    {
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }
    
    // GameManager를 찾아 GameOver 함수 호출
    FindObjectOfType<GameManager>().GameOver();
    
    Destroy(gameObject);
}
}
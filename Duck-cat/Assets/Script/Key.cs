using UnityEngine;

public class Key : MonoBehaviour
{
    // public float rotationSpeed = 100f; // 회전 속도 변수 삭제 또는 주석 처리
    public GameObject pickupEffectPrefab;

    /*
    // Update 함수 자체를 삭제하거나 주석 처리합니다.
    void Update()
    {
        // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // 회전 코드 삭제
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControoller player = other.GetComponent<PlayerControoller>();
            if (player != null)
            {
                player.CollectKey();

                if (pickupEffectPrefab != null)
                {
                    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }
}
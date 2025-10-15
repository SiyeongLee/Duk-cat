using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectileprefab;
    public GameObject projectileprefab2;
    public Transform FirePoint;

    // 'Crystal' 레이어를 무시하기 위한 LayerMask 변수 추가
    public LayerMask layerMask;

    private Camera cam;
    private bool isSpecial = false;

    void Start()
    {
        cam = Camera.main;
        // 'Crystal' 레이어를 제외한 모든 레이어를 공격 대상으로 설정
        layerMask = ~(1 << LayerMask.NameToLayer("Crystal"));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            WeaponChange();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPoint;

        // Raycast를 사용하여 공격 목표 지점을 정합니다.
        // Crystal 레이어는 무시하고, 최대 100의 거리까지 감지합니다.
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 아무것도 맞지 않았다면, 카메라 정면 100미터 지점을 목표로 합니다.
            targetPoint = ray.GetPoint(100f);
        }

        Vector3 direction = (targetPoint - FirePoint.position).normalized;

        if (isSpecial)
        {
            Instantiate(projectileprefab2, FirePoint.position, Quaternion.LookRotation(direction));
        }
        else
        {
            Instantiate(projectileprefab, FirePoint.position, Quaternion.LookRotation(direction));
        }
    }

    void WeaponChange()
    {
        isSpecial = !isSpecial;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectileprefab;
    public GameObject projectileprefab2;
    public Transform FirePoint;

    Camera cam;

    bool isSpecial = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
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
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(50f);
        Vector3 direction = (targetPoint - FirePoint.position).normalized;

        if (isSpecial)
        {
            GameObject proj = Instantiate(projectileprefab2, FirePoint.position, Quaternion.LookRotation(direction));
        }
        else
        {

        GameObject proj = Instantiate(projectileprefab, FirePoint.position, Quaternion.LookRotation(direction)); 
        }

    }
    void WeaponChange()
    {
        isSpecial = !isSpecial;
    }
    




}


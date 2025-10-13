using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [Header("설정")]
    public string sceneNameToLoad;
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject interactionUI;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

            if (Input.GetKeyDown(interactionKey))
            {
                LoadNextScene();
            }
        }
        else
        {
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneNameToLoad))
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
        else
        {
            Debug.LogWarning("SceneChanger 스크립트에 이동할 씬 이름이 설정되지 않았습니다!");
        }
    }
}
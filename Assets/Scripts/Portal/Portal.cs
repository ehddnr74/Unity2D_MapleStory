using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // 이동할 씬의 이름
    public Transform spawnPositionObject; // 스폰 위치 오브젝트

    private bool playerIsInPortal = false;
    private SceneFader sceneFader;
    private void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>();
    }

    void Update()
    {
        // 플레이어가 포탈 안에 있고 위쪽 방향키를 눌렀을 때 씬 이동
        if (playerIsInPortal && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (spawnPositionObject != null)
            {
                // 플레이어 위치 저장
                Vector3 spawnPosition = spawnPositionObject.position;
                PlayerPrefs.SetFloat("SpawnPosX", spawnPosition.x);
                PlayerPrefs.SetFloat("SpawnPosY", spawnPosition.y);
                PlayerPrefs.SetFloat("SpawnPosZ", spawnPosition.z);
                PlayerPrefs.Save();

                // 씬 전환 시 페이딩 효과 호출
                if (sceneFader != null)
                {
                    sceneFader.FadeToScene(sceneToLoad);
                }

                else
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInPortal = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInPortal = false;
        }
    }
}

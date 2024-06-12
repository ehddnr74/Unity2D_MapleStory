using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // �̵��� ���� �̸�
    public Transform spawnPositionObject; // ���� ��ġ ������Ʈ

    private bool playerIsInPortal = false;
    private SceneFader sceneFader;
    private void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>();
    }

    void Update()
    {
        // �÷��̾ ��Ż �ȿ� �ְ� ���� ����Ű�� ������ �� �� �̵�
        if (playerIsInPortal && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (spawnPositionObject != null)
            {
                // �÷��̾� ��ġ ����
                Vector3 spawnPosition = spawnPositionObject.position;
                PlayerPrefs.SetFloat("SpawnPosX", spawnPosition.x);
                PlayerPrefs.SetFloat("SpawnPosY", spawnPosition.y);
                PlayerPrefs.SetFloat("SpawnPosZ", spawnPosition.z);
                PlayerPrefs.Save();

                // �� ��ȯ �� ���̵� ȿ�� ȣ��
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

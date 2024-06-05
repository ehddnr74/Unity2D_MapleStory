using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public bool useSavedPosition = true; // ����� ��ġ�� ������� ����
    public Vector3 initialPosition; // �ʱ� ��ġ

    void Start()
    {
        Vector3 spawnPosition;

        if (useSavedPosition)
        {
            float spawnPosX = PlayerPrefs.GetFloat("SpawnPosX", initialPosition.x); // �⺻���� �ʱ� ��ġ
            float spawnPosY = PlayerPrefs.GetFloat("SpawnPosY", initialPosition.y);
            float spawnPosZ = PlayerPrefs.GetFloat("SpawnPosZ", initialPosition.z);

            spawnPosition = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        }
        else
        {
            spawnPosition = initialPosition;
        }

        GameObject player = FindObjectOfType<Player>().gameObject;
        if (player != null)
        {
            player.transform.position = spawnPosition;
        }
        else
        {
            Debug.LogWarning("Player not found!");
        }
    }
}

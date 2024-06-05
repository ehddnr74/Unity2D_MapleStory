using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public bool useSavedPosition = true; // 저장된 위치를 사용할지 여부
    public Vector3 initialPosition; // 초기 위치

    void Start()
    {
        Vector3 spawnPosition;

        if (useSavedPosition)
        {
            float spawnPosX = PlayerPrefs.GetFloat("SpawnPosX", initialPosition.x); // 기본값은 초기 위치
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

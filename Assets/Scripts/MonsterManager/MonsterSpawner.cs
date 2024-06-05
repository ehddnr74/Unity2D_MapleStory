using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public MonsterManager monsterManager;
    public GameObject someMonsterPrefab;
    public Transform[] spawnPoints; // 배열로 스폰 포인트를 저장

    public int maxMonsters = 10; // 최대 마리수
    private int currentMonsterCount = 0; // 현재 활성화된 몬스터 수

    void Start()
    {
        // 예시로 5초마다 몬스터를 스폰
        InvokeRepeating("SpawnMonster", 2.0f, 5.0f);
    }


    void SpawnMonster()
    {

        if (currentMonsterCount >= maxMonsters)
        {
            return; // 최대 마리수에 도달하면 스폰하지 않음
        }

        // 랜덤으로 스폰 포인트 선택
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject monster = monsterManager.GetMonsterFromPool(someMonsterPrefab);
        if (monster != null)
        {
            monster.transform.position = spawnPoint.position; // 선택된 스폰 포인트에 위치 설정
            currentMonsterCount++;
        }
    }

    void DespawnMonster(GameObject monster)
    {
        monsterManager.ReturnMonsterToPool(monster, someMonsterPrefab);
        currentMonsterCount--;
    }
}


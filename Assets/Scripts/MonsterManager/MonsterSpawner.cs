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
        // 시작 시에 스폰 포인트에 초기 몬스터 스폰
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnMonster(spawnPoints[i]);
        }
    }


    void SpawnMonster(Transform spawnPoint)
    {

        if (currentMonsterCount >= maxMonsters)
        {
            return; // 최대 마리수에 도달하면 스폰하지 않음
        }

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


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
            monster.SetActive(true); // 몬스터 활성화
            currentMonsterCount++;
        }
    }

    public void DespawnMonster(GameObject monster)
    {
        monsterManager.ReturnMonsterToPool(monster, someMonsterPrefab);
        currentMonsterCount--;

        // 일정 시간 후 다시 스폰
        StartCoroutine(RespawnAfterDelay(monster, 5f)); // 5초 후 다시 스폰
    }

    private IEnumerator RespawnAfterDelay(GameObject monster, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 스폰 위치를 무작위로 선택
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        SpawnMonster(spawnPoint);
    }
}


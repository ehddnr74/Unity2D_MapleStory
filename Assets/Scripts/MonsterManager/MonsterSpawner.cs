using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public MonsterManager monsterManager;
    public GameObject someMonsterPrefab;
    public Transform[] spawnPoints; // �迭�� ���� ����Ʈ�� ����

    public int maxMonsters = 10; // �ִ� ������
    private int currentMonsterCount = 0; // ���� Ȱ��ȭ�� ���� ��

    void Start()
    {
        // ���� �ÿ� ���� ����Ʈ�� �ʱ� ���� ����
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnMonster(spawnPoints[i]);
        }
    }


    void SpawnMonster(Transform spawnPoint)
    {
        if (currentMonsterCount >= maxMonsters)
        {
            return; // �ִ� �������� �����ϸ� �������� ����
        }

        GameObject monster = monsterManager.GetMonsterFromPool(someMonsterPrefab);

        if (monster != null)
        {
            monster.transform.position = spawnPoint.position; // ���õ� ���� ����Ʈ�� ��ġ ����
            monster.SetActive(true); // ���� Ȱ��ȭ
            currentMonsterCount++;
        }
    }

    public void DespawnMonster(GameObject monster)
    {
        monsterManager.ReturnMonsterToPool(monster, someMonsterPrefab);
        currentMonsterCount--;

        // ���� �ð� �� �ٽ� ����
        StartCoroutine(RespawnAfterDelay(monster, 5f)); // 5�� �� �ٽ� ����
    }

    private IEnumerator RespawnAfterDelay(GameObject monster, float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���� ��ġ�� �������� ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        SpawnMonster(spawnPoint);
    }
}


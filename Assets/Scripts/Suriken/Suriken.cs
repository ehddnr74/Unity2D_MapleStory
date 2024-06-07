using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class Shuriken : MonoBehaviour
{
    public Player player;
    public ShurikenManager shurikenManager;


    public float speed = 10f;
    public float lifetime = 0.5f; // 수리검이 발사된 후 풀에 반환되기까지의 시간

    public bool direction;  // false가 왼쪽으로 나아감

    private Transform target; // 추적할 몬스터의 위치
    public float trackingRange = 5f; // 추적 범위

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        shurikenManager = FindObjectOfType<ShurikenManager>();
        if (!player.flipX)
        {
            direction = false;
        }
        else
        {
            direction = true;
        }

        // 몬스터 추적 코루틴 시작
        StartCoroutine(TrackEnemy());

        // 일정 시간이 지난 후 수리검을 풀에 반환하는 코루틴 시작
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void Update()
    {
        if (target != null)
        {
            // 몬스터가 있는 방향으로 표창 이동
            Vector2 targetDirection = target.position - transform.position;
            transform.Translate(targetDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            // 몬스터를 추적하지 않으면 설정된 방향으로 표창 이동
            if (!direction)
            {
                this.transform.Translate(Vector2.left * speed * Time.deltaTime);
            }
            else
            {
                this.transform.Translate(Vector2.right * speed * Time.deltaTime);
            }
        }
    }

    private IEnumerator TrackEnemy()
    {
        while (true)
        {
            // 몬스터를 추적하는 코드
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance && distanceToEnemy < trackingRange)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform;
            }
            else
            {
                target = null;
            }

            yield return new WaitForSeconds(0.5f); // 매 0.5초마다 몬스터 추적
        }
    }

    private IEnumerator ReturnToPoolAfterDelay()
    {
        yield return new WaitForSeconds(lifetime); // 일정 시간(1초) 대기

        // 수리검을 풀에 반환
        if (shurikenManager != null)
        {
            shurikenManager.ReturnSurikenToPool(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            shurikenManager.ReturnSurikenToPool(gameObject);
        }
    }
}

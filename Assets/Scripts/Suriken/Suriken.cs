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
    public float trackingRange; // 추적 범위

    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        shurikenManager = FindObjectOfType<ShurikenManager>();

        direction = player.flipX;
        // 표창 발사 시 범위 내 적 감지
        CheckForEnemiesInRange();

        // 일정 시간이 지난 후 수리검을 풀에 반환하는 코루틴 시작
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    void OnDisable()
    {
        target = null;
    }

    void Update()
    {
        if (target != null)
        {
            // 몬스터가 있는 방향으로 표창 이동
            Vector2 targetDirection = (target.position - transform.position).normalized;
            transform.Translate(targetDirection * speed * Time.deltaTime);
        }
        else
        {
            // 몬스터를 추적하지 않으면 설정된 방향으로 표창 이동
            Vector2 moveDirection = direction ? Vector2.right : Vector2.left;
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    private void CheckForEnemiesInRange()
    {
        Vector2 directionVector = direction ? Vector2.right : Vector2.left;
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 2); // 반 높이로 시야 범위 설정

        // 플레이어 위치에서 살짝 오른쪽 또는 왼쪽에 탐지 범위의 시작점 계산
        Vector2 offset = direction ? Vector2.right * 0.5f : Vector2.left * 0.5f;
        Vector2 startPosition = (Vector2)player.transform.position + offset;

        // 사각형 범위 내의 적을 감지
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(startPosition + directionVector * trackingRange / 2, boxSize, 0);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.gameObject;
                }
            }
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
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

    private void OnDrawGizmos()
    {
        // 선택된 상태에서 추적 범위를 시각화
        Gizmos.color = Color.red;
        Vector2 directionVector = direction ? Vector2.right : Vector2.left;
        Vector2 boxSize = new Vector2(trackingRange, trackingRange / 2);
        Gizmos.DrawWireCube(transform.position + (Vector3)directionVector * trackingRange / 2, boxSize);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemData : MonoBehaviour
{
    public Item item; // 드랍된 아이템의 데이터

    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Item newItem)
    {
        item = newItem;
        spriteRenderer.sprite = item.Icon;

        // 아이템 드랍 애니메이션 시작
        StartCoroutine(DropAnimation());
    }
    private IEnumerator DropAnimation()
    {
        float animationDuration = 1.0f; // 애니메이션 지속 시간
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position;
        rb.isKinematic = true; // 애니메이션 동안 중력의 영향을 받지 않도록 설정

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // 위로 올라갔다가 내려오는 위치 계산 최대높이가 4.0f가 됨
            float height = Mathf.Sin(t * Mathf.PI) * 5.0f;
            transform.position = new Vector3(initialPosition.x, initialPosition.y + height, initialPosition.z);

            // 초당 두바퀴 회전
            transform.Rotate(Vector3.forward, 720 * Time.deltaTime);

            yield return null;
        }

        // 애니메이션이 끝나면 중력에 의해 하강
        rb.isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            // 아이템이 Ground에 닿았을 때의 로직
            rb.velocity = Vector2.zero; // 아이템의 속도를 0으로 설정
            rb.isKinematic = true; // 아이템을 바닥에 고정

            StartCoroutine(BounceAnimation());
        }
    }
    private IEnumerator BounceAnimation()
    {
        float bounceHeight = 0.1f; // 위아래로 움직일 높이
        Vector3 initialPosition = transform.position;

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < 3.0f)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Sin(elapsedTime * Mathf.PI); // 1초에 1번 왕복

                transform.position = new Vector3(initialPosition.x, initialPosition.y + t * bounceHeight, initialPosition.z);
                yield return null;
            }
        }
    }
}

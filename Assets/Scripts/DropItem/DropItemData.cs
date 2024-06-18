using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemData : MonoBehaviour
{
    public Item item; // ����� �������� ������
    private ItemPool itemPool;

    private SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public bool pickUp = false;
    private Transform playerTransform; // �÷��̾��� Ʈ������

    private Coroutine bounceCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Item newItem, ItemPool pool)
    {
        item = newItem;
        itemPool = pool;
        spriteRenderer.sprite = item.Icon;

        // ������ ��� �ִϸ��̼� ����
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DropAnimation());
        }
    }
    private IEnumerator DropAnimation()
    {
        float animationDuration = 1.0f; // �ִϸ��̼� ���� �ð�
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position;
        rb.isKinematic = true; // �ִϸ��̼� ���� �߷��� ������ ���� �ʵ��� ����

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // ���� �ö󰬴ٰ� �������� ��ġ ��� �ִ���̰� 4.0f�� ��
            float height = Mathf.Sin(t * Mathf.PI) * 5.0f;
            transform.position = new Vector3(initialPosition.x, initialPosition.y + height, initialPosition.z);

            // �ʴ� �ι��� ȸ��
            transform.Rotate(Vector3.forward, 720 * Time.deltaTime);

            yield return null;
        }

        // �ִϸ��̼��� ������ �߷¿� ���� �ϰ�
        rb.isKinematic = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MonsterGround") && gameObject.activeInHierarchy)
        {
            // �������� Ground�� ����� ���� ����
            rb.velocity = Vector2.zero; // �������� �ӵ��� 0���� ����
            rb.isKinematic = true; // �������� �ٴڿ� ����

            bounceCoroutine = StartCoroutine(BounceAnimation());
        }
    }
    private IEnumerator BounceAnimation()
    {
        float bounceHeight = 0.1f; // ���Ʒ��� ������ ����
        Vector3 initialPosition = transform.position;

        while (true)
        {
            if (pickUp)
            {
                PickUpAnim();
                yield break; // �ڷ�ƾ ����
            }

            float elapsedTime = 0f;

            while (elapsedTime < 3.0f)
            {
                if (pickUp)
                {
                    PickUpAnim();
                    yield break; // �ڷ�ƾ ����
                }
                elapsedTime += Time.deltaTime;
                float t = Mathf.Sin(elapsedTime * Mathf.PI); // 1�ʿ� 1�� �պ�

                transform.position = new Vector3(initialPosition.x, initialPosition.y + t * bounceHeight, initialPosition.z);
                yield return null;
            }
        }
    }

    public void PickUpAnim()
    {
        if (bounceCoroutine != null)
        {
            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;
        }

        // �÷��̾��� Ʈ�������� ã�� �����մϴ�.
        playerTransform = FindObjectOfType<Player>().transform;

        StartCoroutine(PickUpAnimation());
    }

    private IEnumerator PickUpAnimation()
    {
        float animationDuration = 0.2f; // �Ⱦ� �ִϸ��̼� ���� �ð�
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = playerTransform.position;//+ new Vector3(0f, 0.f, 0f); // ��ǥ ��ġ�� �÷��̾� ��ġ�� ����



        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;

            // ���� �ö󰡴� �ִϸ��̼�
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            yield return null;
        }

        pickUp = false;
        // Ǯ�� ��ȯ
        ReturnToPool();
    
}

        public void ReturnToPool()
    {
        if (itemPool != null)
        {
            itemPool.ReturnItem(gameObject);
        }
    }
}

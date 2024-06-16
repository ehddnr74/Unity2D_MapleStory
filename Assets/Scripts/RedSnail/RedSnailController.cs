using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using static RedSnailController;
using Random = UnityEngine.Random;


public class RedSnailController : MonoBehaviour
{
    public enum RedSnailState
    {
        Stand,
        Move,
        Hit,
        Die,
    }

    private DamageTextManager damageTextManager;

    public RedSnailState mRedSnailState;
    private MonsterSpawner monsterSpawner; // ���� ������ ����
    private ItemDataBase itemDataBase; // ������ �����ͺ��̽� ����


    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float moveTime = 3f; // �̵� �ð�
    public float idleTime = 1f; // ��� �ð�
    public float moveSpeed = 3f;
    private float stateChangeTime;
    private float moveDir;

    private bool isChangingDirection = false; // ���� ���� ������ ���θ� ��Ÿ���� �÷���

    public int maxHealth = 500; // �ִ� ü��
    private int currentHealth;

    private bool hitCheck;
    private bool dieCheck;

    public List<DropItem> dropTable; // ��� ���̺� �߰�
    public GameObject itemPrefab; // ������ ������

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = FindObjectOfType<MonsterSpawner>();
        itemDataBase = FindObjectOfType<ItemDataBase>();

        mRedSnailState = RedSnailState.Stand;

        currentHealth = maxHealth;

        hitCheck = false;
        dieCheck = false;

        SetMoveDir();
        stateChangeTime = Time.time;
    }

    public void TakeDamage(Vector3 Position, int damage, bool isCritical, bool luckySeven, bool secondSuriken)
    {
        damageTextManager.ShowDamage(Position, damage, isCritical);
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            if (!luckySeven || (luckySeven && !secondSuriken))
            {
                dieCheck = true;
            }
        }
        else
        {
            hitCheck = true;
        }
    }

    private void Update()
    {
        SetSpriteDir(moveDir);

        switch (mRedSnailState)
        {
            case RedSnailState.Stand:
                stand();
                break;

            case RedSnailState.Move:
                move();
                break;

            case RedSnailState.Hit:
                hit();
                break;

            case RedSnailState.Die:
                die();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (mRedSnailState == RedSnailState.Move)
        {
            mRigidBody.velocity = new Vector2(moveDir * moveSpeed, mRigidBody.velocity.y);
        }
    }

    private void stand()
    {
        // ���� �ð� ��� �� �̵� ���·� ��ȯ
        if (Time.time - stateChangeTime >= idleTime)
        {
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsMoving", true);
            mRedSnailState = RedSnailState.Move;
            SetMoveDir();
            stateChangeTime = Time.time;
        }

        if(hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetTrigger("IsHitting");
            mRedSnailState = RedSnailState.Hit;
        }

        if(dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }

    private void move()
    {
        // �̵� �ð��� ������ ��� ���·� ��ȯ
        if (Time.time - stateChangeTime >= moveTime)
        {
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
            stateChangeTime = Time.time;
        }

        // ������ ���� ��ġ�� x ��ǥ 46.2�� �Ѿ�� �ݴ� �������� �̵�
        if (transform.position.x > 46.2f || transform.position.x < -76.17f)
        {
            moveDir *= -1f; // �̵� ������ �ݴ�� ����
            SetSpriteDir(moveDir);
            isChangingDirection = true; // ���� ���� �� �÷��׸� ����
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= 46.2f && transform.position.x >= -76.17f)
        {
            isChangingDirection = false;
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetTrigger("IsHitting");
            mRedSnailState = RedSnailState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }
    private void hit()
    {
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        mRigidBody.velocity = Vector2.zero; // �ǰ� �߿��� �������� ����
        yield return new WaitForSeconds(0.15f); // �ǰ� �ִϸ��̼� ��� �ð�
        if (currentHealth > 0) // �ǰ� �� ü���� ���� ���� ���� Stand ���·� ��ȯ
        {
            mAnimator.ResetTrigger("IsHitting"); // Ʈ���� �缳��
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
        }
        else
        {
            mAnimator.SetBool("IsDying", true);
            mRedSnailState = RedSnailState.Die;
        }
    }
    private void die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        col.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        mRigidBody.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        yield return new WaitForSeconds(1.0f);
        col.enabled = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mRedSnailState = RedSnailState.Stand;
        currentHealth = maxHealth; // �ʱ� ü���� �ִ� ü������ ����
        DropItems(); // ������ ��� ���� �߰�
        monsterSpawner.DespawnMonster(gameObject); // ���͸� Ǯ�� ��ȯ

    }

    private void DropItems()
    {
        int cnt = 0; // ������ �������� ����������� �ٸ��� �ϱ� ���� ���� 
        if (itemDataBase == null) return;

        foreach (var dropItem in dropTable)
        {
            if (Random.value <= dropItem.dropChance)
            {
                cnt++;
                Item item = itemDataBase.FetchItemByID(dropItem.itemId);
                Debug.Log(item.Name);
                if (item != null)
                {
                    GameObject droppedItem = Instantiate(itemPrefab, transform.position + new Vector3(cnt * 1.6f, 0f, 0f), Quaternion.identity);
                    DropItemData dropItemData = droppedItem.GetComponent<DropItemData>();
                    if (dropItemData != null)
                    {
                        dropItemData.Initialize(item);
                    }
                }
            }
        }
    }

    private void SetMoveDir()
    {
        moveDir = Random.Range(-1f, 1f);
        SetSpriteDir(moveDir);
    }

    private void SetSpriteDir(float dir)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (dir < 0) // ���� �̵�
        {
            spriteRenderer.flipX = false;
        }
        else if (dir > 0) // ������ �̵�
        {
            spriteRenderer.flipX = true;
        }
    }
}

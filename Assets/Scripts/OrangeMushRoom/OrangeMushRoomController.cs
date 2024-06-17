using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static GreenMushRoomController;

public class OrangeMushRoomController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    Animator mAnimator;

    public ItemPool itemPool; // ������ Ǯ ����
    private DamageTextManager damageTextManager;
    public OrangeMushRoomState mOrangeMushRoomState;
    private MonsterSpawner monsterSpawner; // ���� ������ ����
    private ItemDataBase itemDataBase; // ������ �����ͺ��̽� ����

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

    public Transform leftBoundary; // ���� ���
    public Transform rightBoundary; // ������ ���

    public float jumpForce = 10f; // ���� ��

    private bool isGround;

    private int experience = 10; // ȹ�� ����ġ��

    public bool onceAddExperience = true;


    public enum OrangeMushRoomState
    {
        Stand,
        Move,
        Jump,
        Hit,
        Die

    }

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = GameObject.Find("OrangeMushRoomSpawner").GetComponent<MonsterSpawner>();
        itemDataBase = FindObjectOfType<ItemDataBase>();
        itemPool = GameObject.Find("ItemPoolManager").GetComponent<ItemPool>();

        mOrangeMushRoomState = OrangeMushRoomState.Stand;

        currentHealth = maxHealth;

        // ��� ������Ʈ�� �������� ����
        leftBoundary = GameObject.Find("GMRLeftBoundary").transform;
        rightBoundary = GameObject.Find("GMRRightBoundary").transform;

        SetMoveDir();
        stateChangeTime = Time.time;
    }
    void Update()
    {
        SetSpriteDir(moveDir);

        switch (mOrangeMushRoomState)
        {
            case OrangeMushRoomState.Stand:
                stand();
                break;

            case OrangeMushRoomState.Move:
                move();
                break;
            case OrangeMushRoomState.Jump:
                jump();
                break;
            case OrangeMushRoomState.Hit:
                hit();
                break;

            case OrangeMushRoomState.Die:
                die();
                break;
        }
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

    private void FixedUpdate()
    {
        if (isGround)
        {
            if (mOrangeMushRoomState == OrangeMushRoomState.Move)
            {
                rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);
            }
        }
    }

    private void stand()
    {
        // ���� �ð� ��� �� �̵� ���·� ��ȯ
        if (Time.time - stateChangeTime >= idleTime)
        {
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsMoving", true);
            mOrangeMushRoomState = OrangeMushRoomState.Move;
            SetMoveDir();
            stateChangeTime = Time.time;
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetTrigger("IsHitting");
            mOrangeMushRoomState = OrangeMushRoomState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsStanding", false);
            mAnimator.SetBool("IsDying", true);
            mOrangeMushRoomState = OrangeMushRoomState.Die;
        }
    }
    private void move()
    {
        // �̵� �ð��� ������ ��� ���·� ��ȯ
        if (Time.time - stateChangeTime >= moveTime)
        {
            if (isGround)
            {
                isGround = false;
                mAnimator.SetBool("IsMoving", false);
                mAnimator.SetBool("IsJumping", true);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                mOrangeMushRoomState = OrangeMushRoomState.Jump;
                stateChangeTime = Time.time;
            }
        }
        // ������ ���� ��ġ�� ��踦 �Ѿ�� �ݴ� �������� �̵�
        if (!isChangingDirection)
        {
            if (transform.position.x > rightBoundary.position.x || transform.position.x < leftBoundary.position.x)
            {
                moveDir *= -1f; // �̵� ������ �ݴ�� ����
                SetSpriteDir(moveDir);
                isChangingDirection = true; // ���� ���� �� �÷��׸� ����
            }
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= (rightBoundary.position.x - 1f) && transform.position.x >= (leftBoundary.position.x + 1f))
        {
            isChangingDirection = false;
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetTrigger("IsHitting");
            mOrangeMushRoomState = OrangeMushRoomState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsDying", true);
            mOrangeMushRoomState = OrangeMushRoomState.Die;
        }
    }

    private void jump()
    {
        // ������ ���� ��ġ�� ��踦 �Ѿ�� �ݴ� �������� �̵�
        if (!isChangingDirection)
        {
            if (transform.position.x > rightBoundary.position.x || transform.position.x < leftBoundary.position.x)
            {
                moveDir *= -1f; // �̵� ������ �ݴ�� ����
                rb.velocity = new Vector2(-rb.velocity.x, jumpForce);
                SetSpriteDir(moveDir);
                isChangingDirection = true; // ���� ���� �� �÷��׸� ����
            }
        }

        // ���� ���� ���̰�, ������ x ��ǥ�� �ٽ� ���� ���� ������ ���� �� �÷��׸� ����
        if (isChangingDirection && transform.position.x <= (rightBoundary.position.x - 1f) && transform.position.x >= (leftBoundary.position.x + 1f))
        {
            isChangingDirection = false;
        }

        if (isGround)
        {
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsStanding", true);
            mOrangeMushRoomState = OrangeMushRoomState.Stand; // ��� ���·� ��ȯ
            stateChangeTime = Time.time;
        }
        else
        {
            rb.gravityScale = 5f; // �߷��� ���������� ����
        }

        if (hitCheck)
        {
            hitCheck = false;
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetTrigger("IsHitting");
            mOrangeMushRoomState = OrangeMushRoomState.Hit;
        }

        if (dieCheck)
        {
            dieCheck = false;
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsDying", true);
            mOrangeMushRoomState = OrangeMushRoomState.Die;
        }
    }

    private void hit()
    {
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        rb.velocity = Vector2.zero; // �ǰ� �߿��� �������� ����
        yield return new WaitForSeconds(0.15f); // �ǰ� �ִϸ��̼� ��� �ð�
        if (currentHealth > 0 && isGround) // �ǰ� �� ü���� ���� ���� ���� Stand ���·� ��ȯ
        {
            mAnimator.ResetTrigger("IsHitting"); // Ʈ���� �缳��
            mAnimator.SetBool("IsStanding", true);
            mOrangeMushRoomState = OrangeMushRoomState.Stand;
        }
        else if(currentHealth >0 && !isGround)
        {
            mAnimator.ResetTrigger("IsHitting");
            mAnimator.SetBool("IsJumping", true);
            mOrangeMushRoomState = OrangeMushRoomState.Jump;
        }
        else
        {
            mAnimator.SetBool("IsDying", true);
            mOrangeMushRoomState = OrangeMushRoomState.Die;
        }
    }
    private void die()
    {
        if (onceAddExperience)
        {
            onceAddExperience = false;
            DataManager.instance.AddExperience(experience);
        }
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        col.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        rb.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        yield return new WaitForSeconds(1.0f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mOrangeMushRoomState = OrangeMushRoomState.Stand;
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
                    Vector3 dropPosition = transform.position + new Vector3(cnt * 1.6f, 0f, 0f);
                    GameObject droppedItem = itemPool.GetItem(dropPosition, Quaternion.identity);
                    DropItemData dropItemData = droppedItem.GetComponent<DropItemData>();
                    if (dropItemData != null)
                    {
                        dropItemData.Initialize(item,itemPool);
                    }
                }
            }
        }
    }

    private void SetMoveDir()
    {
        moveDir = Random.value < 0.5f ? -1f : 1f;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MonsterGround"))
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            isGround = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MonsterGround"))
        {
            isGround = false;
        }
    }

}

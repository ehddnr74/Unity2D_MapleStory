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

    public ItemPool itemPool; // 아이템 풀 참조
    private DamageTextManager damageTextManager;
    public OrangeMushRoomState mOrangeMushRoomState;
    private MonsterSpawner monsterSpawner; // 몬스터 스포너 참조
    private ItemDataBase itemDataBase; // 아이템 데이터베이스 참조

    public float moveTime = 3f; // 이동 시간
    public float idleTime = 1f; // 대기 시간
    public float moveSpeed = 3f;
    private float stateChangeTime;
    private float moveDir;

    private bool isChangingDirection = false; // 방향 변경 중인지 여부를 나타내는 플래그

    public int maxHealth = 500; // 최대 체력
    private int currentHealth;

    private bool hitCheck;
    private bool dieCheck;

    public List<DropItem> dropTable; // 드랍 테이블 추가
    public GameObject itemPrefab; // 아이템 프리팹

    public Transform leftBoundary; // 왼쪽 경계
    public Transform rightBoundary; // 오른쪽 경계

    public float jumpForce = 10f; // 점프 힘

    private bool isGround;

    private int experience = 10; // 획득 경험치량

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

        // 경계 오브젝트를 동적으로 참조
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
        // 일정 시간 대기 후 이동 상태로 전환
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
        // 이동 시간이 지나면 대기 상태로 전환
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
        // 몬스터의 현재 위치가 경계를 넘어가면 반대 방향으로 이동
        if (!isChangingDirection)
        {
            if (transform.position.x > rightBoundary.position.x || transform.position.x < leftBoundary.position.x)
            {
                moveDir *= -1f; // 이동 방향을 반대로 변경
                SetSpriteDir(moveDir);
                isChangingDirection = true; // 방향 변경 중 플래그를 설정
            }
        }

        // 방향 변경 중이고, 몬스터의 x 좌표가 다시 일정 범위 안으로 들어올 때 플래그를 리셋
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
        // 몬스터의 현재 위치가 경계를 넘어가면 반대 방향으로 이동
        if (!isChangingDirection)
        {
            if (transform.position.x > rightBoundary.position.x || transform.position.x < leftBoundary.position.x)
            {
                moveDir *= -1f; // 이동 방향을 반대로 변경
                rb.velocity = new Vector2(-rb.velocity.x, jumpForce);
                SetSpriteDir(moveDir);
                isChangingDirection = true; // 방향 변경 중 플래그를 설정
            }
        }

        // 방향 변경 중이고, 몬스터의 x 좌표가 다시 일정 범위 안으로 들어올 때 플래그를 리셋
        if (isChangingDirection && transform.position.x <= (rightBoundary.position.x - 1f) && transform.position.x >= (leftBoundary.position.x + 1f))
        {
            isChangingDirection = false;
        }

        if (isGround)
        {
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsStanding", true);
            mOrangeMushRoomState = OrangeMushRoomState.Stand; // 대기 상태로 전환
            stateChangeTime = Time.time;
        }
        else
        {
            rb.gravityScale = 5f; // 중력을 정상적으로 설정
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
        rb.velocity = Vector2.zero; // 피격 중에는 움직이지 않음
        yield return new WaitForSeconds(0.15f); // 피격 애니메이션 재생 시간
        if (currentHealth > 0 && isGround) // 피격 후 체력이 남아 있을 때만 Stand 상태로 전환
        {
            mAnimator.ResetTrigger("IsHitting"); // 트리거 재설정
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
        col.enabled = false; // 콜라이더 비활성화
        rb.velocity = Vector2.zero; // 속도 초기화
        yield return new WaitForSeconds(1.0f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mOrangeMushRoomState = OrangeMushRoomState.Stand;
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
        DropItems(); // 아이템 드랍 로직 추가
        monsterSpawner.DespawnMonster(gameObject); // 몬스터를 풀로 반환

    }

    private void DropItems()
    {
        int cnt = 0; // 아이템 개수별로 드랍포지션을 다르게 하기 위한 변수 
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
        if (dir < 0) // 왼쪽 이동
        {
            spriteRenderer.flipX = false;
        }
        else if (dir > 0) // 오른쪽 이동
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

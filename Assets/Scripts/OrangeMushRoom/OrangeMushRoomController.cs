using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrangeMushRoomController : MonoBehaviour
{
    private string monsterName = "주황 버섯";
    public Transform nameTagPosition; // 네임태그를 표시할 위치 (예: 몬스터의 머리 위)
    private GameObject nameTagInstance; // 생성된 네임태그 인스턴스
    private TextMeshProUGUI nameTagText; // 네임태그의 텍스트 컴포넌트
    private NameTagPool nameTagPool; // 네임태그 풀링 시스템

    public Transform hpBarPosition; // HP 바를 표시할 위치 (예: 몬스터의 머리 위)
    private GameObject hpBarInstance; // 생성된 HP 바 인스턴스
    private Slider hpSlider; // HP 바의 Slider 컴포넌트
    private HpBarPool hpBarPool; // HP 바 풀링 시스템

    private AudioSource audioSource;
    public AudioClip HitSound;
    public AudioClip DieSound;

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

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
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

        hpBarPool = GameObject.Find("HpBarCanvas").GetComponent<HpBarPool>();
        nameTagPool = GameObject.Find("NameTagCanvas").GetComponent<NameTagPool>();
        currentHealth = maxHealth;

        // HP 바 인스턴스 생성 및 캔버스에 추가
        hpBarInstance = hpBarPool.GetHPBar();
        hpSlider = hpBarInstance.GetComponent<Slider>();
        hpSlider.maxValue = maxHealth;
        hpSlider.value = currentHealth;

        // 경계 오브젝트를 동적으로 참조
        leftBoundary = GameObject.Find("GMRLeftBoundary").transform;
        rightBoundary = GameObject.Find("GMRRightBoundary").transform;

        // 네임태그 풀링 시스템 초기화
        nameTagInstance = nameTagPool.GetNameTag();
        nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
        nameTagText.text = monsterName; // 몬스터 이름 설정

        SetMoveDir();
        stateChangeTime = Time.time;

        // 초기화
        hitCheck = false;
        dieCheck = false;
        isGround = false;
        isChangingDirection = false;
        onceAddExperience = true;

        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mAnimator.ResetTrigger("IsHitting");
    }

    private void OnDisable()
    {
        if (hpBarInstance != null)
        {
            hpBarPool.ReturnHPBar(hpBarInstance);
            hpBarInstance = null;
        }

        if (nameTagInstance != null)
        {
            nameTagPool.ReturnNameTag(nameTagInstance);
            nameTagInstance = null;
        }
    }


    void Update()
    {
        if (hpBarInstance != null)
        {
            // 몬스터 위치에 따라 HP 바 위치 업데이트
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(hpBarPosition.position);
            hpBarInstance.transform.position = screenPosition;
        }

        if (nameTagInstance != null)
        {
            // 몬스터 위치에 따라 네임태그 위치 업데이트
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(nameTagPosition.position);
            nameTagInstance.transform.position = screenPosition;
        }

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
            if (luckySeven)
            {
                if (!secondSuriken)
                {
                    // 첫 번째 표창에 맞았을 때 죽이지 않도록 변경
                    StartCoroutine(WaitForSecondShuriken());
                    PlaySound(DieSound, 0.2f);
                }
                else
                {
                    // 두 번째 표창에 맞았을 때도 적이 죽도록 처리
                    StartCoroutine(LateDie());
                    PlaySound(DieSound, 0.2f);
                }
            }
            else
            {
                // 럭키세븐이 아닌 경우에는 첫 번째 표창에 맞았을 때 죽도록 처리
                dieCheck = true;
                PlaySound(DieSound, 0.2f);
            }
        }
        else
        {
            hitCheck = true;
            PlaySound(HitSound, 0.2f);
        }
        hpSlider.value = currentHealth;
    }

    private IEnumerator WaitForSecondShuriken()
    {
        yield return new WaitForSeconds(0.2f); // 두 번째 표창이 맞을 시간을 기다림
        if (currentHealth <= 0)
        {
            dieCheck = true;
        }
    }

    private IEnumerator LateDie()
    {
        yield return new WaitForSeconds(0.2f);

        dieCheck = true;
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
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        rb.velocity = Vector2.zero; // 피격 중에는 움직이지 않음
        yield return new WaitForSeconds(0.15f); // 피격 애니메이션 재생 시간
        if (currentHealth > 0)
        {
            if (isGround) // 피격 후 체력이 남아 있을 때만 Stand 상태로 전환
            {
                mAnimator.ResetTrigger("IsHitting"); // 트리거 재설정
                mAnimator.SetBool("IsStanding", true);
                mOrangeMushRoomState = OrangeMushRoomState.Stand;
            }
            else
            {
                mAnimator.ResetTrigger("IsHitting");
                mAnimator.SetBool("IsJumping", true);
                mOrangeMushRoomState = OrangeMushRoomState.Jump;
            }
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
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mOrangeMushRoomState = OrangeMushRoomState.Stand;
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
        DropItems(); // 아이템 드랍 로직 추가
        monsterSpawner.DespawnMonster(gameObject); // 몬스터를 풀로 반환

        // 추가 초기화
        hitCheck = false;
        dieCheck = false;
        isGround = false;
        isChangingDirection = false;

        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mAnimator.ResetTrigger("IsHitting");

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

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            audioSource.volume = volume;
        }
    }
}

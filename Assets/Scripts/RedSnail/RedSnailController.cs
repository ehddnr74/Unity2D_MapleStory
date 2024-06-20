using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using static RedSnailController;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Unity.Mathematics;
using TMPro;


public class RedSnailController : MonoBehaviour
{

    private string monsterName = "빨간 달팽이";
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

    public enum RedSnailState
    {
        Stand,
        Move,
        Hit,
        Die,
    }

    private DamageTextManager damageTextManager;


    public ItemPool itemPool; // 아이템 풀 참조
    public RedSnailState mRedSnailState;
    private MonsterSpawner monsterSpawner; // 몬스터 스포너 참조
    private ItemDataBase itemDataBase; // 아이템 데이터베이스 참조


    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

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

    private int experience = 8; // 획득 경험치량

    public bool onceAddExperience = true;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = GameObject.Find("RedSnailSpawner").GetComponent<MonsterSpawner>();
        itemDataBase = FindObjectOfType<ItemDataBase>();
        itemPool = GameObject.Find("ItemPoolManager").GetComponent<ItemPool>();

        mRedSnailState = RedSnailState.Stand;

        hpBarPool = GameObject.Find("HpBarCanvas").GetComponent<HpBarPool>();
        nameTagPool = GameObject.Find("NameTagCanvas").GetComponent<NameTagPool>();

        currentHealth = maxHealth;

        // HP 바 인스턴스 생성 및 캔버스에 추가
        hpBarInstance = hpBarPool.GetHPBar();
        hpSlider = hpBarInstance.GetComponent<Slider>();
        hpSlider.maxValue = maxHealth;
        hpSlider.value = currentHealth;

        // 네임태그 풀링 시스템 초기화
        nameTagInstance = nameTagPool.GetNameTag();
        nameTagText = nameTagInstance.GetComponentInChildren<TextMeshProUGUI>();
        nameTagText.text = monsterName; // 몬스터 이름 설정

        hitCheck = false;
        dieCheck = false;

        SetMoveDir();
        stateChangeTime = Time.time;
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

    private void Update()
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
        // 몬스터의 현재 위치가 x 좌표 46.2를 넘어가면 반대 방향으로 이동
        if (transform.position.x >= 46.2f || transform.position.x <= -76.17f)
        {
            moveDir *= -1f; // 이동 방향을 반대로 변경
            SetSpriteDir(moveDir);
            isChangingDirection = true; // 방향 변경 중 플래그를 설정
        }

        // 방향 변경 중이고, 몬스터의 x 좌표가 다시 일정 범위 안으로 들어올 때 플래그를 리셋
        if (isChangingDirection && transform.position.x <= 45.2f && transform.position.x >= -75.17f)
        {
            isChangingDirection = false;
        }


        // 일정 시간 대기 후 이동 상태로 전환
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
        // 이동 시간이 지나면 대기 상태로 전환
        if (Time.time - stateChangeTime >= moveTime)
        {
            mAnimator.SetBool("IsMoving", false);
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
            stateChangeTime = Time.time;
        }

        // 몬스터의 현재 위치가 x 좌표 46.2를 넘어가면 반대 방향으로 이동
        if (transform.position.x >= 46.2f || transform.position.x <= -76.17f)
        {
            moveDir *= -1f; // 이동 방향을 반대로 변경
            SetSpriteDir(moveDir);
            isChangingDirection = true; // 방향 변경 중 플래그를 설정
        }

        // 방향 변경 중이고, 몬스터의 x 좌표가 다시 일정 범위 안으로 들어올 때 플래그를 리셋
        if (isChangingDirection && transform.position.x <= 45.2f && transform.position.x >= -75.17f)
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
        mRigidBody.velocity = Vector2.zero; // 피격 중에는 움직이지 않음
        yield return new WaitForSeconds(0.15f); // 피격 애니메이션 재생 시간
        if (currentHealth > 0) // 피격 후 체력이 남아 있을 때만 Stand 상태로 전환
        {
            mAnimator.ResetTrigger("IsHitting"); // 트리거 재설정
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
        mRigidBody.velocity = Vector2.zero; // 속도 초기화
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
        onceAddExperience = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mRedSnailState = RedSnailState.Stand;
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
                            dropItemData.Initialize(item, itemPool);
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
        if (dir < 0) // 왼쪽 이동
        {
            spriteRenderer.flipX = false;
        }
        else if (dir > 0) // 오른쪽 이동
        {
            spriteRenderer.flipX = true;
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

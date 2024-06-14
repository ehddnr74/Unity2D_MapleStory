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
    private MonsterSpawner monsterSpawner; // 몬스터 스포너 참조


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

    private void OnEnable()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();
        monsterSpawner = FindObjectOfType<MonsterSpawner>();

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
        if (transform.position.x > 46.2f || transform.position.x < -76.17f)
        {
            moveDir *= -1f; // 이동 방향을 반대로 변경
            SetSpriteDir(moveDir);
            isChangingDirection = true; // 방향 변경 중 플래그를 설정
        }

        // 방향 변경 중이고, 몬스터의 x 좌표가 다시 일정 범위 안으로 들어올 때 플래그를 리셋
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
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        col.enabled = false; // 콜라이더 비활성화
        mRigidBody.velocity = Vector2.zero; // 속도 초기화
        yield return new WaitForSeconds(1.0f);
        col.enabled = true;
        mAnimator.SetBool("IsDying", false);
        mAnimator.SetBool("IsStanding", true);
        mRedSnailState = RedSnailState.Stand;
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
        monsterSpawner.DespawnMonster(gameObject); // 몬스터를 풀로 반환

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
}

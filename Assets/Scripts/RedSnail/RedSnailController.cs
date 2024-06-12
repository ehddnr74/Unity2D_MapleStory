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


    Collider2D col;
    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float moveTime = 3f; // 이동 시간
    public float idleTime = 1f; // 대기 시간
    public float moveSpeed = 3f;
    private float moveStartTime;
    private float moveDir;

    private bool isChangingDirection = false; // 방향 변경 중인지 여부를 나타내는 플래그


    private void Start()
    {
        col = GetComponent<Collider2D>();
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        damageTextManager = FindObjectOfType<DamageTextManager>();

        mRedSnailState = RedSnailState.Stand;
        SetMoveDir();
    }

    public void TakeDamage(int damage, bool isCritical, bool secondSuriken)
    {
        if (!secondSuriken)
        {
            Vector3 displayPosition = transform.position + new Vector3(-1.5f, 3.0f, 0); // y축으로 1.0f 만큼 더 높게 설정
            damageTextManager.ShowDamage(displayPosition, damage, isCritical); 
        }
        else
        {
            Vector3 secondPoisition = transform.position + new Vector3(-1.5f, 6.0f, 0); 
            damageTextManager.ShowDamage(secondPoisition, damage, isCritical);
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
        if (Time.time - moveStartTime >= idleTime)
        {
            mAnimator.SetBool("IsMoving", true);
            mRedSnailState = RedSnailState.Move;
            SetMoveDir();
        }
    }

    private void move()
    {
        // 이동 시간이 지나면 대기 상태로 전환
        if (Time.time - moveStartTime >= moveTime)
        {
            mAnimator.SetBool("IsStanding", true);
            mRedSnailState = RedSnailState.Stand;
            moveStartTime = Time.time;
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
    }
    private void hit()
    {

    }

    private void die()
    {
       
    }

    private void SetMoveDir()
    {
        moveDir = Random.Range(-1f, 1f);
        moveStartTime = Time.time;
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

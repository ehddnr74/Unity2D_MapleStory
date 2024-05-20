using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
    }

    public PlayerState mPlayerState;

    Rigidbody2D mRigidBody;
    Animator mAnimator;

    public float dir;
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    private bool isGround;


    // Start is called before the first frame update
    void Start()
    {
        mRigidBody = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mPlayerState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        SetSpriteDir(dir); // dir에 따른 스프라이트 반전 함수

        switch (mPlayerState)
        {
            case PlayerState.Idle:
                if (dir != 0)
                    mPlayerState = PlayerState.Walk;

                if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround)
                {
                    isGround = false;
                    mRigidBody.velocity = Vector2.up * jumpForce;
                    mAnimator.SetBool("IsJumping", true);
                    mPlayerState = PlayerState.Jump;     
                }

                mAnimator.SetBool("IsWalking", false);
                break;

            case PlayerState.Walk:
                if (dir == 0)
                    mPlayerState = PlayerState.Idle;

                if (Input.GetKeyDown(KeyCode.LeftAlt) && isGround)
                {
                    isGround = false;
                    mRigidBody.velocity = new Vector2(mRigidBody.velocity.x, jumpForce);
                    mAnimator.SetBool("IsJumping", true);
                    mPlayerState = PlayerState.Jump;
                }


                mAnimator.SetBool("IsWalking", true);
                break;

            case PlayerState.Jump:
                if (isGround)
                {
                    if (dir != 0)
                        mPlayerState = PlayerState.Walk;
                    else
                        mPlayerState = PlayerState.Idle;

                    mAnimator.SetBool("IsJumping", false);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isGround)
        {
            dir = Input.GetAxisRaw("Horizontal");
            mRigidBody.velocity = new Vector2(dir * moveSpeed, mRigidBody.velocity.y);
        }

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

}

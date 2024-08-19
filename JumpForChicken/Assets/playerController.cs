using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float height = -999;
    private float maxHeight = -999;
    private float width = -999;
    private float maxWidth = -999;

    private float gravity = 9.81f * 61 / 60;

    // 기본 값
    public float movePower = 4f;
    private Rigidbody2D rigid;
    private Animator animator;
    private Animator gaugeAnim;

    // 게이지 바 프리팹
    public GameObject gaugeBar;

    private GameObject gaugeObject;

    // 방향: 오른쪽 = 1, 왼쪽 = -1
    private int lookAt = 1;

    // 점프 키를 누른 프레임
    private int jumpHoldTime = 0;

    // Update에서 입력받는 inputAxis Horizontal, FixedUpdate에서 사용
    private float inputAxis;

    // 점프 여부
    private bool isJump = false;

    // 점프 속도 변수
    private float moveX = 0;
    private float moveY = 0;

    // 시작될 때 실행되는 코드
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isFirstJump", true);

        // 고정 프레임 60
        Application.targetFrameRate = 60;
    }

    // 프레임 당 초기화: 사용자 입력 감지
    void Update()
    {
        // 땅에 있을 때
        if (!animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
        {
            if (Input.GetButtonDown("Jump") && rigid.velocity.y == 0)
            {
                gaugeObject = Instantiate(gaugeBar, transform.position + new Vector3(lookAt * 0.7f, 0, 0), Quaternion.identity);
                gaugeObject.transform.SetParent(transform);
                gaugeAnim = gaugeObject.GetComponent<Animator>();
            }

            if (Input.GetButton("Jump") && rigid.velocity.y == 0)
            {
                // doJumpReady 트리거를 한 번만 작동시키기 위한 조건문
                if (animator.GetBool("isFirstJump"))
                {
                    animator.SetTrigger("doJumpReady");
                    animator.SetBool("isFirstJump", false);
                }

                // 점프 준비 시간 증가
                jumpHoldTime = Mathf.Min(jumpHoldTime + 1, 90);
            }

            if (Input.GetButtonUp("Jump") && rigid.velocity.y == 0)
            {
                Debug.Log(transform.position.y);
                height = transform.position.y;
                maxHeight = transform.position.y;
                width = transform.position.x;

                isJump = true;
                animator.SetBool("isJumping", true);
                animator.SetTrigger("doJumping");
                gaugeAnim.speed = 0;
            }

            // 땅에 있는데 y속도가 음수인 버그 우회 (물리랑 화면 간의 프레임 차이)
            if (rigid.velocity.y < 0)
            {
                animator.SetBool("isFirstJump", true);
                jumpHoldTime = 0;
            }
        }

        // Update에서 변수 저장
        inputAxis = Input.GetAxisRaw("Horizontal");
    }


    private void FixedUpdate()
    {
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수
        Animations(); // 애니메이션 담당 함수

        if (maxHeight < transform.position.y)
        {
            maxHeight = transform.position.y;
            Debug.Log(maxHeight - height);
        }
    }

    // 낙하 감지
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.7f)
        {
            Landing();
        }
    }

    private void Move()
    {
        // 플레이어의 속도를 직접적으로 설정 불가능하므로, 새로운 변수에 저장 후 수정
        Vector2 currentVelocity = rigid.velocity;

        // 점프 장전이 안 되어있고, 점프 중이지 않고, 낙하 중이지 않을 때
        if (jumpHoldTime == 0 && !animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
        {
            if (inputAxis < 0)
            {
                lookAt = -1;
                currentVelocity.x = movePower * lookAt;
            }
            else if (inputAxis > 0)
            {
                lookAt = 1;
                currentVelocity.x = movePower * lookAt;
            }
            else
            {
                currentVelocity.x = 0;
            }
            rigid.velocity = currentVelocity;
        }

        // 플레이어 좌우 반전
        transform.localScale = new Vector3(lookAt * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // 점프 장전 중에는 정지
        if (jumpHoldTime > 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
    }

    private void Jump()
    {
        if (!isJump)
        {
            return; // Jump 함수 종료
        }

        // 점프 궤도 계산 (한 번만 실행)
        switch (jumpHoldTime)
        {
            case int n when 0 <= n && n < 18:
                moveX = 2.5f;
                moveY = 0.5f;
                break;

            case int n when 18 <= n && n < 36:
                moveX = 4f;
                moveY = 1f;
                break;

            case int n when 36 <= n && n < 54:
                moveX = 4f;
                moveY = 3.05f;
                break;

            case int n when 54 <= n && n < 72:
                moveX = 6f;
                moveY = 5.15f;
                break;

            case int n when 72 <= n && n < 90:
                moveX = 4f;
                moveY = 7.20f;
                break;

            case int n when n <= 90:
                moveX = 2f;
                moveY = 8.25f;
                break;
        }

        moveX -= 0.01f;
        moveY += 0.0285f;

        // 점프 속도 적용
        rigid.velocity += new Vector2(moveX * Mathf.Sqrt(gravity / (8 * moveY)) * lookAt, Mathf.Sqrt(2 * gravity * moveY));

        // 점프 후 초기화
        jumpHoldTime = 0;
        isJump = false;
        animator.SetBool("onGround", false);
    }

    private void Animations()
    {
        // 이동 애니메이션 설정
        animator.SetBool("isMove", inputAxis != 0 && jumpHoldTime == 0);

        // 낙하 애니메이션 설정
        if (rigid.velocity.y < 0)
        {
            if (!animator.GetBool("isFalling"))
            {
                animator.SetTrigger("doFalling");
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
                animator.SetBool("onGround", false);
            }
        }

        // 낙하 모션 중 땅에서 정지하는 버그 수정
        if ((animator.GetBool("isFalling") && animator.GetBool("isFirstJump")))
        {
            Landing();
        }
    }

    private void Landing()
    {
        animator.SetBool("isFalling", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFirstJump", true);
        rigid.velocity = Vector2.zero;
        animator.SetBool("onGround", true);
        Destroy(gaugeObject);

        maxWidth = transform.position.x;
        Debug.Log(Mathf.Abs(width - maxWidth));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{
    // 기본 값
    public float movePower = 4f;
    Rigidbody2D rigid;
    Animator animator_;

    // 오른쪽 : 1, 왼쪽 : -1
    public int lookat = 1;

    // 점프 키를 누른 프레임
    int jumpHoldTime = 0;

    // Update에서 입력받는 inputAxis Horizontal, FixedUpdate에서 사용
    float inputAxis;

    // true가 되면 점프 실행 (한 프레임 후 false)
    bool isJump = false;

    // 미세하게 움직이면 x 속도가 0이 되어 떨어지는 버그 수정
    bool stoped;

    float move_x = 0;
    float move_y = 0;

    // 시작될 떄 실행되는 코드
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
        animator_.SetBool("isFirstJump", true);
        stoped = false;

        Application.targetFrameRate = 60;
    }

    // 프레임 당 초기화 : 사용자 인풋 감지를 위함
    void Update()
    {
        if (!animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){ // 땅에 있을 때
            if (Input.GetButton("Jump") && rigid.velocity.y == 0){
                // doJumpReady 트리거를 한 번만 작동시키기 위한 조건문
                if (animator_.GetBool("isFirstJump")){
                    animator_.SetTrigger("doJumpReady");
                    animator_.SetBool("isFirstJump", false);
                }
                jumpHoldTime += jumpHoldTime < 90 ? 1 : 0;
                Debug.Log(jumpHoldTime);
                Debug.Log(1/Time.deltaTime);
                // Debug.Log(delta * Time.deltaTime);
            }
            if (Input.GetButtonUp("Jump") && rigid.velocity.y == 0){
                isJump = true;
                animator_.SetBool("isJumping", true);
                animator_.SetTrigger("doJumping");
                stoped = false;
            }
            if (rigid.velocity.y < 0){
                animator_.SetBool("isFirstJump", true);
                jumpHoldTime = 0;
            }
        }

        // Update에서 변수 저장
        inputAxis = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수
        Animations(); // 애니메이션 담당 함수
    }

    /** 낙하감지 */
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contacts[0].normal.y > 0.7f){
            animator_.SetBool("isFalling", false);
            animator_.SetBool("isJumping", false);
            animator_.SetBool("isFirstJump", true);
            rigid.velocity = Vector2.zero;
            animator_.SetBool("onGround", true);
        }
    }

    void Move()
    {
        // 플레이어의 속도를 직접적으로 설정 불가능 하므로, 새로운 변수에 저장 후 변수를 수정하여 다시 대입
        Vector2 currentVelocity = rigid.velocity;
        // 만약 점프 장전이 안 되어있고, 점프 중이지 않고, 낙하 중이지 않을 때
        if (jumpHoldTime == 0 && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){
            if (inputAxis < 0){
                lookat = -1;
                currentVelocity.x = movePower * lookat;
                stoped = false;
            } else if (inputAxis > 0){
                lookat = 1;
                currentVelocity.x = movePower * lookat;
                stoped = false;
            } else {
                currentVelocity.x = 0;
                stoped = true;
            }
            rigid.velocity = currentVelocity;
        }

        // inputAxis가 0인 상황에서 떨어지기 시작했을 떄. 이론 상은 문제 없지만, 프레임 차이로 발생하는 버그 수정
        if (animator_.GetBool("isFalling") && rigid.velocity.x == 0 && stoped){
            currentVelocity.x = movePower * lookat;
            rigid.velocity = currentVelocity;
            stoped = false;
        }
        
        // 플레이어 좌우 반전
        transform.localScale = new Vector3(lookat*Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        // 점프 장전 중에는 정지
        if (jumpHoldTime > 0) rigid.velocity = Vector2.zero;
    }

    void Jump()
    {
        if (!isJump){
            return; // Jump 함수 종료
        }

        // 이 아래로는 한 번만 실행

        switch (jumpHoldTime)
        {
            case int n when 0 <= n && n < 18:
                    move_x = 2.5f;
                    move_y = 0.5f;
                break;

            case int n when 18 <= n && n < 36:
                    move_x = 4f;
                    move_y = 1f;
                break;

            case int n when 36 <= n && n < 54:
                    move_x = 4f;
                    move_y = 3f;
                break;

            case int n when 54 <= n && n < 72:
                    move_x = 6f;
                    move_y = 5f;
                break;

            case int n when 72 <= n && n < 90:
                    move_x = 4f;
                    move_y = 7f;
                break;

            case int n when n <= 90:
                    move_x = 2f;
                    move_y = 8f;
                break;
        }
        
        rigid.velocity += new Vector2(move_x*Mathf.Sqrt(9.81f/(8*move_y))*lookat,Mathf.Sqrt(2*9.81f*move_y));

        jumpHoldTime = 0;
        isJump = false;
        animator_.SetBool("onGround", false);

    }

    void Animations()
    {
        if (inputAxis == 0 || jumpHoldTime > 0){
            animator_.SetBool("isMove", false);
        }
        else {
            animator_.SetBool("isMove", true);
        }

        if (rigid.velocity.y < 0){
            if (!animator_.GetBool("isFalling")){
                animator_.SetTrigger("doFalling");
                animator_.SetBool("isJumping", false);
                animator_.SetBool("isFalling", true);
                animator_.SetBool("onGround", false);
            }
        }
    }
}

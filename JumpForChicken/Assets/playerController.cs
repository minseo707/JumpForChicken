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
    public float movePower = 5f;
    public float jumpPower = 0.015f;
    public float minJumpPower = 7.5f;
    public float maxJumpPower = 11f;
    public float widthHeight = 32f;
    Rigidbody2D rigid;
    Animator animator_;

    // 오른쪽 : 1, 왼쪽 : -1
    public int lookat = 1;

    // Start에서 jumpGauge = minJumpPower
    float jumpGauge = 0f;

    // Update에서 입력받는 inputAxis Horizontal, FixedUpdate에서 사용
    float inputAxis;

    float maxHeight = -1000f;
    
    // true가 되면 점프 실행 (한 프레임 후 false)
    bool isJump = false;

    // 미세하게 움직이면 x 속도가 0이 되어 떨어지는 버그 수정
    bool stoped;

    float whControl;
    float delta;

    // 시작될 떄 실행되는 코드
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
        jumpGauge = minJumpPower;
        animator_.SetBool("isFirstJump", true);
        stoped = false;
        delta = maxJumpPower - minJumpPower;
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
                jumpGauge += jumpGauge < maxJumpPower ? delta * Time.deltaTime / 1.5f: 0;
                Debug.Log(jumpGauge);
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
                jumpGauge = minJumpPower;
                // Debug.Log("Bugged!s");
            }
        }

        // Update에서 변수 저장
        inputAxis = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수
        Animations(); // 애니메이션 담당 함수

        if (transform.position.y + 27.35211> maxHeight){
            // if (transform.position.y + 27.35211f > 0.2) Debug.Log(transform.position.y + 27.35211f);
            maxHeight = transform.position.y + 27.35211f;
        }
    }

    /** 낙하감지 */
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contacts[0].normal.y > 0.7f){
            animator_.SetBool("isFalling", false);
            animator_.SetBool("isJumping", false);
            animator_.SetBool("isFirstJump", true);
            rigid.velocity = Vector2.zero;
            maxHeight = -27.35211f;
        }
    }

    void Move()
    {
        // 플레이어의 속도를 직접적으로 설정 불가능 하므로, 새로운 변수에 저장 후 변수를 수정하여 다시 대입
        Vector2 currentVelocity = rigid.velocity;
        // 만약 점프 장전이 안 되어있고, 점프 중이지 않고, 낙하 중이지 않을 때
        if (jumpGauge == minJumpPower && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){
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
        if (jumpGauge != minJumpPower) rigid.velocity = Vector2.zero;
    }

    void Jump()
    {
        if (!isJump){
            return; // Jump 함수 종료
        }

        // 이 아래로는 한 번만 실행
        
        whControl = 0.75f*(Mathf.Log((jumpGauge - minJumpPower) / maxJumpPower + 0.01f)-Mathf.Log(0.01f))/(Mathf.Log(1.01f)-Mathf.Log(0.01f))+0.25f;
        rigid.velocity += new Vector2(whControl*widthHeight*Mathf.Pow(9.81f, 2)/Mathf.Pow(jumpGauge, 3)*lookat, jumpGauge);


        jumpGauge = minJumpPower;
        isJump = false;

    }

    void Animations()
    {
        if (inputAxis == 0 || jumpGauge != minJumpPower){
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
            }
        }
    }
}

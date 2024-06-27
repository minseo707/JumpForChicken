using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{
    public float movePower = 1f;
    public float jumpPower = 0.05f;
    public float minJumpPower = 4f;
    public float maxJumpPower = 8f;
    public float widthHeight = 8f;
    Rigidbody2D rigid;
    Animator animator_;

    // 오른쪽 : 1, 왼쪽 : -1
    public int lookat = 1;

    Vector3 movement;
    // 점프 힘 조절
    float jumpGauge = 0f;
    bool isJump = false;
    float inputAxis;

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
        jumpGauge = minJumpPower;
        animator_.SetBool("isFirstJump", true);
    }

    // 프레임 당 초기화 : 사용자 인풋 감지를 위함
    void Update()
    {
        if (!animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){
            if (Input.GetButton("Jump")){
                if (animator_.GetBool("isFirstJump")){
                    animator_.SetTrigger("doJumpReady");
                    animator_.SetBool("isFirstJump", false);
                }
                jumpGauge += jumpGauge < maxJumpPower ? jumpPower : 0;
            }
            if (Input.GetButtonUp("Jump")){
                isJump = true;
                animator_.SetBool("isJumping", true);
                animator_.SetTrigger("doJumping");
            }
        }

        inputAxis = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        Move();
        Jump();
        Animations(); // 애니메이션 함수
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.contacts[0].normal.y > 0.7f){
            animator_.SetBool("isFalling", false);
            animator_.SetBool("isJumping", false);
            animator_.SetBool("isFirstJump", true);
            rigid.velocity = Vector2.zero;
        }
    }

    void Move()
    {
        Vector2 currentVelocity = rigid.velocity;
        if (jumpGauge == minJumpPower && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){
            if (inputAxis < 0){
                lookat = -1;
                currentVelocity.x = movePower * lookat;
            } else if (inputAxis > 0){
                lookat = 1;
                currentVelocity.x = movePower * lookat;
            } else {
                currentVelocity.x = 0;
            }
            rigid.velocity = currentVelocity;
        }
        
        transform.localScale = new Vector3(lookat*Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (jumpGauge != minJumpPower) rigid.velocity = Vector2.zero;
    }

    void Jump()
    {
        if (!isJump){
            return; // Jump 함수 종료
        }
        rigid.velocity += new Vector2(widthHeight*Mathf.Pow(9.81f, 2)/Mathf.Pow(jumpGauge, 3)*lookat, jumpGauge);


        jumpGauge = minJumpPower;
        isJump = false;
    }

    void Animations()
    {
        if (inputAxis == 0 || jumpGauge != minJumpPower){
            animator_.SetBool("isMove", false);
        }
        else if (inputAxis != 0 && jumpGauge == minJumpPower) {
            animator_.SetBool("isMove", true);
        }

        if(rigid.velocity.y < 0){
            if (!animator_.GetBool("isFalling")){
                animator_.SetTrigger("doFalling");
                animator_.SetBool("isJumping", false);
                animator_.SetBool("isFalling", true);
            }
        }
    }
}

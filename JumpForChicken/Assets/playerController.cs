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

    float first = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
        jumpGauge = minJumpPower;
    }

    // 프레임 당 초기화 : 사용자 인풋 감지를 위함
    void Update()
    {
        if (Input.GetButton("Jump") && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")) {
            jumpGauge += jumpGauge < maxJumpPower ? jumpPower : 0;
        }
        if (Input.GetButtonUp("Jump") && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")){
            isJump = true;
            animator_.SetBool("isJumping", true);
            animator_.SetTrigger("doJumping");
        }

        inputAxis = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate() {
        Move();
        Jump();
        Animations(); // 애니메이션 함수
    }

    void Move()
    {
        Vector2 currentVelocity = rigid.velocity;
        if (jumpGauge == minJumpPower && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling") && animator_.GetBool("onGround")){
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
        
        transform.localScale = new Vector3(lookat, 1, 1);
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
        if (first < rigid.velocity.y && rigid.velocity.y == 0f){
            Debug.Log("가속됨");
            Debug.Log(rigid.velocity.y);
        }
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

        first = rigid.velocity.y;
    }
}

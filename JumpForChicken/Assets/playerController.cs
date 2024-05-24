using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float movePower = 1f;
    public float jumpPower = 0.05f;
    public float minJumpPower = 2f;
    public float maxJumpPower = 5f;
    Rigidbody2D rigid;
    Animator animator_;

    // 오른쪽 : 1, 왼쪽 : -1
    public int lookat = 1;

    Vector3 movement;
    // 점프 힘 조절
    float jumpGauge = 0f;
    bool isJump = false;

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
    }

    // 프레임 당 초기화 : 사용자 인풋 감지를 위함
    void Update()
    {
        if (Input.GetButton("Jump") && !animator_.GetBool("isJumping") && !animator_.GetBool("isFalling")) {
            jumpGauge += jumpGauge < maxJumpPower ? jumpPower : 0;
        }
        if (Input.GetButtonUp("Jump") && !animator_.GetBool("isJumping") && jumpGauge >= minJumpPower && !animator_.GetBool("isFalling")){
            isJump = true;
            animator_.SetBool("isJumping", true);
            animator_.SetTrigger("doJumping");
        } else if (Input.GetButtonUp("Jump") && !animator_.GetBool("isJumping") && jumpGauge < minJumpPower && !animator_.GetBool("isFalling")){
            jumpGauge = 0f;
        }
    }

    private void FixedUpdate() {
        Move();
        Jump();
        Animations(); // 애니메이션 함수
    }

    void Move()
    {
        // 초기화
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0){
            moveVelocity = Vector3.left;
            lookat = -1;
        } else if (Input.GetAxisRaw("Horizontal") > 0){
            moveVelocity = Vector3.right;
            lookat = 1;
        }
        transform.localScale = new Vector3(lookat, 1, 1);
        transform.position += jumpGauge == 0 ? moveVelocity * movePower * Time.deltaTime : Vector3.zero;
    }

    void Jump()
    {
        if (!isJump){
            return; // Jump 함수 종료
        }
        rigid.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpGauge);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        jumpGauge = 0f;
        isJump = false;
    }

    void Animations()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 || jumpGauge != 0){
            animator_.SetBool("isMove", false);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0 && jumpGauge == 0) {
            animator_.SetBool("isMove", true);
        }

        if(rigid.velocity.y < 0){
            if (!animator_.GetBool("isFalling")){
                animator_.SetTrigger("doFalling");
            }
            animator_.SetBool("isJumping", false);
            animator_.SetBool("isFalling", true);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer == 6 && rigid.velocity.y == 0) {
            animator_.SetBool("isFalling", false);
            animator_.SetBool("isJumping", false);
        }
    }
}

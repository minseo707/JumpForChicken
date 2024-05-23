using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public float movePower = 1f;
    public float jumpPower = 1f;
    Rigidbody2D rigid;
    Animator animator_;

    // 오른쪽 : 1, 왼쪽 : -1
    public int lookat = 1;

    Vector3 movement;
    bool isJump = false;
    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator_ = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && !animator_.GetBool("isJumping")) {
            isJump = true;
            animator_.SetBool("isJumping", true);
            animator_.SetTrigger("doJumping");
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
        transform.position += moveVelocity * movePower * Time.deltaTime;
    }

    void Jump()
    {
        if (!isJump){
            return; // Jump 함수 종료
        }
        rigid.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJump = false;
    }

    void Animations()
    {
        if (Input.GetAxisRaw("Horizontal") == 0){
            animator_.SetBool("isMove", false);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0) {
            animator_.SetBool("isMove", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 6 && rigid.velocity.y < 0) {
            animator_.SetBool("isJumping", false);
        }
    }
}

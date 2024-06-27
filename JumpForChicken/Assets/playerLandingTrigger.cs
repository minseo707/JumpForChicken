using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerLandingTrigger : MonoBehaviour
{
    Rigidbody2D rigid;

    Animator animator_;
    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponentInParent<Rigidbody2D>();
        animator_ = GetComponentInParent<Animator>();
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer == 6) {
            if (rigid.velocity.y == 0 && animator_.GetBool("isFalling")){
                animator_.SetBool("isFalling", false);
                animator_.SetBool("isJumping", false);
                animator_.SetBool("isFirstJump", true);
                rigid.velocity = Vector2.zero;
            }
            animator_.SetBool("onGround", true);
        }
    }
}

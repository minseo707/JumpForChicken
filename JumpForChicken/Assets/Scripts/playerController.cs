
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int height = 0;
    private float maxHeight = -999;
    private float width = -999;
    private float maxWidth = -999;

    private float gravity = 9.81f * 61 / 60;

    public float gameSpeed = 1.6f;

    private float jumpTime = 0f;

    public bool inputLeft = false;
    public bool inputRight = false;
    public bool inputJump = false;
    public bool firstJumpUp = false;

    // 기본 값
    public float movePower = 4f;
    private Rigidbody2D rigid;
    private Animator animator;

    // 게이지 바 프리팹
    public GameObject gaugeBar;

    // 사라지는 파티클 프리팹
    public GameObject daParticle;

    // Trail 파티클
    public GameObject trailParticle;

    // 착지 먼지 프리팹
    public GameObject landingParticle;

    private SpriteRenderer spriteRendererGauge;
    private GameObject gaugeObject;

    // 방향: 오른쪽 = 1, 왼쪽 = -1
    // animator.GetInteger("lookAt")

    // 점프 키를 누른 프레임
    private int jumpHoldTime = 0;

    // Update에서 입력받는 inputAxis Horizontal, FixedUpdate에서 사용
    private float inputAxis;

    // 점프 여부
    private bool isJump = false;

    // 미세하게 움직이면 x 속도가 0이되어 떨어지는 버그 수정
    private bool stopped;

    // 점프 속도 변수
    private float moveX = 0;
    private float moveY = 0;

    private float landingFreezeDuration = 0.1f; // 착지 후 일시 정지 시간
    private float landingFreezeTimer = 0f; // 착지 후 타이머

    private bool isDead = false;

    // 점프 최대 높이 오프셋 (보정치)
    private float offest = 2.355f;

    private GameObject soundPlayManager;
    private bool isWalkingSoundPlaying = false; // 현재 걷는 소리가 재생 중인지 추적

    GameObject gm;
    void Awake(){
        gm = GameObject.Find("GameManager");
    }

    // 시작될 때 실행되는 코드
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("isFirstJump", true);
        stopped = false;
        isDead = false;

        soundPlayManager = GameObject.Find("Sound Player");

        isWalkingSoundPlaying = false;

        spriteRendererGauge = gaugeBar.GetComponent<SpriteRenderer>();

        // 고정 프레임 60
        Application.targetFrameRate = 60;
        // Vsync 비활성화
        QualitySettings.vSyncCount = 0;

        gravity *= gameSpeed;

        rigid.gravityScale = gameSpeed;

        inputLeft = false;
        inputRight = false;
        inputJump = false;
        firstJumpUp = false;

        UIButtonManager uib = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIButtonManager>();
        uib.Init();
}

    // 프레임 당 초기화: 사용자 입력 감지
    void Update()
    {
        // 타이머 업데이트
        if (landingFreezeTimer > 0f)
        {
            landingFreezeTimer -= Time.deltaTime;
            if (landingFreezeTimer <= 0f)
            {
                landingFreezeTimer = 0f;
                // 타이머가 끝난 후 다시 움직일 수 있도록 설정
                rigid.velocity = new Vector2(inputAxis * movePower, rigid.velocity.y);
            }
        }
        else
        {
            // 땅에 있을 때
            if (!animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
            {
                if ((inputJump || Input.GetButton("Jump")) && rigid.velocity.y == 0)
                {
                    // doJumpReady 트리거를 한 번만 작동시키기 위한 조건문
                    // isFirstJump : 다음 점프는 첫 번째 점프이다.
                    if (animator.GetBool("isFirstJump"))
                    {
                        animator.ResetTrigger("doJumpCancel");
                        animator.SetTrigger("doJumpReady");
                        animator.SetBool("isFirstJump", false);

                        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jumpReady", 2f, 1.5f);

                        spriteRendererGauge.enabled = true;
                        MoveAnimRandom();
                    }

                    // 점프 준비 시간 증가
                    jumpHoldTime = Mathf.Min(jumpHoldTime + 1, 181);

                    // 점프 캔슬 (1 프레임 오차 허용)
                    if (jumpHoldTime == 180){
                        Destroy(gaugeObject);
                        spriteRendererGauge.enabled = false;
                        animator.SetTrigger("doJumpCancel");
                        GameObject dapInstance = Instantiate(daParticle);
                        dapInstance.transform.position = new Vector3(transform.position.x + animator.GetInteger("lookAt") * 0.7f, transform.position.y, transform.position.z);
                    }

                    // 게이지바에 점프 준비 시간 동기화
                    gaugeBar.GetComponent<GaugeBarManager>().jumpGauge = jumpHoldTime;
                }

                if ((Input.GetButtonUp("Jump") || firstJumpUp) && rigid.velocity.y == 0)
                {
                    if (jumpHoldTime >= 180){
                        jumpHoldTime = 0;
                        Landing(false);
                    }
                    else {
                        width = transform.position.x;

                        isJump = true;
                        animator.SetBool("isJumping", true);
                        stopped = false;

                        jumpTime = 0f;
                    }
                    
                }

                firstJumpUp = false;

                // 땅에 있는데 y속도가 음수인 버그 우회 (물리랑 화면 간의 프레임 차이)
                if (rigid.velocity.y < 0)
                {
                    animator.SetBool("isFirstJump", true);
                    jumpHoldTime = 0;
                }
            }

            // Update에서 변수 저장
            inputAxis = Input.GetAxisRaw("Horizontal");
            
            inputAxis = inputLeft ? -1 : inputAxis;
            inputAxis = inputRight ? 1 : inputAxis;
            inputAxis = inputLeft && inputRight ? 0 : inputAxis;
        }

        jumpTime += Time.deltaTime;

        // Trail Particle Management
        TrailManager();
    }


    private void FixedUpdate()
    {
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수
        Animations(); // 애니메이션 담당 함수
        ScoreByHeight(); // 높이에 따른 점수 담당 함수

        if (isDead) rigid.velocity = Vector2.zero;

        if (maxHeight - offest < transform.position.y)
        {
            maxHeight = transform.position.y + offest;
        }
    }

    // 낙하 감지
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.7f && animator.GetBool("isFalling"))
        {
            Landing();
        }
    }
    
    // 벽 충돌 시 실행
    private void OnCollisionEnter2D(Collision2D collision) {
        // 플레이어가 왼쪽에서 오른쪽으로 충돌
        if (collision.contacts[0].normal.x < -0.7f && !collision.gameObject.CompareTag("PassBlock"))
        {
            if (animator.GetBool("isJumping")){
                rigid.velocity = new Vector2(-1.5f, rigid.velocity.y/3);
                animator.SetTrigger("doCrashing");
                animator.SetBool("isCrashing", true);
                animator.SetInteger("lookAt", 1);
                soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
            }
            else if (animator.GetBool("isFalling")){
                rigid.velocity = new Vector2(-0.75f, rigid.velocity.y);
                animator.SetTrigger("doCrashing");
                animator.SetBool("isCrashing", true);
                animator.SetInteger("lookAt", 1);
                soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
            }
        }
        // 플레이어가 오른쪽에서 왼쪽으로 충돌
        else if (collision.contacts[0].normal.x > 0.7f && !collision.gameObject.CompareTag("PassBlock"))
        {
            if (animator.GetBool("isJumping")){
                rigid.velocity = new Vector2(1.5f, rigid.velocity.y/3);
                animator.SetTrigger("doCrashing");
                animator.SetBool("isCrashing", true);
                animator.SetInteger("lookAt", -1);
                soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
            }
            else if (animator.GetBool("isFalling")){
                rigid.velocity = new Vector2(0.75f, rigid.velocity.y);
                animator.SetTrigger("doCrashing");
                animator.SetBool("isCrashing", true);
                animator.SetInteger("lookAt", -1);
                soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
            }
        }

    }

    private void Move()
    {
        // 플레이어의 속도를 직접적으로 설정 불가능하므로, 새로운 변수에 저장 후 수정
        Vector2 currentVelocity = rigid.velocity;

        if (landingFreezeTimer > 0f)
        {
            currentVelocity.x = 0; // 착지 후 멈추는 상태
        }

        // 점프 장전이 안 되어있고, 점프 중이지 않고, 낙하 중이지 않을 때
        else if (jumpHoldTime == 0 && !animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
        {
            if (inputAxis < 0)
            {
                animator.SetInteger("lookAt", -1);
                stopped = false;
                currentVelocity.x = movePower * animator.GetInteger("lookAt");
                MoveAnimRandom();
            }
            else if (inputAxis > 0)
            {
                animator.SetInteger("lookAt", 1);
                stopped = false;
                currentVelocity.x = movePower * animator.GetInteger("lookAt");
                MoveAnimRandom();
            }
            else
            {
                currentVelocity.x = 0;
                stopped = true;
            }
            rigid.velocity = currentVelocity;
        }

        // inputAxis가 0인 상황에서 떨어지기 시작했을 때 이론 상은 문제 없지만, 프레임 차이로 발생하는 버그 수정
        if (animator.GetBool("isFalling") && rigid.velocity.x == 0 && stopped)
        {
            currentVelocity.x = movePower * animator.GetInteger("lookAt");
            rigid.velocity = currentVelocity;
            stopped = false;
        }

        // 플레이어 좌우 반전
        // transform.localScale = new Vector3(lookAt * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // 점프 장전 중에는 정지
        if (jumpHoldTime > 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            MoveAnimRandom();
        }

        // 걷는 효과음
        if (animator.GetBool("isMove") && !animator.GetBool("isJumping") && !animator.GetBool("isFalling")){
            if (!isWalkingSoundPlaying)
            {
                soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("walking");
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            if (isWalkingSoundPlaying)
            {
                soundPlayManager.GetComponent<SoundPlayManager>().StopSound("walking");
                isWalkingSoundPlaying = false;
            }
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
                moveX = 1.53f;
                moveY = 1.55f;
                break;

            case int n when 18 <= n && n < 36:
                moveX = 2.55f;
                moveY = 3.05f;
                break;

            case int n when 36 <= n && n < 54:
                moveX = 4.05f;
                moveY = 3.07f;
                break;

            case int n when 54 <= n && n < 72:
                moveX = 6.05f;
                moveY = 5.15f;
                break;

            case int n when 72 <= n && n < 90:
                moveX = 4f;
                moveY = 7.20f;
                break;

            case int n when n >= 90:
                moveX = 2f;
                moveY = 8.25f;
                break;
        }

        // 보정치, 실제 값 오차 복구
        moveX -= 0.01f;
        moveY += 0.0285f;

        // 점프 속도 적용
        rigid.velocity += new Vector2(moveX * Mathf.Sqrt(gravity / (8 * moveY)) * animator.GetInteger("lookAt"), Mathf.Sqrt(2 * gravity * moveY));

        // Play Jump Sound
        if (jumpHoldTime < 54) soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jump1");
        else soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jump2");;

        jumpHoldTime = Math.Min(jumpHoldTime, 119);
        gaugeBar.GetComponent<GaugeBarManager>().jumpGauge = jumpHoldTime;

        // 점프 후 초기화
        isJump = false;
        jumpHoldTime = 0;
        animator.SetBool("onGround", false);
        MoveAnimRandom();
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
                animator.SetBool("isFirstJump", false);
                MoveAnimRandom();
            }
        }

        // 낙하 모션 중 땅에서 정지하는 버그 수정
        if (animator.GetBool("isFalling") && animator.GetBool("isFirstJump"))
        {
            Landing();
        }
    }

    private void Landing(bool playParticle = true)
    {
        // 떨어질 때만 효과음 재생 (점프 취소 시 재생되는 오류 해결)
        if (animator.GetBool("isFalling")){
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("landing");
        }

        animator.SetBool("isFalling", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFirstJump", true);
        rigid.velocity = Vector2.zero;
        animator.SetBool("onGround", true);
        animator.SetBool("isCrashing", false);
        Destroy(gaugeObject);

        // 동기화 원리 : 점프 키를 떼는 순간, jumpHoldTime은 0이 되지만, gaugeBar의 jumpGauge는 점프 키를 떼기 직전의 jumpHoldTime을 가지고 있으므로
        // jumpHoldTime이 0이 되어도 점프 중일 때 적절한 게이지바를 출력 
        gaugeBar.GetComponent<GaugeBarManager>().jumpGauge = jumpHoldTime;

        spriteRendererGauge.enabled = false;

        maxWidth = transform.position.x;

        // 착지 후 일시적으로 멈추는 타이머 시작
        landingFreezeTimer = landingFreezeDuration;

        if (playParticle){
            GameObject landingPtcInstance = Instantiate(landingParticle);
            landingPtcInstance.transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Dead" && !isDead){
            Die();
        }
    }

    private void ScoreByHeight(){
        if (maxHeight - height >= 1f){
            gm.GetComponent<ScoreManager>().AddScore(1);
            height += 1;
        }
    }

    private void MoveAnimRandom(){
        if (UnityEngine.Random.Range(0, 2) == 0){
            animator.SetBool("isIDLE1", true);
        } else {
            animator.SetBool("isIDLE1", false);
        }
    }

    private void TrailManager(){
        if (animator.GetBool("isJumping") || animator.GetBool("isFalling")){
            trailParticle.SetActive(true);
        } else {
            trailParticle.SetActive(false);
        }
    }

    private void Die(){
        isDead = true;
        FirstGameUpdater.Instance.OnPlayerDead();
        rigid.velocity = Vector2.zero;

        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("failed");

        gm.GetComponent<GameManager>().EndingUIActive();
    }
}
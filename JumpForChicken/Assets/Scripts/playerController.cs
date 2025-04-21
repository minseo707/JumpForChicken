
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 플레이어 움직임 및 기타 효과 재생
/// </summary>
public class PlayerController : MonoBehaviour
{
    private int height = 0;
    private float maxHeight = -999;

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

    private PlayerAnimationManager pam;

    // 게이지 바 프리팹
    public GameObject gaugeBar;

    private GaugeBarManager gbm;
    private TutorialGaugeBarManager tgbm;
    private bool isTutorial = false;

    // 사라지는 파티클 프리팹
    public GameObject daParticle;

    // Trail 파티클
    public GameObject trailParticle;

    // 착지 먼지 프리팹
    public GameObject landingParticle;

    private SpriteRenderer spriteRendererGauge;


    private UIButtonManager uib;

    private CameraController mainCameraController;

    private Collider2D col;

    // 방향: 오른쪽 = 1, 왼쪽 = -1
    // animator.GetInteger("lookAt")

    // 점프 키를 누른 프레임
    private int jumpHoldTime = 0;

    // Update에서 입력받는 inputAxis Horizontal, FixedUpdate에서 사용
    private float inputAxis;

    private float lastInputAxis;

    private bool jumpLock = false;

    // 점프 여부
    private bool isJump = false;

    // 미세하게 움직이면 x 속도가 0이되어 떨어지는 버그 수정
    private bool stopped;

    // 점프 속도 변수
    private float moveX = 0;
    private float moveY = 0;

    private readonly float landingFreezeDuration = 0.1f; // 착지 후 일시 정지 시간
    private float landingFreezeTimer = 0f; // 착지 후 타이머

    private bool isDead = false;

    // 점프 최대 높이 오프셋 (보정치)
    private readonly float offest = 2.355f;

    private GameObject soundPlayManager;
    private bool isWalkingSoundPlaying = false; // 현재 걷는 소리가 재생 중인지 추적

    // 계속 헷갈려서 분명히 기재
    // [i][j] 에서 i는 순서, j는 x, y
    private readonly bool[][] isPlayerVelocityZero = {new bool[] {false, false}, new bool[] {false, false}};

    private float breakTime;
    private bool jumpBreak;

    GameObject gm;
    GameManager gameManager;
    ScoreManager scoreManager;
    
    void Awake(){
        gm = GameObject.Find("GameManager");
        gameManager = gm.GetComponent<GameManager>();
        scoreManager = gm.GetComponent<ScoreManager>();
        gm = null; // 게임 매니저 오브젝트 참조 해제
    }

    // 시작될 때 실행되는 코드
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        pam = GetComponent<PlayerAnimationManager>();
        col = GetComponent<Collider2D>();
        pam.isFirstJump = true;
        stopped = false;
        isDead = false;
        jumpLock = false;

        soundPlayManager = GameObject.Find("Sound Player");

        isWalkingSoundPlaying = false;

        if (gaugeBar.GetComponent<GaugeBarManager>() != null){
            gbm = gaugeBar.GetComponent<GaugeBarManager>();
            isTutorial = false;
        } else if (gaugeBar.GetComponent<TutorialGaugeBarManager>() != null){
            tgbm = gaugeBar.GetComponent<TutorialGaugeBarManager>();
            isTutorial = true;
        } else {
            Debug.LogError("[PlayerController] GaugeBarManager 컴포넌트를 찾을 수 없습니다.");
        }

        spriteRendererGauge = gaugeBar.GetComponent<SpriteRenderer>();

        // 고정 프레임 60
        Application.targetFrameRate = 60;
        // Vsync 비활성화
        QualitySettings.vSyncCount = 0;

        gravity *= gameSpeed;
        breakTime = 0f;

        rigid.gravityScale = gameSpeed;

        inputLeft = false;
        inputRight = false;
        inputJump = false;
        firstJumpUp = false;
        jumpBreak = false;

        uib = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIButtonManager>();
        uib.Init();
}

    // 프레임 당 초기화: 사용자 입력 감지
    void Update()
    {
        if (jumpLock && (Input.GetButtonUp("Jump") || firstJumpUp)){
            jumpLock = false;
        }

        // 타이머 업데이트
        if (landingFreezeTimer > 0f)
        {
            landingFreezeTimer = Mathf.Max(0f, landingFreezeTimer - Time.deltaTime);

            if (landingFreezeTimer <= 0f){  
                inputAxis = lastInputAxis;
                pam.isFirstJump = true;
            }
        }
        else
        {
            // 땅에 있을 때
            if (!pam.isJumping && !pam.isFalling && !pam.isCrashing)
            {
                if ((inputJump || Input.GetButton("Jump")) && rigid.velocity.y == 0 && !jumpLock)
                {
                    // doJumpReady 트리거를 한 번만 작동시키기 위한 조건문
                    // isFirstJump : 다음 점프는 첫 번째 점프이다.
                    if (pam.isFirstJump)
                    {
                        pam.isJumpReady = true;
                        pam.isFirstJump = false;

                        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jumpReady", 2f, 1.5f);

                        spriteRendererGauge.enabled = true;
                        stopped = true;
                        MoveAnimRandom();
                    }

                    // 점프 준비 시간 증가
                    jumpHoldTime = Mathf.Min(jumpHoldTime + 1, 181);

                    // 게이지바에 점프 준비 시간 동기화
                    // BugFix: jumpGauge가 179까지만 증가하는 버그
                    if (isTutorial){
                        tgbm.jumpGauge = jumpHoldTime;
                    } else {
                        gbm.jumpGauge = jumpHoldTime;
                    }

                    // 점프 캔슬 (1 프레임 오차 허용)
                    if (jumpHoldTime == 180){
                        gbm.NextFrameDisappear();
                        pam.isJumpReady = false;
                        GameObject dapInstance = Instantiate(daParticle);
                        dapInstance.transform.position = new Vector3(transform.position.x + pam.lookAt * 0.7f, transform.position.y, transform.position.z);
                    }
                }

                if (rigid.velocity.y == 0)
                {
                    if (jumpHoldTime >= 180){
                        jumpHoldTime = 0;
                        Landing(false);
                        pam.isFirstJump = false;
                        jumpLock = true;
                    }

                    else if ((Input.GetButtonUp("Jump") || firstJumpUp) && pam.isJumpReady) {
                        isJump = true;
                        pam.isJumping = true;
                        stopped = false;

                        jumpTime = 0f;
                    }
                    
                }

                firstJumpUp = false;

                // 땅에 있는데 y속도가 음수인 버그 우회 (물리랑 화면 간의 프레임 차이)
                if (rigid.velocity.y < 0)
                {
                    pam.isFirstJump = true;
                    jumpHoldTime = 0;
                }
            }

            // Update에서 변수 저장
            inputAxis = uib.buttonLock ? 0 : Input.GetAxisRaw("Horizontal");
            
            inputAxis = inputLeft ? -1 : inputAxis;
            inputAxis = inputRight ? 1 : inputAxis;
            inputAxis = inputLeft && inputRight ? 0 : inputAxis;

        }

        if (rigid.velocity.y != 0f){
            pam.onGround = false;
        }

        if (pam.onGround && !pam.isJumpReady && !pam.isCrashing && jumpHoldTime == 0){
            lastInputAxis = uib.buttonLock ? 0 : Input.GetAxisRaw("Horizontal");
            lastInputAxis = inputLeft ? -1 : lastInputAxis;
            lastInputAxis = inputRight ? 1 : lastInputAxis;
            lastInputAxis = inputLeft && inputRight ? 0 : lastInputAxis;

            if (lastInputAxis > 0) pam.lookAt = 1;
            else if (lastInputAxis < 0) pam.lookAt = -1;
        }

        jumpTime += Time.deltaTime;

        // Trail Particle Management
        TrailManager();

        currentVelocity = rigid.velocity;
        // inputAxis가 0인 상황에서 떨어지기 시작했을 때 이론 상은 문제 없지만, 프레임 차이로 발생하는 버그 수정
        if (rigid.velocity.y < 0f && rigid.velocity.x == 0 && stopped)
        {
            currentVelocity.x = movePower * pam.lookAt;
            rigid.velocity = currentVelocity;
            stopped = false;
            Debug.Log("[PlayerController] inputAxis가 0인 상황에서 떨어지기 시작했을 때 이론 상은 문제 없지만, 프레임 차이로 발생하는 버그 수정 (Update)");

            if (pam.isJumpReady){
                CancelJumpReady();
                Debug.Log("[PlayerController] 점프 준비 중 낙하 시 생기는 버그 수정 (Update)");
            }
        }

        // 기절 중에는 게이지바 제거
        if (pam.isCrashing) spriteRendererGauge.enabled = false;
    }

    private void FixedUpdate()
    {
        if (mainCameraController == null) mainCameraController = Camera.main.GetComponent<CameraController>();
        if (breakTime > 0f) {
            breakTime -= Time.deltaTime;
            stopped = false;
        }
        if (pam.isFalling && breakTime > 0f){
            pam.isCrashing = true;
            jumpBreak = true;
            rigid.velocity = new Vector2(rigid.velocity.x != 0f ? (rigid.velocity.x > 0 ? Mathf.Max(rigid.velocity.x / 2, 1f) : Mathf.Min(rigid.velocity.x / 2, -1f)) : 0, rigid.velocity.y);
        }
        if (!jumpBreak && breakTime > 0f){
            rigid.velocity = new Vector2(rigid.velocity.x / 2 , rigid.velocity.y);
            pam.isCrashing = true;
        } else if (!jumpBreak && breakTime <= 0f){
            breakTime = 0f;
            pam.isCrashing = false;
        }
        Animations(); // 애니메이션 담당 함수
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수

        /* 카메라 시점 변환 */
        if (uib.buttonLock){
            if (mainCameraController.maxHeight < transform.position.y - 7f){ /* Offset 설정 및 플레이어 위치, 카메라 위치 설정 */
                uib.buttonLock = false;
                mainCameraController.ChangeHeight(mainCameraController.maxHeight + 23.2f);
                GameManager.stage += 1;
                gameManager.ChangeBackground(GameManager.stage);
                StartCoroutine(CameraDifferenceAdjust());
            }
        }

        ScoreByHeight(); // 높이에 따른 점수 담당 함수

        isPlayerVelocityZero[1][0] = isPlayerVelocityZero[0][0];
        isPlayerVelocityZero[1][1] = isPlayerVelocityZero[0][1];

        if (rigid.velocity.x == 0f){
            isPlayerVelocityZero[0][0] = true;
        } else {
            isPlayerVelocityZero[0][0] = false;
        }
        if (rigid.velocity.y == 0f){
            isPlayerVelocityZero[0][1] = true;
        } else {
            isPlayerVelocityZero[0][1] = false;
        }

        if (isDead) rigid.velocity = Vector2.zero;

        if (maxHeight - offest < transform.position.y)
        {
            maxHeight = transform.position.y + offest;
        }

    }

    IEnumerator CameraDifferenceAdjust()
    {
        // 1프레임 대기 (Update 루프 기준)
        yield return new WaitForSeconds(0.4f);
        mainCameraController.difference = 0f;
    }

    // 낙하 감지
    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > 0.7f && (pam.isFalling 
            || isPlayerVelocityZero[0][1] == true && isPlayerVelocityZero[1][1] == true && pam.isJumping))
            {
                if (isPlayerVelocityZero[0][1] == true && isPlayerVelocityZero[1][1] == true && pam.isJumping){
                    Debug.Log("[PlayerController] 상승 중 착지 시 발생하는 버그 수정");
                }
                Landing();

                if (collision.gameObject.CompareTag("LastBlock")){
                    StartCoroutine(NextStageAnimaiton());
                } else {
                    uib.buttonLock = false;
                }
            }
        }
    }


    readonly float[] heightList = {134.7f, 320.7f, 540.7f};

    internal IEnumerator NextStageAnimaiton(){
        if (!uib.buttonLock) pam.lookAt = transform.position.x >= 0 ? -1 : 1;
        uib.buttonLock = true;
        pam.isJumpReady = true;
        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jumpReady");
        StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomCamera(pam.lookAt));
        // 90 프레임 대기
        yield return new WaitForSeconds(1.5f);
        GameManager.NextSound();
        pam.isJumpReady = false;
        pam.isJumping = true;
        pam.onGround = false;
        stopped = false;
        rigid.velocity = new Vector2(1.2f * pam.lookAt, Mathf.Sqrt(2 * gravity * (heightList[GameManager.stage - 1] - transform.position.y)));
        /* 콜라이더 해제 */
        col.enabled = false;

        /* 효과음 재생 */
        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jump2");

        
    }
    
    // 벽 충돌 시 실행
    private void OnCollisionEnter2D(Collision2D collision) {

        for (int i = 0; i < collision.contacts.Length; i++){
            ContactPoint2D contact = collision.contacts[i];
            // 플레이어가 왼쪽에서 오른쪽으로 충돌
            if (contact.normal.x < -0.7f && !collision.gameObject.CompareTag("PassBlock") && !(isPlayerVelocityZero[1][0] | isPlayerVelocityZero[0][1]))
            {
                if (pam.isJumping){
                    rigid.velocity = new Vector2(-1.5f, rigid.velocity.y/3);
                    pam.isCrashing = true;
                    jumpBreak = true;
                    pam.lookAt = 1;
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
                }
                else if (pam.isFalling){
                    rigid.velocity = new Vector2(-0.75f, rigid.velocity.y);
                    pam.isCrashing = true;
                    jumpBreak = true;
                    pam.lookAt = 1;
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
                }
            }
            // 플레이어가 오른쪽에서 왼쪽으로 충돌
            else if (contact.normal.x > 0.7f && !collision.gameObject.CompareTag("PassBlock") && !(isPlayerVelocityZero[1][0] | isPlayerVelocityZero[0][1]))
            {
                if (pam.isJumping){
                    rigid.velocity = new Vector2(1.5f, rigid.velocity.y/3);
                    pam.isCrashing = true;
                    jumpBreak = true;
                    pam.lookAt = -1;
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
                }
                else if (pam.isFalling){
                    rigid.velocity = new Vector2(0.75f, rigid.velocity.y);
                    pam.isCrashing = true;
                    jumpBreak = true;
                    pam.lookAt = -1;
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
                }
            }

        }
        
    }

    public Vector2 currentVelocity;

    private void Move()
    {

        // 플레이어의 속도를 직접적으로 설정 불가능하므로, 새로운 변수에 저장 후 수정
        currentVelocity = rigid.velocity;

        if (landingFreezeTimer > 0f)
        {
            inputAxis = 0;
            pam.isFirstJump = false;
            jumpHoldTime = 0;
        }

        // 점프 장전이 안 되어있고, 점프 중이지 않고, 낙하 중이지 않을 때
        else if (jumpHoldTime == 0 && !pam.isJumping && !pam.isFalling && !pam.isCrashing && !uib.buttonLock)
        {
            if (inputAxis < 0)
            {
                pam.lookAt = -1;
                stopped = false;
                currentVelocity.x = movePower * pam.lookAt;
                MoveAnimRandom();
            }
            else if (inputAxis > 0)
            {
                pam.lookAt = 1;
                stopped = false;
                currentVelocity.x = movePower * pam.lookAt;
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
        if (pam.isFalling && rigid.velocity.x == 0 && stopped)
        {
            currentVelocity.x = movePower * pam.lookAt;
            rigid.velocity = currentVelocity;
            stopped = false;
            Debug.Log("[PlayerController] inputAxis가 0인 상황에서 떨어지기 시작했을 때 이론 상은 문제 없지만, 프레임 차이로 발생하는 버그 수정");

            if (pam.isJumpReady){
                CancelJumpReady();
                Debug.Log("[PlayerController] 점프 준비 중 낙하 시 생기는 버그 수정");
            }
        }

        // 점프 장전 중에는 정지
        if (jumpHoldTime > 0)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            MoveAnimRandom();
        }

        // 걷는 효과음
        if (pam.isMove && !pam.isJumping && !pam.isFalling){
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
        rigid.velocity += new Vector2(moveX * Mathf.Sqrt(gravity / (8 * moveY)) * pam.lookAt, Mathf.Sqrt(2 * gravity * moveY));

        // Play Jump Sound
        if (jumpHoldTime < 54) soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jump1");
        else soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("jump2");;

        jumpHoldTime = Math.Min(jumpHoldTime, 119);
        if (isTutorial){
            tgbm.jumpGauge = jumpHoldTime;
        } else {
            gbm.jumpGauge = jumpHoldTime;
        }

        // 점프 후 초기화
        isJump = false;
        jumpHoldTime = 0;
        pam.onGround = false;
        pam.isJumpReady = false;
        MoveAnimRandom();
    }

    private void Animations()
    {
        // 이동 애니메이션 설정
        pam.isMove = inputAxis != 0 && jumpHoldTime == 0;

        // 낙하 애니메이션 설정
        if (rigid.velocity.y < 0)
        {
            if (!pam.isFalling)
            {
                pam.isFalling = true;
                pam.isJumping = false;
                pam.onGround = false;
                pam.isFirstJump = false;
                col.enabled = true;
                MoveAnimRandom();
            }
        }

        // 낙하 모션 중 땅에서 정지하는 버그 수정
        if (pam.isFalling && pam.isFirstJump && !pam.isCrashing)
        {
            Landing(false);
            Debug.Log("[PlayerController] 낙하 모션 중 땅에서 정지하는 버그가 발생하였으나 해결됨");
        }
    }

    private void Landing(bool playParticle = true)
    {
        if (breakTime > 0f) return;
        // 떨어질 때만 효과음 재생 (점프 취소 시 재생되는 오류 해결)
        if (pam.isFalling){
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("landing");
        }

        pam.isFalling = false;
        pam.isJumping = false;
        pam.isFirstJump = true;
        rigid.velocity = Vector2.zero;
        pam.onGround = true;
        pam.isCrashing = false;
        jumpBreak = false;

        // 동기화 원리 : 점프 키를 떼는 순간, jumpHoldTime은 0이 되지만, gaugeBar의 jumpGauge는 점프 키를 떼기 직전의 jumpHoldTime을 가지고 있으므로
        // jumpHoldTime이 0이 되어도 점프 중일 때 적절한 게이지바를 출력 
        if (isTutorial){
            tgbm.jumpGauge = jumpHoldTime;
        } else {
            gbm.jumpGauge = jumpHoldTime;
        }

        spriteRendererGauge.enabled = false;

        // 착지 후 일시적으로 멈추는 타이머 시작
        landingFreezeTimer = landingFreezeDuration;

        // 기절 중 착지하였는데, 착지 후 Landing 함수가 실행 될 때 Paricle 나오는 현상 해결
        if (isPlayerVelocityZero[1][1]){
            playParticle = false;
        }

        if (playParticle){
            GameObject landingPtcInstance = Instantiate(landingParticle);
            landingPtcInstance.transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        }
    }

    private void CancelJumpReady(){
        pam.isJumpReady = false;
        pam.isFirstJump = true;
        jumpHoldTime = 0;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.tag == "Dead" && !isDead){
            Die();
        }

        if (other.CompareTag("Taxi")){
            // 플레이어 콜라이더 너비: 0.3f
            float previousVelocity = 24.19f;
            pam.isCrashing = true;
            stopped = false;
            jumpBreak = false;
            CancelJumpReady();
            Vector2 direction = -(other.transform.position - transform.position).normalized;
            pam.lookAt = -(int)(direction.x / Mathf.Abs(direction.x));
            if (breakTime > 0f){
                breakTime = 0.1f;
                rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 5);
                return;
            }
            breakTime = 0.1f;
            rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 5);
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
        }

        if (other.CompareTag("Tiger")){
            // 플레이어 콜라이더 너비: 0.3f
            float previousVelocity = 50f;
            pam.isCrashing = true;
            stopped = false;
            jumpBreak = false;
            CancelJumpReady();
            Vector2 direction = -(other.transform.position - transform.position).normalized;
            pam.lookAt = -(int)(direction.x / Mathf.Abs(direction.x));
            if (breakTime > 0f){
                breakTime = 0.1f;
                rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 4.5f);
                return;
            }
            breakTime = 0.1f;
            rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 4.5f);
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("tiger");
        }

        if (other.CompareTag("Airplane")){
            // 플레이어 콜라이더 너비: 0.3f
            float previousVelocity = 75f;
            pam.isCrashing = true;
            stopped = false;
            jumpBreak = false;
            CancelJumpReady();
            Vector2 direction = -(other.transform.position - transform.position).normalized;
            pam.lookAt = -(int)(direction.x / Mathf.Abs(direction.x));
            if (breakTime > 0f){
                breakTime = 0.1f;
                rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 4.5f);
                return;
            }
            breakTime = 0.1f;
            rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 4.5f);
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
        }

        if (other.CompareTag("Laser")){
            // 플레이어 콜라이더 너비: 0.3f
            pam.isCrashing = true;
            stopped = false;
            jumpBreak = false;
            CancelJumpReady();
            Vector2 direction = -(other.transform.position - transform.position).normalized;
            pam.lookAt = -(int)(direction.x / Mathf.Abs(direction.x));
            if (breakTime > 0f){
                breakTime = 4f;
                rigid.velocity = new Vector2(0f, rigid.velocity.y / 4.5f);
                return;
            }
            breakTime = 4f;
            rigid.velocity = new Vector2(0f, rigid.velocity.y / 4.5f);
        }

        if (other.CompareTag("Spaceman")){
            // 플레이어 콜라이더 너비: 0.3f
            float previousVelocity = 24.19f;
            pam.isCrashing = true;
            stopped = false;
            jumpBreak = false;
            CancelJumpReady();
            Vector2 direction = -(other.transform.position - transform.position).normalized;
            pam.lookAt = -(int)(direction.x / Mathf.Abs(direction.x));
            if (breakTime > 0f){
                breakTime = 0.1f;
                rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 5);
                return;
            }
            breakTime = 0.1f;
            rigid.velocity = new Vector2(previousVelocity * direction.x / Mathf.Abs(direction.x), rigid.velocity.y / 5);
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("breakFx");
        }
    }


    private void ScoreByHeight(){
        if (maxHeight - height >= 1f){
            scoreManager.AddScore(1);
            height += 1;
        }
    }

    private void MoveAnimRandom(){
        if (UnityEngine.Random.Range(0, 2) == 0){
            pam.isIDLE1 = true;
        } else {
            pam.isIDLE1 = false;
        }
    }

    private void TrailManager(){
        if (pam.isJumping || pam.isFalling){
            trailParticle.SetActive(true);
        } else {
            trailParticle.SetActive(false);
        }
    }

    private void Die(){
        isDead = true;
        rigid.velocity = Vector2.zero;

        soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("failed");

        gameManager.EndingUIActive();
    }
}

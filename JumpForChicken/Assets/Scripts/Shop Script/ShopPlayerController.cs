using System;
using UnityEngine;

public class ShopPlayerController : MonoBehaviour
{
    private float gravity = 9.81f * 61 / 60;
    public float gameSpeed = 2.56f;

    // 사용자 입력 관련 (추후 삭제)
    public bool inputLeft = false;
    public bool inputRight = false;
    public bool inputJump = false; // 조작 시 점프 키 클릭 중
    public float inputAxis;
    public bool firstJumpUp = false; // 조작 시 점프 키 릴리즈

    public float movePower = 4f;
    public float jumpPower = 0.55f;


    private Rigidbody2D rigid;

    private PlayerAnimationManager pam;

    public GameObject landingParticle;
    private GameObject soundPlayManager;
    private bool isWalkingSoundPlaying = false;

    private int jumpHoldTime = 0;
    private bool isJump = false;

    private float moveX = 0;
    private float moveY = 0;

    private readonly float landingFreezeDuration = 0.1f; // 착지 후 일시 정지 시간
    private float landingFreezeTimer = 0f; // 착지 후 타이머

    // 행동 실행 중 프레임
    private int actFrame = 0;

    private int preActCode = 0; // 0: 처음, 1: 무행동, 2: 걷기, 3: 점프

    // 점프를 한 지 몇 턴이 되었는지 기록
    private int jumpTurn = 0;

    // 행동이 끝나면 true
    private bool isActDone = true;
    private int randomJumpHoldTime = 0;



    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        pam = GetComponent<PlayerAnimationManager>();
        pam.isFirstJump = true;

        soundPlayManager = GameObject.Find("Sound Player");
        isWalkingSoundPlaying = false;

        Application.targetFrameRate = 60;

        gravity *= gameSpeed;
        rigid.gravityScale = gameSpeed;

        inputJump = false;
        firstJumpUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        RandomActivity();
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
            if (!pam.isJumping && !pam.isFalling)
            {
                if ((inputJump || Input.GetButton("Jump")) && rigid.velocity.y == 0)
                {
                    // doJumpReady 트리거를 한 번만 작동시키기 위한 조건문
                    // isFirstJump : 다음 점프는 첫 번째 점프이다.
                    if (pam.isFirstJump)
                    {
                        pam.isFirstJump = false;
                        pam.isJumpReady = true;

                        soundPlayManager.GetComponent<ShopSoundPlayManager>().PlaySound("jumpReady", 2f, 1.5f);

                        MoveAnimRandom();
                    }

                    // 점프 준비 시간 증가
                    jumpHoldTime = Mathf.Min(jumpHoldTime + 1, 181);
                }

                if ((Input.GetButtonUp("Jump") || firstJumpUp) && rigid.velocity.y == 0)
                {
                    isJump = true;
                    pam.isJumping = true;
                }

                firstJumpUp = false;

                // 땅에 있는데 y속도가 음수인 버그 우회 (물리랑 화면 간의 프레임 차이)
                if (rigid.velocity.y < 0)
                {
                    pam.isFirstJump = true;
                    jumpHoldTime = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Move(); // 움직임 담당 함수
        Jump(); // 점프 담당 함수
        Animations(); // 애니메이션 담당 함수
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > 0.7f && pam.isFalling)
        {
            Landing();
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
        else if (jumpHoldTime == 0 && !pam.isJumping && !pam.isFalling)
        {
            if (inputAxis < 0)
            {
                pam.lookAt = -1;
                currentVelocity.x = movePower * pam.lookAt;
;
                MoveAnimRandom();
            }
            else if (inputAxis > 0)
            {
                pam.lookAt = 1;
                currentVelocity.x = movePower * pam.lookAt;
                MoveAnimRandom();
            }
            else
            {
                currentVelocity.x = 0;
            }
            rigid.velocity = currentVelocity;
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
                soundPlayManager.GetComponent<ShopSoundPlayManager>().PlaySound("walking");
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            if (isWalkingSoundPlaying)
            {
                soundPlayManager.GetComponent<ShopSoundPlayManager>().StopSound("walking");
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
        rigid.velocity += new Vector2(moveX * Mathf.Sqrt(gravity / (8 * moveY)) * pam.lookAt, Mathf.Sqrt(2 * gravity * moveY) * jumpPower);

        // Play Jump Sound
        if (jumpHoldTime < 54) soundPlayManager.GetComponent<ShopSoundPlayManager>().PlaySound("jump1");
        else soundPlayManager.GetComponent<ShopSoundPlayManager>().PlaySound("jump2");;

        jumpHoldTime = Math.Min(jumpHoldTime, 119);
        pam.onGround = false;

        // 점프 후 초기화
        isJump = false;
        pam.isJumpReady = false;
        jumpHoldTime = 0;
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
                pam.isJumping = false;
                pam.isFalling = true;
                pam.onGround = false;
                pam.isFirstJump = false;
                MoveAnimRandom();
            }
        }

        // 낙하 모션 중 땅에서 정지하는 버그 수정
        if (pam.isFalling && pam.isFirstJump)
        {
            Landing();
        }
    }

    private void Landing(bool playParticle = true)
    {
        // 떨어질 때만 효과음 재생 (점프 취소 시 재생되는 오류 해결)
        if (pam.isFalling){
            soundPlayManager.GetComponent<ShopSoundPlayManager>().PlaySound("landing");
        }

        pam.isFalling = false;
        pam.isJumping = false;
        pam.isFirstJump = true;
        rigid.velocity = Vector2.zero;
        pam.onGround = true;
        pam.isCrashing = false;

        // 착지 후 일시적으로 멈추는 타이머 시작
        landingFreezeTimer = landingFreezeDuration;

        if (playParticle){
            GameObject landingPtcInstance = Instantiate(landingParticle);
            landingPtcInstance.transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        }
    }

    private void MoveAnimRandom(){
        if (UnityEngine.Random.Range(0, 2) == 0){
            pam.isIDLE1 = true;
        } else {
            pam.isIDLE1 = false;
        }
    }

    // 랜덤 움직임 함수 선언
    private void RandomWalk(){
        isActDone = false;
        preActCode = 2;
        actFrame = UnityEngine.Random.Range(25, 50);; // 랜덤 가능
        
        inputAxis = transform.position.x >= 0 ? -1 : 1;
        jumpTurn++;
    }

    private void RandomJump(){
        isActDone = false;
        preActCode = 3;
        actFrame = -1; // 착지할 때까지 지속
        pam.lookAt = transform.position.x >= 0 ? -1 : 1; // 중앙 바라보기
        randomJumpHoldTime = UnityEngine.Random.Range(40, 91); // 40~91 Frame

        jumpTurn = 0;
    }

    private void RandomRest(){
        if (preActCode == 2) inputAxis = 0; // 이전에 걸었다면 초기화
        
        pam.lookAt = transform.position.x >= 0 ? -1 : 1; // 중앙 바라보기

        // preActCode == 3 일 때는 알아서 초기화

        actFrame = 240;
        preActCode = 1;
        isActDone = false;
    }

    private void RandomActivity(){
        if (actFrame > 0){
            actFrame--;
        } else if (actFrame == -1){ // 점프 중이면
            if (randomJumpHoldTime > 0){
                inputJump = true;
                randomJumpHoldTime--;
            } else if (randomJumpHoldTime == 0) {
                inputJump = false;
                firstJumpUp = true;
                pam.onGround = false;
                randomJumpHoldTime = -1;
            }

            if (pam.onGround && randomJumpHoldTime < 0){ // 착지했음
                actFrame = 0;
                isActDone = true;
            }
        } else { // actFrame == 0
            isActDone = true;
        }

        if (!isActDone) return;

        if (preActCode != 1){ // 전 행동이 무행동이 아니라면
            RandomRest();
            return; // 탈출
        }

        int random = UnityEngine.Random.Range(0, 3); // 0 ~ 2
        // 만약 점프 할 때가 되었다면
        if (jumpTurn >= 2){
            // 반반 확률로 걷기 vs 점프
            if (random != 1){ // 점프
                RandomJump();
            } else { // 걷기
                RandomWalk();
            }
        } else { // 아직 점프할 때가 아니라면
            RandomWalk(); // 걷기
        }
    }
}

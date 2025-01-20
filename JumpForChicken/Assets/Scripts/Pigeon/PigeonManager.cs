using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : MonoBehaviour
{


    public GameObject chickenPrefab;

    GameObject cameras;

    public float moveSpeed = 0.06f;

    // objectSize는 직접 수정
    public float objectSize = 1f;

    public float surviveTime = 6f; // 초 단위

    private float xPos = 0;

    private int moveVector = 1;

    private float randomDropTime;

    private float yPosOffset = 0;
    private float yPosCurve = 0;

    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.main.gameObject; // 메인 카메라를 찾음

        moveVector = Random.Range(1, 3) == 1 ? -1 : 1; // 1 or -1 선택
        xPos = Random.Range(-4.5f + objectSize/2, 4.5f - objectSize/2); // 랜덤 좌표 선택 (화면 안)

        surviveTime = 6f; // 6초 생존
        yPosOffset = 0f; // 등장 및 퇴장을 자연스럽게 만들기 위해 y좌표를 설정해야 함 yPosOffset -> objectSize
        yPosCurve = objectSize; // 등장을 지수적으로 하기 위해서 만듬

        randomDropTime = Random.Range(1f, 5f); // 치킨을 떨어뜨릴 시간을 float로 구함

        transform.position = new Vector3(xPos, cameras.transform.position.y + 8f + objectSize/2, 0); // 화면 바로 위
    }

    // 일시정지 기능에 의해 Update -> FixedUpdate
    // FixedUpdate로 설정하면, 비둘기가 카메라 상단에 고정되지 않음
    void Update()
    {
        if (Time.timeScale > 0f){
            transform.position = new Vector3(xPos, cameras.transform.position.y + 8f + objectSize/2 - yPosOffset, 0);
            Move();

            if (surviveTime > 0){ 
                surviveTime -= Time.deltaTime;
                if (randomDropTime > surviveTime){
                    ChickenDrop();
                    randomDropTime = -1f; // 재실행 방지
                }
            } else {
                // 예상 개발 기능을 위해 함수화 (여러 번 실행)
                Disappear();
            }
        }
    }

    void Move(){
        // 비둘기가 좌우로 이동 (벽에 부딪히면 반대로 방향을 바꿈)
        if (xPos + moveSpeed*moveVector >= 4.5f - objectSize/2){
            xPos = 4.5f - objectSize/2;
            moveVector *= -1;
        } else if (xPos + moveSpeed*moveVector <= -4.5f + objectSize/2){
            xPos = -4.5f + objectSize/2;
            moveVector *= -1;
        } else {
            xPos += moveSpeed * moveVector;
        }

        // 비둘기 등장 (지수) 및 퇴장 (직선)
        if (yPosCurve >= 0){
            if (yPosCurve > 0.005){ // 약 66프레임에 완전히 도착, 약 1.1초 / 시간 상수 14프레임, 약 0.24초
                yPosOffset += yPosCurve/15;
                yPosCurve -= yPosCurve/15;
            } else{
                yPosOffset = objectSize;
                yPosCurve = 0;
            }
        } else {
            yPosOffset -= objectSize/15; // 15프레임 만에 완전히 올라감, 약 0.25초
        }
    }

    void ChickenDrop(){
        GameObject chickens = Instantiate(chickenPrefab);
        chickens.transform.position = new Vector3(xPos, gameObject.transform.position.y - 0.3f, 0);
    }

    void Disappear(){
        yPosCurve = -1; // 변수량을 줄이기 위해 주어진 상황에서 사용하지 않는 변수를 사용
        moveSpeed *= 0.96f; // 올라갈 때에는 x축 이동속도를 늦춤, 약 0.54배 조정
        Destroy(gameObject, 0.3f); // 0.25초보다 크게
    }
}

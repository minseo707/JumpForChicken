using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TitleCameraController : MonoBehaviour
{
    public float startCameraHeight = 16f;

    public float maxTouchTime = 1f;

    private float leftHeight = 0f;

    private float touchTime = 0f;

    private bool once;

    private MainMusicManager mmm;
    public GameObject settingUI;

    void Awake() {
        if (PlayerPrefs.GetInt("Init") == 0){
            // 처음 높이 초기화
            transform.position = new Vector3(0, startCameraHeight, transform.position.z);
            PlayerPrefs.SetInt("Init", 1);
        } else {
            transform.position = new Vector3(0, 0, transform.position.z);
        }
        once = false;
    }

    void Start() {
        leftHeight = 0f;
        
        // 고정 프레임 60
        Application.targetFrameRate = 60;
        // Vsync 비활성화
        QualitySettings.vSyncCount = 0;

        mmm = MainMusicManager.Instance;
        if (transform.position.y == 0) mmm.MusicPlay();
    }

    void Update() {

        if (touchTime > 0){
            touchTime += Time.deltaTime;
        }

        if (transform.position.y < 1f && !once){
            mmm.MusicPlay();
            once = true;
        }

        // 터치 이벤트 감지가 필요 없을 때에는 이후의 코드를 무시
        if (transform.position.y == 0) return;

        // 터치 시간이 일정 시간 지나면 터치 취소
        if (Input.touchCount > 0){
            if (Input.GetTouch(0).phase == TouchPhase.Began){
                touchTime += Time.deltaTime;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended){
                if (touchTime <= maxTouchTime){
                    Skip();
                    Debug.Log("[TitleCameraController] Tapped!");
                }
                touchTime = 0f;
            }
        }

        // 마우스로도 정상적으로 작동하기 위해서
        if (Input.GetMouseButtonDown(0)){
            touchTime += Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(0)){
            if (touchTime <= maxTouchTime){
                Skip();
                Debug.Log("[TitleCameraController] Clicked!");
            }
            touchTime = 0f;
        }


        // 카메라 움직임
        if (leftHeight == 0){ // 선형적으로 내려올 때
            if ((0.005f <= transform.position.y) && (transform.position.y <= 1.5f)){
                transform.position = new Vector3(0, transform.position.y/1.024f, transform.position.z);
            } else if (0.005f > transform.position.y){
                transform.position = new Vector3(0, 0, transform.position.z);
            } else {transform.position = new Vector3(0, transform.position.y - 0.03f, transform.position.z);}
        } else {
            if (leftHeight >= 0.005f){ // 지수적으로 내려올 때
                transform.position = new Vector3(0, Mathf.Max(0, transform.position.y - leftHeight/9), transform.position.z);
                leftHeight -= leftHeight/9;
            } else {
                transform.position = new Vector3(0, 0, transform.position.z);
                leftHeight = 0f;
            }
        }
        if (transform.position.y == 0){
            Ready(); // 위 무시 로직에 의해 한 번만 실행
        }
    }
    void Skip(){
        leftHeight = transform.position.y;
    }

    void Ready(){
        
    }
}

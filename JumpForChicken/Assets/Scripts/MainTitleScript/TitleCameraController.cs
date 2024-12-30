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

    void Start() {
        leftHeight = 0f;
    }

    void Awake() {
        transform.position = new Vector3(0, startCameraHeight, transform.position.z);
    }

    void Update() {

        if (touchTime > 0){
            touchTime += Time.deltaTime;
        }

        // 터치 시간이 일정 시간 지나면 터치 취소
        if (Input.touchCount > 0){
            if (Input.GetTouch(0).phase == TouchPhase.Began){
                touchTime += Time.deltaTime;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended){
                if (touchTime <= maxTouchTime){
                    Skip();
                    Debug.LogError("Touched!");
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
                Debug.LogError("Clicked!");
            }
            touchTime = 0f;
        }

        // 카메라 움직임
        if (leftHeight == 0){
            if ((0.005f <= transform.position.y) && (transform.position.y <= 1.5f)){
                transform.position = new Vector3(0, transform.position.y/1.024f, transform.position.z);
            } else if (0.005f > transform.position.y){
                transform.position = new Vector3(0, 0, transform.position.z);
            } else {transform.position = new Vector3(0, transform.position.y - 0.03f, transform.position.z);}
        } else {
            if (leftHeight >= 0.005f){
                transform.position = new Vector3(0, Mathf.Max(0, transform.position.y - leftHeight/9), transform.position.z);
                leftHeight -= leftHeight/9;
            } else {
                transform.position = new Vector3(0, 0, transform.position.z);
                leftHeight = 0f;
            }
        }
        if (transform.position.y == 0){
            Ready();
        }
    }

    void Skip(){
        leftHeight = transform.position.y;
    }

    void Ready(){
        
    }
}

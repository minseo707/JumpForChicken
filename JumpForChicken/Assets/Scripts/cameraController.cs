﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 인게임에서의 카메라 움직임 관리
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Player Object")]
    public GameObject player;

    [Header("Offset")]
    public float cameraHeight = 1f;
    public float floor = -2.231772f;

    [Header("Camera Speed")]
    public float secPerTile = 10f;
    private readonly float[] cameraSpeeds = {0.35f, 0.41f, 0.5f, 0.5f};

    [Description("Add Speed Per 5Meters")]
    private float addSpeed1 = 0.02f; // 0 ~ 100
    private float addSpeed2 = 0.015f; // 100 ~ 200
    private float addSpeed3 = 0.010f; // 200 ~ 300 (1.4)
    private float addSpeed4 = 0.005f; // 300 ~

    private float speedOffsetStage4 = 0f;

    [Header("Default Floor Array")]
    [SerializeField] public float[] floors = {0f, 0.5f, 1f, 1.5f};

    private int lastStage = -1;


    // 데드라인 실수 보강
    [Header("Deadline Backwards")]
    public float backwards = 4f;

    public float difference = 0f;
    private float tempDiff = 0f;

    [Header("Scores")]
    public float maxHeight = 0f;

    private float startCamera;
    private float totalDiff;
    private bool trig;
    private PlayerAnimationManager pam;

    private GameManager gm;

    [Header("Stage 2-Leap Particle")]
    [SerializeField] GameObject leapParticle;

    private Camera cameras;

    [Header("Spaceman Config")]
    [SerializeField] private GameObject spacemanPrefab;
    [SerializeField] private float spacemanSpawnHeight = 600f;
    private bool spacemanActive = false;

    [Header("Warning Particle")]
    [SerializeField] GameObject warningParticle;


    void Start()
    {
        // 초기 카메라 위치 설정
        transform.position = new Vector3(0, cameraHeight, -10);
        startCamera = cameraHeight - floor;
        pam = player.GetComponent<PlayerAnimationManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        cameras = GetComponent<Camera>();
        totalDiff = 0f;
        difference = 0f;
        tempDiff = 0f;
        trig = false;
        spacemanActive = false;
        speedOffsetStage4 = 0f;
    }

    void FixedUpdate()
    {
        if (GameManager.stage == 1){
            maxHeight = 120.333f;
        } else if (GameManager.stage == 2){
            maxHeight = 306.666f;
        } else if (GameManager.stage == 3){
            maxHeight = 527f;
        } else if (GameManager.stage == 4){
            maxHeight = 1000f;
        }

        if (GameManager.stage == 4 && !spacemanActive && cameraHeight > spacemanSpawnHeight){
            // Instantiate Spaceman
            Instantiate(spacemanPrefab, new Vector3(0, 10f, 0), Quaternion.identity);
            spacemanActive = true;
        }

        // 카메라 높이 조정 (하강 과정)
        if (player.transform.position.y < cameraHeight - 7f){
            totalDiff = 0;
            tempDiff = cameraHeight - player.transform.position.y - 7f;
            if (tempDiff + difference >= backwards){
                tempDiff = backwards - difference;
            }
            difference += tempDiff;
            cameraHeight -= tempDiff;
            tempDiff = 0f;
        }
        
        // 카메라 높이 조정 (상승 과정)
        if (pam.onGround && cameraHeight - player.transform.position.y < startCamera - floors[GameManager.stage - 1] && !trig)
        {
            if (GameManager.stage < 4){
                totalDiff = Mathf.Min(player.transform.position.y + startCamera - floors[GameManager.stage - 1] - cameraHeight, maxHeight - cameraHeight);
            } else {
                totalDiff = player.transform.position.y + startCamera - floors[GameManager.stage - 1] - cameraHeight;
            }
            
            trig = true;
        }

        if (!pam.onGround)
        {
            trig = false;
        }

        if (GameManager.stage == 4){
            if ((int)cameraHeight - 527 <= 100){
                speedOffsetStage4 = ((int)cameraHeight - 527) / 5 * addSpeed1;
            } else if ((int)cameraHeight - 527 <= 200) {
                speedOffsetStage4 = 20 * addSpeed1 + ((int)cameraHeight - 527 - 100) / 5 * addSpeed2;
            } else if ((int)cameraHeight - 527 <= 300) {
                speedOffsetStage4 = 20 * addSpeed1 + 20 * addSpeed2 + ((int)cameraHeight - 527 - 200) / 5 * addSpeed3;
            } else {
                speedOffsetStage4 = 20 * addSpeed1 + 20 * addSpeed2 + 20 * addSpeed3 + ((int)cameraHeight - 527 - 300) / 5 * addSpeed4;
            }
        }
        // 카메라 상승 속도
        cameraHeight += Time.deltaTime * (cameraSpeeds[GameManager.stage - 1] + (GameManager.stage == 4 ? speedOffsetStage4 : 0));
        Debug.Log("Speed: " + (cameraSpeeds[GameManager.stage - 1] + (GameManager.stage == 4 ? speedOffsetStage4 : 0)));

        if (totalDiff > 1e-4)
        {
            cameraHeight = GameManager.stage < 4 ? Mathf.Min(cameraHeight + totalDiff / 16, maxHeight) : cameraHeight + totalDiff / 16;
            difference = Mathf.Max(0, difference - totalDiff / 64);
            totalDiff -= totalDiff / 16;
        }
        else
        {
            cameraHeight = GameManager.stage < 4 ? Mathf.Min(cameraHeight + totalDiff / 16, maxHeight) : cameraHeight + totalDiff / 16;
            totalDiff = 0;
        }

        // 카메라 위치 업데이트
        transform.position = new Vector3(transform.position.x, cameraHeight, -10);

        if (GameManager.stage != lastStage)
        {
            leapParticle.SetActive(GameManager.stage == 2);
            lastStage = GameManager.stage;
        }

        // 일정 시간 동기화되지 않은 인스턴스는 삭제
        warnings.RemoveAll(warning =>
        {
            
            warning.frame--;
            if (warning.frame <= 0)
            {
                Destroy(warning.instance);
                return true;
            }
            warning.instance.transform.position = new Vector2(warning.x, cameraHeight + 6.25f);
            return false;
        });
    }

    [Header("Zoom Camera")]
    public float scale = 0f;

    public IEnumerator ZoomCamera(int direction){
        float rotateTheta = 0f;
        int runFrame = 0;

        while (runFrame < 90){
            if (Time.timeScale == 0f){
                yield return null;
            } else {
                runFrame++;
                rotateTheta += 2f / 90f * direction;
                scale += 0.5f / 90f;
                transform.rotation = Quaternion.Euler(0, 0, rotateTheta);
                cameras.orthographicSize = (8f - scale) * 1f / (Mathf.Sqrt(2) * Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(rotateTheta) + Mathf.PI / 4f));
                yield return null;
            }
            
        }

        while (runFrame >= 60){
            if (Time.timeScale == 0f){
                yield return null;
            } else {
                runFrame--;
                rotateTheta -= 2f / 30f * direction;
                scale = jisuCalc((90 - runFrame) / 30f, 0.5f);
                transform.rotation = Quaternion.Euler(0, 0, rotateTheta);
                cameras.orthographicSize = (8f - scale) * 1f / (Mathf.Sqrt(2) * Mathf.Sin(Mathf.Deg2Rad * Mathf.Abs(rotateTheta) + Mathf.PI / 4f));
                yield return null;
            }
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);
        cameras.orthographicSize = 8;
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        yield return null;
        

    }

    private float cameraPosCalc(float t, int direction){
        return -direction * Mathf.Min(9f / 2f * 0.8f / 8f, Mathf.Abs(player.transform.position.x)) * t;
    }

    private float jisuCalc(float t, float firstValue){
        return firstValue * Mathf.Pow(1/2f, t*7);
    }
 
    public void ChangeHeight(float height){
        cameraHeight = height;

        transform.position = new Vector3(0, cameraHeight, -10);

        totalDiff = 0;
        difference = 0;
        tempDiff = 0;
    }

    private List<Warning> warnings = new();

    public void ViewWarning(float x){
        // 만약 x에 대한 경고가 없으면 추가
        if (!warnings.Exists(w => w.x == x)){
            warnings.Add(new Warning{
                x = x,
                frame = 2,
                instance = Instantiate(
                    warningParticle, 
                    new Vector3(x, cameraHeight + 6.25f, 0), 
                    Quaternion.identity
                )
            });
        }

        // 만약 x에 대한 경고가 있으면 frame 초기화
        foreach (var warning in warnings.Where(w => w.x == x)){
            warning.frame = 2;
        }
    }
}

class Warning {
    public float x;
    public int frame;
    public GameObject instance;

}
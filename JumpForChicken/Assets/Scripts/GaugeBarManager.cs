using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class GaugeBarManager : MonoBehaviour
{
    // jumpHoldTime
    public int jumpGauge = 0;

    // 플레이어 위치 가져오기 위함
    public GameObject player;

    // 카메라의 좌표를 가져오기 위함
    public GameObject cameras;
    private CameraController cc;
    
    // 플레이어 애니메이션 매니저 안에 있는 'lookAt' 변수 가져오기 위함
    private PlayerAnimationManager pam;

    // 플레이어 위치 불러올 변수 Vector3
    private Vector3 playerPos;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    
    // Sound Play Manager
    private GameObject soundPlayManager;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        pam = player.GetComponent<PlayerAnimationManager>();
        soundPlayManager = GameObject.Find("Sound Player");

        cc = cameras.GetComponent<CameraController>();
    }

    void Update()
    {
        
        // 플레이어 위치 동기화 (플레이어 lookAt 반영)

        playerPos = player.transform.position;

        playerPos = new Vector3(player.transform.position.x + pam.lookAt * 0.7f, Mathf.Max(player.transform.position.y, cc.cameraHeight - 7.5f), player.transform.position.z); // (x, y, z)

        transform.position = playerPos;
        transform.localScale = new Vector3(-pam.lookAt, 1, 1);


        // 스프라이트 동기화
        switch (jumpGauge){
            case int n when 0 <= n && n < 18:
                spriteRenderer.sprite = sprites[0];
                break;

            case int n when 18 <= n && n < 36:
                spriteRenderer.sprite = sprites[1];
                break;

            case int n when 36 <= n && n < 54:
                spriteRenderer.sprite = sprites[2];
                break;

            case int n when 54 <= n && n < 72:
                spriteRenderer.sprite = sprites[3];
                break;

            case int n when 72 <= n && n < 90:
                spriteRenderer.sprite = sprites[4];
                break;

            case int n when n >= 90:
                spriteRenderer.sprite = sprites[5];
                break;
        }

        if (jumpGauge >= 120){
            Shake();
        }

        if (((jumpGauge % 18) == 0) && (jumpGauge <= 90) && jumpGauge != 0 && pam.isJumpReady){
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("tick", 1f, 1f + (jumpGauge - 18) / 100f);
        }

        if (jumpGauge == 180){
            soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("gaugeDisappear");
        }
    }

    void Shake(){
        float shakeX = Mathf.Sin(Time.time * 60f) * .042f;
        transform.position = new Vector3(player.transform.position.x + pam.lookAt * 0.7f + shakeX, transform.position.y, transform.position.z);
    }
}

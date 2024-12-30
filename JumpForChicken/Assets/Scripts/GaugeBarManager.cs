using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GaugeBarManager : MonoBehaviour
{
    // jumpHoldTime
    public int jumpGauge = 0;

    // 임시 소리 테스트
    public AudioSource audioSource;
    public AudioClip[] audioClips;  // 여러 AudioClip

    // 플레이어 위치 가져오기 위함
    public GameObject player;

    // 카메라의 좌표를 가져오기 위함
    public GameObject cameras;
    
    // 플레이어 애니메이터 안에 있는 'lookAt' 변수 가져오기 위함
    private Animator playerAnimator;

    // 플레이어 위치 불러올 변수 Vector3
    private Vector3 playerPos;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;



    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];

        playerAnimator = player.GetComponent<Animator>();
    }

    void Update()
    {
        
        // 플레이어 위치 동기화 (플레이어 lookAt 반영)

        playerPos = player.transform.position;

        playerPos = new Vector3(player.transform.position.x + playerAnimator.GetInteger("lookAt") * 0.7f, Mathf.Max(player.transform.position.y, cameras.GetComponent<CameraController>().cameraHeight - 7.5f), player.transform.position.z); // (x, y, z)
        // playerPos = new Vector3(player.transform.position.x + playerAnimator.GetInteger("lookAt") * 0.7f, player.transform.position.y, player.transform.position.z); // (x, y, z)

        transform.position = playerPos;
        transform.localScale = new Vector3(-playerAnimator.GetInteger("lookAt"), 1, 1);


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

        if (((jumpGauge % 18) == 0) && (jumpGauge <= 90) && jumpGauge != 0){
            audioSource.clip = audioClips[0];
            audioSource.pitch = 1f + (jumpGauge - 18) / 100f;
            audioSource.Play();
        }

        if (jumpGauge == 180){
            audioSource.volume = 1f;
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
    }

    void Shake(){
        float shakeX = Mathf.Sin(Time.time * 60f) * .042f;
        transform.position = new Vector3(player.transform.position.x + playerAnimator.GetInteger("lookAt") * 0.7f + shakeX, transform.position.y, transform.position.z);
    }
}

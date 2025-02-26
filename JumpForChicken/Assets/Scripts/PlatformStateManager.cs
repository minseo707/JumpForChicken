﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformStateManager : MonoBehaviour
{
    public enum PlatformState
    {
        Current, // 현재 발판
        Next,    // 다음 발판
        Past,    // 지나온 발판
        Default  // 기본 상태
    }

    public PlatformState state = PlatformState.Default;

    private static List<PlatformStateManager> allPlatforms = new List<PlatformStateManager>();
    private static PlatformStateManager nextPlatform;
    private bool isPlayerOnPlatform = false;
    private float timeOnPlatform = 0f;
    public float requiredTimeOnPlatform = 0.1f; // 플레이어가 발판 위에 있어야 하는 시간 (초)
    private bool stateChanged = false; // 상태 변경 플래그 (1번만 실행)
    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Collider2D nextPlatformCollider;

    private Renderer objectRenderer;
    private Material originalMaterial;
    private Color originalColor;

    private float transparency = 0.3f; // 투명도 비율 (0.0f = 완전 투명, 1.0f = 완전 불투명)

    private void Start()
    {
        
        // 모든 발판을 리스트에 추가
        allPlatforms.Add(this);

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // 원래의 머티리얼 저장
            originalMaterial = objectRenderer.material;
            originalColor = originalMaterial.color;
        }

        // 초기 상태에 따라 레이어 설정
        SetPlatformLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어가 발판에 닿았을 때
        if (collision.collider.CompareTag("Player"))
        {
            playerRigidbody = collision.collider.GetComponent<Rigidbody2D>();
            playerCollider = playerRigidbody.GetComponent<Collider2D>();
            isPlayerOnPlatform = true;
            timeOnPlatform = 0f; // 착지 시간 초기화
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어가 발판 위에 머무를 때
        if (collision.collider.CompareTag("Player") && collision.contacts[0].normal.y < -0.7f)
        {
            if (playerRigidbody != null && isPlayerOnPlatform)
            {
                timeOnPlatform += Time.deltaTime;

                // 플레이어가 발판 위에 충분히 머무르면 상태를 변경
                if (timeOnPlatform >= requiredTimeOnPlatform && state == PlatformState.Next && !stateChanged)
                {
                    if (IsPlayerLanding())
                    {
                        // 상태 변경 플래그 설정
                        stateChanged = true;

                        // 다음 발판 설정
                        UpdateNextPlatform();

                        if (stateChanged){
                            // 모든 Current 발판을 Past로 변경
                            foreach (var platform in allPlatforms)
                            {
                                if (platform.state == PlatformState.Current)
                                {
                                    platform.state = PlatformState.Past;
                                    platform.SetPlatformLayer();
                                }
                            }

                            // 현재 Next 발판을 Current로 변경
                            if (nextPlatform != null)
                            {
                                nextPlatform.state = PlatformState.Current;
                                nextPlatform.SetPlatformLayer();

                                // 이펙트와 효과음 재생
                                PlayEffectAndSound();
                            }

                            nextPlatform.state = PlatformState.Next;
                            nextPlatform.SetPlatformLayer();
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 플레이어가 발판에서 떨어졌을 때
        if (collision.collider.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
            timeOnPlatform = 0f; // 착지 시간 초기화
            playerRigidbody = null; // Rigidbody2D 참조 해제
        }
    }

    private bool IsPlayerLanding()
    {
        if (playerRigidbody == null)
            return false;

        // 플레이어의 X, Y축 속도가 거의 0일 때 착지로 간주 (발판에 딱 붙어서 못 올라가는 일부 경우에 작동하는 버그가 있어 일단 Y축 속도 감지값을 낮춤)
        return Mathf.Abs(playerRigidbody.velocity.y) <= 0.01f && Mathf.Abs(playerRigidbody.velocity.x) <= 0.1f;
    }

    private void SetPlatformLayer()
    {
        // 현재 상태에 따라 레이어 설정
        switch (state)
        {
            case PlatformState.Current:
                gameObject.layer = LayerMask.NameToLayer("CurrentPlatform");
                SetTransparency(false);
                break;
            case PlatformState.Next:
                gameObject.layer = LayerMask.NameToLayer("NextPlatform");
                SetTransparency(false);
                break;
            case PlatformState.Past:
                gameObject.layer = LayerMask.NameToLayer("PastPlatform");
                break;
            default:
                gameObject.layer = LayerMask.NameToLayer("DefaultPlatform"); // DefaultPlatform 레이어 설정
                SetTransparency(true);
                break;
        }
    }

    private void UpdateNextPlatform()
    {
        // 현재 발판의 다음 발판을 찾아 상태를 Next로 변경
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10f); // 10f는 적절한 반경으로 설정
        PlatformStateManager newNextPlatform = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("DefaultPlatform"))
            {
                PlatformStateManager platformManager = hitCollider.GetComponent<PlatformStateManager>();
                if (platformManager != null)
                {
                    if (newNextPlatform == null)
                    {
                        newNextPlatform = platformManager;
                        nextPlatformCollider = hitCollider;
                    }
                    else
                    {
                        // 가장 y좌표가 작은 발판을 선택
                        if (platformManager.transform.position.y < newNextPlatform.transform.position.y)
                        {
                            newNextPlatform = platformManager;
                            nextPlatformCollider = hitCollider;
                        }

                    }
                }
            }
        }

        // 가장 가까운 발판을 Next로 설정
        if (newNextPlatform != null)
        {
            nextPlatform = newNextPlatform;
            if (nextPlatformCollider.bounds.Intersects(playerCollider.bounds)){
                stateChanged = false;
                Debug.Log("[PlatformStateManager] 플레이어가 끼는 버그가 발생하였지만 해결됨");
                return;
            } 
        }
    }

    // 모든 발판의 리스트를 초기화 (게임 시작 시 호출)
    public static void InitializePlatforms(List<PlatformStateManager> platforms)
    {
        allPlatforms = platforms;
        // 초기 상태 설정 로직 추가 가능
    }

    // 상태 변경 플래그 초기화
    public void ResetStateChanged()
    {
        stateChanged = false;
    }

    // 투명도 조절
    public void SetTransparency(bool transparent)
    {
        if (objectRenderer != null)
        {
            if (transparent)
            {
                // 투명하게 만들기
                Color color = originalColor;
                color.a = transparency;
                objectRenderer.material.color = color;
                // 머티리얼의 렌더 모드를 투명으로 설정
                objectRenderer.material.SetFloat("_Mode", 3);
                objectRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                objectRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                objectRenderer.material.SetInt("_ZWrite", 0);
                objectRenderer.material.DisableKeyword("_ALPHATEST_ON");
                objectRenderer.material.DisableKeyword("_ALPHABLEND_ON");
                objectRenderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                objectRenderer.material.renderQueue = 3000;
            }
            else
            {
                // 원래 상태로 복구
                objectRenderer.material.color = originalColor;
                // 머티리얼의 렌더 모드를 불투명으로 설정
                objectRenderer.material.SetFloat("_Mode", 0);
                objectRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                objectRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                objectRenderer.material.SetInt("_ZWrite", 1);
                objectRenderer.material.DisableKeyword("_ALPHATEST_ON");
                objectRenderer.material.DisableKeyword("_ALPHABLEND_ON");
                objectRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                objectRenderer.material.renderQueue = -1;
            }
        }
    }

    // 파티클 & 사운드 재생
    private void PlayEffectAndSound()
    {
        /*
        // 파티클 효과 재생
        ParticleSystem blockCreateEffect = Resources.Load<ParticleSystem>(""); // 여기에 경로 기입
        if (blockCreateEffect != null)
        {
            // 파티클 시스템을 인스턴스화해서 발판에 추가
            ParticleSystem particleEffect = Instantiate(blockCreateEffect, nextPlatform.transform.position, Quaternion.identity);
            particleEffect.Play(); // 파티클 재생
        }

        // 효과음 재생
        AudioClip blockCreateSound = Resources.Load<AudioClip>(""); // 여기에 경로 기입
        AudioSource audioSource = nextPlatform.GetComponent<AudioSource>();
        if (audioSource != null && blockCreateSound != null)
        {
            audioSource.PlayOneShot(blockCreateSound); // 효과음 재생
        }
        */
    }
}
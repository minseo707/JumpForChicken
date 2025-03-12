using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼 한정 플랫폼 상태 관리자
/// </summary>
public class TutorialPlatformStateManager : MonoBehaviour
{
    public int blockNumber = 0; // index

    private bool isPlayerOnPlatform = false;
    private float timeOnPlatform = 0f;
    public float requiredTimeOnPlatform = 0.1f; // 플레이어가 발판 위에 있어야 하는 시간 (초)
    private bool stateChanged = false; // 상태 변경 플래그 (1번만 실행)

    private int currentPlatformLayer;
    private int nextPlatformLayer;
    private int pastPlatformLayer;
    private int defaultPlatformLayer;

    private Rigidbody2D playerRigidbody;
    private Collider2D playerCollider;
    private Collider2D nextPlatformCollider;

    private Renderer objectRenderer;
    private Material cachedMaterial;
    private Color originalColor;

    private float transparency = 0.3f; // 투명도 비율 (0.0f = 완전 투명, 1.0f = 완전 불투명)

    private static readonly int _ModeID = Shader.PropertyToID("_Mode");
    private static readonly int _SrcBlendID = Shader.PropertyToID("_SrcBlend");
    private static readonly int _DstBlendID = Shader.PropertyToID("_DstBlend");
    private static readonly int _ZWriteID = Shader.PropertyToID("_ZWrite");


    private TutorialBlockStore tutorialBlockStore;

    // Start is called before the first frame update
    void Awake()
    {
        tutorialBlockStore = FindAnyObjectByType<TutorialBlockStore>();

        currentPlatformLayer = LayerMask.NameToLayer("CurrentPlatform");
        nextPlatformLayer = LayerMask.NameToLayer("NextPlatform");
        pastPlatformLayer = LayerMask.NameToLayer("PastPlatform");
        defaultPlatformLayer = LayerMask.NameToLayer("DefaultPlatform");

        if (tutorialBlockStore == null) Debug.LogError("[TutorialBlockStore] No TutorialBlockStore found");

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // 원래의 머티리얼 저장
            cachedMaterial = objectRenderer.material;
            originalColor = cachedMaterial.color;
        }

        // 초기 상태에 따라 레이어 설정
        SetPlatformLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어가 발판에 닿았을 때
        if (collision.collider.CompareTag("Player"))
        {
            playerCollider = collision.collider;
            playerRigidbody = playerCollider.attachedRigidbody;
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
                if (timeOnPlatform >= requiredTimeOnPlatform && gameObject.layer != currentPlatformLayer && !stateChanged)
                {
                    if (IsPlayerLanding())
                    {
                        // 최고높이에서는 Next가 존재하지 않으므로 탈출
                        if (blockNumber == tutorialBlockStore.blocks.Count - 1){
                            GameObject.Find("GameManager").GetComponent<TutorialGameManager>().TutorialClear();
                            stateChanged = true;
                            return;
                        }

                        // 상태 변경 플래그 설정
                        stateChanged = true;

                        // 다음 발판 설정
                        UpdateNextPlatform();
                        
                        if (stateChanged){
                            // 레이어 설정
                            tutorialBlockStore.ChangeBlockStateLayer(blockNumber);
                            ResetStateChanged();
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
            playerCollider = null; // Collider2D 참조 해제
        }
    }

    private bool IsPlayerLanding()
    {
        if (playerRigidbody == null)
            return false;

        // 플레이어의 X, Y축 속도가 거의 0일 때 착지로 간주 (발판에 딱 붙어서 못 올라가는 일부 경우에 작동하는 버그가 있어 일단 Y축 속도 감지값을 낮춤)
        return Mathf.Abs(playerRigidbody.velocity.y) <= 0.01f && Mathf.Abs(playerRigidbody.velocity.x) <= 0.1f;
    }

    public void SetPlatformLayer()
    {
        // 현재 상태에 따라 투명도 설정
        if (gameObject.layer == currentPlatformLayer){
            SetTransparency(false);
        } else if (gameObject.layer == nextPlatformLayer){
            SetTransparency(false);
        } else if (gameObject.layer == pastPlatformLayer){
            return;
        } else { // Default Layer
            SetTransparency(true);
        }
    }

    /// <summary>
    /// 현재 발판의 다음 발판을 찾아 상태를 Next로 변경
    /// </summary>
    private void UpdateNextPlatform()
    {
        TutorialPlatformStateManager newNextPlatform = null;

        // 지금 시점에는 밟은 블록 레이어가 Next => 다음 블록이 다음에 Next가 될 블록
        newNextPlatform = tutorialBlockStore.blocks[blockNumber + 1];
        nextPlatformCollider = newNextPlatform.gameObject.GetComponent<Collider2D>();

        // 만약 겹치는 부분이 있다면 Change 철회
        if (nextPlatformCollider.bounds.Intersects(playerCollider.bounds)){
            stateChanged = false;
            Debug.Log("[PlatformStateManager] 플레이어가 끼는 버그가 발생하였지만 해결됨");
        } 
    }

    // 상태 변경 플래그 초기화
    public void ResetStateChanged()
    {
        stateChanged = false;
    }

    // 투명도 조절
    public void SetTransparency(bool transparent)
    {
        if (objectRenderer == null || cachedMaterial == null) return;


        // 현재 상태 체크: 이미 원하는 투명도면 업데이트하지 않음
        if (transparent && Mathf.Approximately(cachedMaterial.color.a, transparency))
            return;
        if (!transparent && Mathf.Approximately(cachedMaterial.color.a, originalColor.a))
            return;

        if (transparent)
        {
            Color color = originalColor;
            color.a = transparency;
            cachedMaterial.color = color;

            // 투명 렌더링 모드 설정 (예: Standard Shader의 경우)
            cachedMaterial.SetFloat(_ModeID, 3);
            cachedMaterial.SetInt(_SrcBlendID, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            cachedMaterial.SetInt(_DstBlendID, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            cachedMaterial.SetInt(_ZWriteID, 0);
            cachedMaterial.DisableKeyword("_ALPHATEST_ON");
            cachedMaterial.DisableKeyword("_ALPHABLEND_ON");
            cachedMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            cachedMaterial.renderQueue = 3000;
        }
        else
        {
            cachedMaterial.color = originalColor;

            // 불투명 렌더링 모드 설정
            cachedMaterial.SetFloat(_ModeID, 0);
            cachedMaterial.SetInt(_SrcBlendID, (int)UnityEngine.Rendering.BlendMode.One);
            cachedMaterial.SetInt(_DstBlendID, (int)UnityEngine.Rendering.BlendMode.Zero);
            cachedMaterial.SetInt(_ZWriteID, 1);
            cachedMaterial.DisableKeyword("_ALPHATEST_ON");
            cachedMaterial.DisableKeyword("_ALPHABLEND_ON");
            cachedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            cachedMaterial.renderQueue = -1;
        }
    }
}

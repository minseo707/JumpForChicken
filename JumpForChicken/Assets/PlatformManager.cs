using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public enum PlatformState
    {
        Current, // 현재 발판
        Next,    // 다음 발판
        Past,    // 지나온 발판
        Default  // 기본 상태
    }

    public PlatformState state = PlatformState.Default;

    private static List<PlatformManager> allPlatforms = new List<PlatformManager>();
    private static PlatformManager nextPlatform;
    private bool isPlayerOnPlatform = false;
    private float timeOnPlatform = 0f;
    public float requiredTimeOnPlatform = 0.1f; // 플레이어가 발판 위에 있어야 하는 시간 (초)
    private bool stateChanged = false; // 상태 변경 플래그 (1번만 실행)
    private Rigidbody2D playerRigidbody;

    private void Start()
    {
        // 모든 발판을 리스트에 추가
        allPlatforms.Add(this);

        // 초기 상태에 따라 레이어 설정
        SetPlatformLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어가 발판에 닿았을 때
        if (collision.collider.CompareTag("Player"))
        {
            playerRigidbody = collision.collider.GetComponent<Rigidbody2D>();
            isPlayerOnPlatform = true;
            timeOnPlatform = 0f; // 착지 시간 초기화
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 플레이어가 발판 위에 머무를 때
        if (collision.collider.CompareTag("Player"))
        {
            if (playerRigidbody != null && isPlayerOnPlatform)
            {
                timeOnPlatform += Time.deltaTime;

                // 플레이어가 발판 위에 충분히 머무르면 상태를 변경
                if (timeOnPlatform >= requiredTimeOnPlatform && state == PlatformState.Next && !stateChanged)
                {
                    if (IsPlayerLanding())
                    {
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
                        }

                        // 다음 발판 설정
                        UpdateNextPlatform();

                        // 상태 변경 플래그 설정
                        stateChanged = true;
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

        // 플레이어의 Y축 속도가 0에 가까우면서 X축 속도가 거의 0일 때 착지로 간주
        return Mathf.Abs(playerRigidbody.velocity.y) <= 0.1f && Mathf.Abs(playerRigidbody.velocity.x) <= 0.1f;
    }

    private void SetPlatformLayer()
    {
        // 현재 상태에 따라 레이어 설정
        switch (state)
        {
            case PlatformState.Current:
                gameObject.layer = LayerMask.NameToLayer("CurrentPlatform");
                break;
            case PlatformState.Next:
                gameObject.layer = LayerMask.NameToLayer("NextPlatform");
                break;
            case PlatformState.Past:
                gameObject.layer = LayerMask.NameToLayer("PastPlatform");
                break;
            default:
                gameObject.layer = LayerMask.NameToLayer("DefaultPlatform"); // DefaultPlatform 레이어 설정
                break;
        }
    }

    private void UpdateNextPlatform()
    {
        // 현재 발판의 다음 발판을 찾아 상태를 Next로 변경
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10f); // 10f는 적절한 반경으로 설정 => 감지 범위로 인한 버그 가능성 있음 (확인 못함)
        PlatformManager newNextPlatform = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("DefaultPlatform"))
            {
                PlatformManager platformManager = hitCollider.GetComponent<PlatformManager>();
                if (platformManager != null)
                {
                    if (newNextPlatform == null)
                    {
                        newNextPlatform = platformManager;
                    }
                    else
                    {
                        // 가장 가까운 발판을 선택
                        float currentDistance = Vector2.Distance(transform.position, newNextPlatform.transform.position);
                        float newDistance = Vector2.Distance(transform.position, platformManager.transform.position);
                        if (newDistance < currentDistance)
                        {
                            newNextPlatform = platformManager;
                        }
                    }
                }
            }
        }

        // 가장 가까운 발판을 Next로 설정
        if (newNextPlatform != null)
        {
            nextPlatform = newNextPlatform;
            nextPlatform.state = PlatformState.Next;
            nextPlatform.SetPlatformLayer();
        }
    }

    // 모든 발판의 리스트를 초기화 (게임 시작 시 호출)
    public static void InitializePlatforms(List<PlatformManager> platforms)
    {
        allPlatforms = platforms;
        // 초기 상태 설정 로직 추가 가능
    }

    // 상태 변경 플래그 초기화
    public void ResetStateChanged()
    {
        stateChanged = false;
    }
}
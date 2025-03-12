using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 호랑이 초기 위치 설정 및 충돌 관리
/// </summary>
public class TigerManager : MonoBehaviour
{
    /// <summary>
    /// 딛고 있는 블록의 너비
    /// </summary>
    public float blockWidth = 3f;

    /// <summary>
    /// 호랑이의 콜라이더 너비
    /// </summary>
    private float colliderWidth = 1f;

    private const float safeDistance = 0.8f;

    private BoxCollider2D boxCollider;

    private SpriteRenderer tigerSpriteRenderer;

    void Awake()
    {
        tigerSpriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        // 랜덤 스케일
        if (Random.Range(0, 2) == 0){
            tigerSpriteRenderer.flipX = true;
        }
    } 

    /// <summary>
    /// 호랑이 생성 시 실행
    /// </summary>
    /// <param name="jumpDirection">점프 방향</param>
    public float TigerPositionChange(int jumpDirection, float x){
        float _x = 0;
        if (jumpDirection == 1){ // 왼쪽에서 오른쪽으로
            _x = Random.Range(x - blockWidth / 2f + safeDistance + colliderWidth / 2f,
            Mathf.Min(4.5f - colliderWidth / 2f, x + blockWidth / 2f - colliderWidth / 2f)); 
        } else if (jumpDirection == -1){
            _x = Random.Range(Mathf.Max(-4.5f + colliderWidth / 2f, x - blockWidth / 2f + colliderWidth / 2f),
            x + blockWidth / 2f - safeDistance - colliderWidth / 2f);
        }
        Debug.Log("[TigerManager] 호랑이의 위치를 수정합니다. " + _x);

        return _x;
    }

    public bool TryAvailable(float x){
        if (x - blockWidth / 2f + safeDistance > 4.5f || x + blockWidth / 2f - safeDistance < - 4.5f)
            return false;
        return true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            // 비활성화
            tigerSpriteRenderer.color = new Color(1, 1, 1, 0.3f);
            boxCollider.enabled = false;
            StartCoroutine(DisableTiger());
        }
    }

    private IEnumerator DisableTiger(){
        yield return new WaitForSeconds(8f);
        boxCollider.enabled = true;
        tigerSpriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

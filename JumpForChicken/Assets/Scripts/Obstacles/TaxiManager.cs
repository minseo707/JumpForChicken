using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 택시 소리 재생 및 이동 관리
/// </summary>
public class TaxiManager : MonoBehaviour
{
    /// <summary>
    /// 초당 움직임
    /// </summary>
    private float moveSpeed = 0.3f;

    /// <summary>
    /// 딛고 있는 블록의 너비
    /// </summary>
    public float BlockWidth = 3f;

    /// <summary>
    /// 택시의 콜라이더 크기
    /// </summary>
    private float colliderWidth;

    // 택시 부웅부붕하는 효과

    /// <summary>
    /// 이차함수의 계수
    /// </summary>
    private readonly float coeff = .7f;

    /// <summary>
    /// 부붕부붕 효과 진행 시간
    /// </summary>
    private float scaleTime = 0f;

    private GameObject spriteObject;
    private SpriteRenderer spriterRenderer;
    private BoxCollider2D boxCollider;

    void Start()
    {
        // 에디터 가변 작업
        colliderWidth = gameObject.GetComponent<BoxCollider2D>().size.x;
        spriteObject = transform.GetChild(0).gameObject;
        scaleTime = 0f;
        spriterRenderer = spriteObject.GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        // 오른쪽 바라보고 있을 떄
        if (transform.localScale.x > 0){
            transform.localPosition += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            if (transform.localPosition.x >= (BlockWidth / 2 - colliderWidth / 2)){ // 밖으로 나가려 하면 조정
                transform.localPosition = new Vector3(BlockWidth / 2 - colliderWidth / 2, 
                                                    transform.localPosition.y, 
                                                    transform.localPosition.z);
                // 반대로 회전
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        } else { // 왼쪽 바라보고 있을 때
            transform.localPosition += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
            if (transform.localPosition.x <= (-BlockWidth / 2 + colliderWidth / 2)){ // 밖으로 나가려 하면 조정
                transform.localPosition = new Vector3(-BlockWidth / 2 + colliderWidth / 2, 
                                                    transform.localPosition.y, 
                                                    transform.localPosition.z);
                // 반대로 회전
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    void Update()
    {
        scaleTime = (scaleTime + Time.deltaTime) % 1f;
        spriteObject.transform.localScale = new Vector3(
            spriteObject.transform.localScale.x,
            1 + coeff * scaleTime * (1 - scaleTime),
            spriteObject.transform.localScale.z
        );
        spriteObject.transform.localPosition = new Vector3(
            spriteObject.transform.localPosition.x,
            coeff * scaleTime * (1 - scaleTime) / 2,
            spriteObject.transform.localPosition.z
        );
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // 불투명
            spriterRenderer.color = new Color(1, 1, 1, 0.3f);
            boxCollider.enabled = false;
            StartCoroutine(DisableTaxi());
        }
    }

    private IEnumerator DisableTaxi(){
        yield return new WaitForSeconds(8f);
        boxCollider.enabled = true;
        spriterRenderer.color = new Color(1, 1, 1, 1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenInWall : MonoBehaviour
{

    private Collider2D chickenCol;     // 치킨 콜라이더
    private Rigidbody2D rb;            // 치킨 리지드바디(있다면)
    private Collider2D[] overlapHits;  // Overlap 결과 배열

    private GameObject chickenSprite;
    private float randomStartTime;

    private void Awake()
    {
        chickenCol = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        overlapHits = new Collider2D[5];
    }

    void Start(){
        chickenSprite = transform.GetChild(1).gameObject;
        randomStartTime = Random.Range(0f, 360f);
    }

    private void FixedUpdate()
    {
        // 1. OverlapCollider로 치킨 콜라이더와 겹치는 모든 콜라이더를 찾는다.
        ContactFilter2D filter = new()
        {
            useTriggers = false,
        };
        filter.SetLayerMask(Physics2D.AllLayers);

        int count = Physics2D.OverlapCollider(chickenCol, filter, overlapHits);
        if (count == 0) return; // 겹치는 콜라이더가 전혀 없으면 끼임 X

        // 2. 겹치는 콜라이더들 중, 태그가 Block인 것만 검사
        for (int i = 0; i < count; i++)
        {
            Collider2D blockCol = overlapHits[i];
            if (blockCol == null) continue;

            if (!blockCol.CompareTag("Block"))
                continue;

            // 3. 실제 겹침 거리 계산 (Distance)
            ColliderDistance2D distInfo = chickenCol.Distance(blockCol);
            
            // distInfo.isOverlapped == true 이면 실제로 겹친 상태
            if (distInfo.isOverlapped)
            {
                // 겹침 해결을 위한 이동 벡터 계산
                Vector2 separationVector = distInfo.normal * distInfo.distance;

                // 치킨의 위치를 조정하여 블록에서 밀어냄
                transform.position = (Vector2)transform.position + separationVector;
            }
        }

        // 4. 배열 정리
        for (int i = 0; i < count; i++)
        {
            overlapHits[i] = null;
        }
    }

    void Update(){
        chickenSprite.transform.localPosition = new Vector3(0, 0.07f * (Mathf.Sin(Time.time * 3f + randomStartTime) + 1), 0);
    }
}
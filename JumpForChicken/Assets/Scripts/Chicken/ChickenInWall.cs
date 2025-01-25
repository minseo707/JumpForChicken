using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenInWall : MonoBehaviour
{

    private Collider2D chickenCol;     // 치킨 콜라이더
    private Rigidbody2D rb;            // 치킨 리지드바디(있다면)
    private Collider2D[] overlapHits;  // Overlap 결과 배열

    private void Awake()
    {
        chickenCol = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        overlapHits = new Collider2D[5];
    }

    private void FixedUpdate()
    {
        // 1. OverlapCollider로 치킨 콜라이더와 겹치는 모든 콜라이더를 찾는다.
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;  
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
                float overlapDistance = -distInfo.distance;

                // 탈출
                transform.position += new Vector3(0, overlapDistance, 0);

                // 리지드바디 속도 보정 (떨림 방지)
                if (rb != null)
                {
                    Vector2 vel = rb.velocity;
                    // 아래로 파고드는 중이라면 y속도 0으로
                    if (vel.y < 0) vel.y = 0;
                    rb.velocity = vel;
                }
            }
        }

        // 4. 배열 정리
        for (int i = 0; i < count; i++)
        {
            overlapHits[i] = null;
        }
    }
}
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
        rb         = GetComponent<Rigidbody2D>();

        overlapHits = new Collider2D[5];
    }

    private void FixedUpdate()
    {
        // 1) OverlapCollider로 "치킨 콜라이더와 겹치는" 모든 콜라이더를 찾는다.
        //    (여기서는 레이어 마스크를 쓰지 않고, 태그로 거르기 때문에 전체 레이어를 검색)
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false;  
        // 모든 레이어를 검색하고 싶다면 아래처럼 설정 (Unity 버전에 따라 다를 수 있음)
        filter.SetLayerMask(Physics2D.AllLayers);

        int count = Physics2D.OverlapCollider(chickenCol, filter, overlapHits);
        if (count == 0) return; // 겹치는 콜라이더가 전혀 없으면 끼임 X

        // 2) 겹치는 콜라이더들 중, 태그가 "Block"인 것만 검사
        for (int i = 0; i < count; i++)
        {
            Collider2D blockCol = overlapHits[i];
            if (blockCol == null) continue;

            // 태그가 "Block"인지 확인
            if (!blockCol.CompareTag("Block"))
                continue;

            // 3) 실제 겹침 거리 계산 (Distance)
            ColliderDistance2D distInfo = chickenCol.Distance(blockCol);
            
            // distInfo.isOverlapped == true 이면 실제로 겹친 상태
            if (distInfo.isOverlapped)
            {
                // distance가 음수면 오버랩, -0.3f라면 0.3만큼 겹침
                float overlapDistance = -distInfo.distance;  // 양수로 전환
                Vector2 pushDir = distInfo.normal;           // block -> chicken 방향

                // 치킨을 오버랩 거리만큼 이동 (정반대 방향으로)
                transform.position += (Vector3)(pushDir * overlapDistance);

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

        // 4) 배열 정리(선택 사항)
        for (int i = 0; i < count; i++)
        {
            overlapHits[i] = null;
        }
    }
}
using UnityEngine;
using UnityEngine.Tilemaps;

public class OneWayPlatform : MonoBehaviour
{

    private GameObject player; 
    
    private Collider2D[] platformColliders;
    private Collider2D[] playerColliders;  

    // 메인 플랫폼 콜라이더(판정을 위한 기준)
    private Collider2D mainPlatformCollider;
    // 메인 플레이어 콜라이더(판정을 위한 기준)
    private Collider2D mainPlayerCollider;

    private bool currentActive = true;


    // 플레이어가 플랫폼 상단보다 아래에 있다고 판단할 때 줄 오프셋
    private float offset = -3f;

    private void Awake()
    {
        platformColliders = GetComponents<Collider2D>();
        mainPlatformCollider = platformColliders[0];
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerColliders = player.GetComponents<Collider2D>();
        mainPlayerCollider = playerColliders[0];
    }

    private void FixedUpdate()
    {
        // 판정을 위해 플레이어 하단 Y, 플랫폼 상단 Y를 가져옴
        float playerBottomY = mainPlayerCollider.bounds.min.y;
        float platformTopY  = mainPlatformCollider.bounds.max.y;

        // 플레이어가 플랫폼보다 위에 있다면
        if (playerBottomY - platformTopY > offset){
            if (!currentActive) {
                CollisionIgnores(false); // 정상
                currentActive = true;
            }
        } else {
            if (currentActive) {
                CollisionIgnores(true); // 무시
                currentActive = false;
            }
        }
    }

    private void CollisionIgnores(bool ignore){
        foreach (var pCol in platformColliders)
        {
            if (pCol == null) continue;
            foreach (var plCol in playerColliders)
            {
                if (plCol == null) continue;
                // 모든 쌍에 대해 충돌 무시 적용
                Physics2D.IgnoreCollision(plCol, pCol, ignore);
            }
        }
    }
}
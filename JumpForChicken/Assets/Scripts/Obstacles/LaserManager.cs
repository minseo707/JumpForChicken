using System.Collections;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        StartCoroutine(SurviveForSeconds(3.0f));
    }

    void OnTriggerStay2D(Collider2D other){
        // 이 부분을 넣을지 말지는 추후 결정
        if (other.gameObject.CompareTag("Player")){
            // 투명도 조정
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            col.enabled = false;
        }
    }

    private IEnumerator SurviveForSeconds(float seconds){
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}

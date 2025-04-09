using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AirplaneManager : MonoBehaviour
{
    [SerializeField] private float waitTime = 3f;

    private const float moveSpeed = 2f; // Move Speed per Second

    private const float colliderWidth = 1f;

    private const float outOffset = 0.25f;

    private BoxCollider2D boxCollider;

    private SpriteRenderer airplaneSpriteRenderer;

    private int direction = 1; // 1: right, -1: left

    private bool wait = false;

    private TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        direction = 2 * Random.Range(0, 2) - 1; // 1 or -1
        wait = false;
        ScaleChange();

        boxCollider = GetComponent<BoxCollider2D>();
        airplaneSpriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = transform.Find("Trail").GetComponent<TrailRenderer>();
    }

    void FixedUpdate()
    {
        if (!IsPlayerInside() && !wait)
        {
            wait = true;
            StartCoroutine(ChangeDirection());
        }

        if (IsPlayerInside() || Mathf.Sign(transform.position.x) != direction){
            transform.Translate(direction * moveSpeed * Time.fixedDeltaTime * Vector2.right);
            if (IsPlayerInside()) wait = false;
        }
    }

    private IEnumerator ChangeDirection(){
        yield return new WaitForSeconds(waitTime);
        direction *= -1;
        ScaleChange();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            // 비활성화
            airplaneSpriteRenderer.color = new Color(1, 1, 1, 0.3f);

            // Trail의 투명도 조정
            Gradient gradient = new();
            gradient.SetKeys(
                new GradientColorKey[] { new(Color.white, 0.0f), new(Color.white, 1.0f) },
                new GradientAlphaKey[] { new(0.3f, 0.0f), new(0.3f, 1.0f) }
            );
            trailRenderer.colorGradient = gradient;
            boxCollider.enabled = false;
            StartCoroutine(DisableAirplane());
        }
    }

    // 비행기 충돌 후 비활성화
    private IEnumerator DisableAirplane(){
        yield return new WaitForSeconds(8f);
        boxCollider.enabled = true;
        airplaneSpriteRenderer.color = new Color(1, 1, 1, 1);
        // Trail의 투명도 조정
        Gradient gradient = new();
        gradient.SetKeys(
            new GradientColorKey[] { new(Color.white, 0.0f), new(Color.white, 1.0f) },
            new GradientAlphaKey[] { new(1f, 0.0f), new(1f, 1.0f) }
        );
        trailRenderer.colorGradient = gradient;
    }

    private bool IsPlayerInside(){
        if (direction == 1){
            return transform.position.x - colliderWidth / 2f - outOffset <= 4.5f;
        } else {
            return transform.position.x + colliderWidth / 2f + outOffset >= -4.5f;
        }
    }

    private void ScaleChange(){
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, transform.localScale.y, transform.localScale.z);
    }
}

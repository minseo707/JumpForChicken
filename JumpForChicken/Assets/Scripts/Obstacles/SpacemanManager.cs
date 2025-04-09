using System.Collections;
using UnityEngine;

public class SpacemanManager : MonoBehaviour
{
    private CameraController cc;

    private Vector2 moveVector;

    private Vector2 localPos;

    private SpriteRenderer spriteRenderer;

    private BoxCollider2D boxCollider;

    private float startTime = 3f;

    private float startYPos = 0f;

    [Header("Spaceman Config")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Vector2 colliderSize = new(0.4f, 1f);
    [SerializeField] private float defalutRotationSpeed = 30f;

    private float currentRotationSpeed = 0f;


    void Awake(){
        cc = Camera.main.GetComponent<CameraController>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = gameObject.GetComponent<BoxCollider2D>();

        float j = -Random.Range(0.9f, 0.95f);
        float i = Mathf.Sqrt(1 - Mathf.Pow(j, 2)) * Mathf.Sign(Random.Range(-1f, 1f));
        
        currentRotationSpeed = defalutRotationSpeed;

        startYPos = transform.transform.position.y;

        moveVector = new(i, j);
        moveVector.Normalize();

        localPos += startYPos * Vector2.up;
    }

    void Update(){
        if (startTime > 0){
            localPos += moveSpeed * Time.deltaTime * moveVector;
            startTime = Mathf.Max(startTime - Time.deltaTime, 0f);

            if (localPos.x <= 4.5f - colliderSize.x / 2 && localPos.x >= -4.5f + colliderSize.x / 2 &&
                localPos.y <= 8f - colliderSize.y / 2 && localPos.y >= -8f + colliderSize.y / 2
            ){
                startTime = 0f;
            }
        } else {
            localPos += moveSpeed * Time.deltaTime * moveVector;
            // (i) Right Wall
            if (localPos.x > 4.5f - colliderSize.x / 2){
                localPos.x = 4.5f - colliderSize.x / 2;
                moveVector.x *= -1 * Random.Range(0.2f, 1.6f);
                moveVector.Normalize();
            }
            // (ii) Left Wall
            else if (localPos.x < -4.5f + colliderSize.x / 2){
                localPos.x = -4.5f + colliderSize.x / 2;
                moveVector.x *= -1 * Random.Range(0.2f, 1.6f);
                moveVector.Normalize();
            }
            // (iii) Top Wall
            else if (localPos.y > 8f - colliderSize.y / 2){
                localPos.y = 8f - colliderSize.y / 2;
                moveVector.y *= -1 * Random.Range(0.2f, 1.6f);
                moveVector.Normalize();
            }
            // (iv) Bottom Wall
            else if (localPos.y < -8f + colliderSize.y / 2){
                localPos.y = -8f + colliderSize.y / 2;
                moveVector.y *= -1 * Random.Range(0.2f, 1.6f);
                moveVector.Normalize();
            }
        }

        currentRotationSpeed = Mathf.Max(defalutRotationSpeed, currentRotationSpeed - 60f * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, (transform.rotation.eulerAngles.z + currentRotationSpeed * Time.deltaTime) % 360f);

        if (!cc) cc = Camera.main.GetComponent<CameraController>();
        if (!cc){
            transform.position = localPos;
        } else {
            transform.position = localPos + Vector2.up * cc.cameraHeight;
        }
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            // 불투명
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            boxCollider.enabled = false;
            currentRotationSpeed += 400f;
            StartCoroutine(DisableSpaceman());
        }
    }

    private IEnumerator DisableSpaceman(){
        yield return new WaitForSeconds(8f);
        boxCollider.enabled = true;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}

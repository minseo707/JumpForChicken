using UnityEngine;

public class SpacemanManager : MonoBehaviour
{
    private CameraController cc;

    private Vector2 moveVector;

    private Vector2 localPos;

    private float startTime = 2f;

    [Header("Spaceman Config")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Vector2 colliderSize = new(0.4f, 1f);


    void Awake(){
        cc = Camera.main.GetComponent<CameraController>();

        float j = Random.Range(0.5f, 1f);
        float i = Mathf.Sqrt(1 - Mathf.Pow(j, 2)) * Mathf.Sign(Random.Range(-1f, 1f));

        moveVector = new(i, j);
        moveVector.Normalize();
    }

    void Update(){
        if (startTime > 0){
            localPos += moveSpeed * Time.deltaTime * moveVector;
            startTime = Mathf.Max(startTime - Time.deltaTime, 0f);
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

        if (!cc) cc = Camera.main.GetComponent<CameraController>();
        if (!cc){
            transform.position = localPos;
        } else {
            transform.position = localPos + Vector2.up * cc.cameraHeight;
        }
        
    }
}

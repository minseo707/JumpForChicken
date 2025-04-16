using System.Collections;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
    [Header("Sound Config")]
    [SerializeField] private float soundDistance = 8f;

    [SerializeField] private float offset = -2f;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private GameObject mainCamera;

    private CameraController cc;

    private AudioSource audioSource;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        StartCoroutine(SurviveForSeconds(3.0f));

        mainCamera = Camera.main.gameObject;
        cc = mainCamera.GetComponent<CameraController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!mainCamera) mainCamera = Camera.main.gameObject;
        if (!cc) cc = mainCamera.GetComponent<CameraController>();

        audioSource.volume = Mathf.Abs(transform.position.y - cc.cameraHeight - offset) <= soundDistance?
                                - 1f / Mathf.Pow(soundDistance, 2) * Mathf.Pow(transform.position.y - cc.cameraHeight - offset, 2) + 1f : 0f;
        audioSource.volume *= Time.timeScale;
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

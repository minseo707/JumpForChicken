using UnityEngine;

public class UfoManager : MonoBehaviour
{
    private readonly float cycleTime = 6f;
    private float runTime = 0f;
    private float ySize = 0.4f;

    private GameObject mainCamera;

    private CameraController cc;

    [Header("Laser Prefab")]
    [SerializeField] private GameObject laserPrefab;

    void Start()
    {
        runTime = Random.Range(2f, 5f);
        mainCamera = Camera.main.gameObject;
        cc = mainCamera.GetComponent<CameraController>();
    }

    void Update(){
        runTime += Time.deltaTime;
        if (runTime >= cycleTime){
            // UFO의 자식으로 레이저 인스턴스화
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.transform.parent = transform;

            laser.transform.localPosition = new Vector3(0, -3f, 0); // Laser 위치를 수정
            runTime = 0f;
        }
    }

    void FixedUpdate()
    {
        if (!mainCamera) mainCamera = Camera.main.gameObject;
        if (!cc) cc = mainCamera.GetComponent<CameraController>();
        if (transform.position.y - cc.cameraHeight < 17f + ySize / 2f && transform.position.y - cc.cameraHeight > 8f - ySize / 2f){
            cc.ViewWarning(transform.position.x);
        }
    }
}

using UnityEngine;

public class UfoManager : MonoBehaviour
{
    private readonly float cycleTime = 6f;
    private float runTime = 0f;

    [Header("Laser Prefab")]
    [SerializeField] private GameObject laserPrefab;

    void Start()
    {
        runTime = Random.Range(2f, 5f);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 1f;
    public float floor = -2.231772f;

    private float startCamera;
    private float cameraSpeed = 0.1f; // 초기 속도
    private float totalDiff;
    private bool trig;

    private Animator playerAnim;

    void Start()
    {
        // 초기 카메라 위치 설정
        transform.position = new Vector3(0, cameraHeight, -10);
        startCamera = cameraHeight - floor;
        playerAnim = player.GetComponent<Animator>();
        totalDiff = 0f;
        trig = false;
    }

    void Update()
    {
        // 카메라 높이 조정
        if (playerAnim.GetBool("onGround") && cameraHeight - player.transform.position.y < startCamera && !trig)
        {
            totalDiff += player.transform.position.y + startCamera - cameraHeight;
            trig = true;
        }

        if (!playerAnim.GetBool("onGround"))
        {
            trig = false;
        }

        cameraHeight += cameraSpeed * Time.deltaTime;

        if (totalDiff > 1e-4)
        {
            cameraHeight += totalDiff / 16;
            totalDiff -= totalDiff / 16;
        }
        else
        {
            cameraHeight += totalDiff;
            totalDiff = 0;
        }

        // 카메라 위치 업데이트
        transform.position = new Vector3(0, cameraHeight, -10);
    }
}

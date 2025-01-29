using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 1f;
    public float floor = -2.231772f;

    public float secPerTile = 10f;


    // 데드라인 실수 보강

    public float backwards = 4f;

    private float difference = 0f;
    private float tempDiff = 0f;

    private float startCamera;
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
        difference = 0f;
        tempDiff = 0f;
        trig = false;
    }

    void FixedUpdate()
    {
        // 카메라 높이 조정 (하강 과정)
        if (player.transform.position.y < cameraHeight - 7f){
            totalDiff = 0;
            tempDiff = cameraHeight - player.transform.position.y - 7f;
            if (tempDiff + difference >= backwards){
                tempDiff = backwards - difference;
            }
            difference += tempDiff;
            cameraHeight -= tempDiff;
            tempDiff = 0f;
        }
        
        // 카메라 높이 조정 (상승 과정)
        if (playerAnim.GetBool("onGround") && cameraHeight - player.transform.position.y < startCamera && !trig)
        {
            totalDiff += player.transform.position.y + startCamera - cameraHeight;
            trig = true;
        }

        if (!playerAnim.GetBool("onGround"))
        {
            trig = false;
        }

        cameraHeight += Time.deltaTime / secPerTile;

        if (totalDiff > 1e-4)
        {
            cameraHeight += totalDiff / 16;
            difference = Mathf.Max(0, difference - totalDiff / 64);
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

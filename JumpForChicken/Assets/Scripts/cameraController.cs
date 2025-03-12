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

    public float maxHeight = 0f;

    private float startCamera;
    private float totalDiff;
    private bool trig;
    private PlayerAnimationManager pam;

    private GameManager gm;

    void Start()
    {
        // 초기 카메라 위치 설정
        transform.position = new Vector3(0, cameraHeight, -10);
        startCamera = cameraHeight - floor;
        pam = player.GetComponent<PlayerAnimationManager>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        totalDiff = 0f;
        difference = 0f;
        tempDiff = 0f;
        trig = false;
    }

    void FixedUpdate()
    {
        if (gm.stage == 1){
            maxHeight = 120.333f;
        } else if (gm.stage == 2){
            maxHeight = 300f;
        }

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
        if (pam.onGround && cameraHeight - player.transform.position.y < startCamera && !trig)
        {
            totalDiff = Mathf.Min(player.transform.position.y + startCamera - cameraHeight, maxHeight - cameraHeight);
            trig = true;
        }

        if (!pam.onGround)
        {
            trig = false;
        }

        cameraHeight += Time.deltaTime / secPerTile;

        if (totalDiff > 1e-4)
        {
            cameraHeight = Mathf.Min(cameraHeight + totalDiff / 16, maxHeight);
            difference = Mathf.Max(0, difference - totalDiff / 64);
            totalDiff -= totalDiff / 16;
        }
        else
        {
            cameraHeight = Mathf.Min(cameraHeight + totalDiff / 16, maxHeight);
            totalDiff = 0;
        }

        // 카메라 위치 업데이트
        transform.position = new Vector3(0, cameraHeight, -10);
    }

    public void ChangeHeight(float height){
        cameraHeight = height;
        trig = false;
        totalDiff = 0;
        difference = 0;
        tempDiff = 0;

        transform.position = new Vector3(0, cameraHeight, -10);
    }
}

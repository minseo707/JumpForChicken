using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class cameraController : MonoBehaviour
{
    public GameObject player;
    public float cameraHeight = 1.5f;
    public float floor = -2.231772f;

    float startCamera;
    float cameraSpeed = 0.1f; // initial Value
    float totalDiff;
    bool trig;

    Animator playerAnim;

    void Start() {
        this.transform.position = new Vector3(0, cameraHeight, -10);
        startCamera = cameraHeight - floor;
        playerAnim = player.GetComponent<Animator>();
        totalDiff = 0f;
        trig = false;
    }

    void Update()
    {
        if (playerAnim.GetBool("onGround") && cameraHeight - player.transform.position.y < startCamera && !trig){
            totalDiff += player.transform.position.y + startCamera - cameraHeight;
            trig = true;
        }
        if (!playerAnim.GetBool("onGround")) trig = false;
        cameraHeight += cameraSpeed * Time.deltaTime;
        if (totalDiff > 1e-4){
            cameraHeight += totalDiff / 16;
            totalDiff -= totalDiff / 16;
        } else {
            cameraHeight += totalDiff;
            totalDiff = 0;
        }
        this.transform.position = new Vector3(0, cameraHeight, -10);
    }
}
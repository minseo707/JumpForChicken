using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼에서의 카메라 움직임 (Follow Player)
/// </summary>
public class TutorialCameraController : MonoBehaviour
{
    private GameObject player;
    public float cameraHeight = 1f;

    public float floor = -2.231772f;

    void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y - floor, transform.position.z);
        cameraHeight = player.transform.position.y - floor;
    }
}

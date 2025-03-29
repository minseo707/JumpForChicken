using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼엥서의 배경 높이 (시차) 관리자
/// </summary>
public class TutorialBackgroundManager : MonoBehaviour
{
    private float offset = 13f;

    private GameObject cameras;

    [Header("Offsets")]
    public float realHeight = 20f;

    void Start()
    {
        cameras = Camera.main.gameObject;
    }

    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            (realHeight - 24f)/realHeight * (cameras.transform.position.y - 1) + offset,
            transform.position.z
        );
    }
}

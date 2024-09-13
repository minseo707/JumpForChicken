using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // 카메라 좌표를 불러오기 위함
    public GameObject cameras;

    public float scope = 13f;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameras.transform.position.x, 4*(cameras.transform.position.y-1)/5 + 13, transform.position.z);
    }
}
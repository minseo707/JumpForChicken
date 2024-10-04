using System.Collections;
using UnityEngine;

public class ObjectRIse : MonoBehaviour
{
    public float riseHeight = 5f; // 오브젝트가 이동할 높이

    float leftY = 0;

    private Vector3 startPos;
    private Vector3 endPos;

    void OnEnable()
    {
        startPos = transform.localPosition - new Vector3(0, riseHeight, 0);
        transform.localPosition = startPos;
        endPos = startPos + new Vector3(0, riseHeight, 0); // Y축으로 올라가는 방향
        leftY = riseHeight;
    }

    private void Update() {
        if (leftY > 0.01){
            transform.localPosition += new Vector3(0, leftY / 15, 0);
            leftY -= leftY/15;
        } else {
            leftY = 0;
            transform.localPosition = endPos;
        }
        // 최종 로컬 위치로 설정
    }
}
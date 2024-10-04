using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLoadManager : MonoBehaviour
{
    public float deactivateDistance = 20.0f;  // 비활성화할 기준 거리
    private GameObject playerCamera;  // 카메라
    private bool isActive = true;  // 현재 활성화 상태 추적
    public float checkInterval = 0.75f;  // 체크할 간격 (초)

    void Start()
    {
        playerCamera = Camera.main.gameObject;  // 메인 카메라를 찾음
        InvokeRepeating("CheckDistance", 0f, checkInterval);  // 주기적으로 거리 체크
    }

    void CheckDistance()
    {
        float distance = Mathf.Abs(playerCamera.transform.position.y - transform.position.y);  // 카메라와 오브젝트 사이 거리 계산

        if (distance > deactivateDistance && isActive)  // 거리가 기준을 넘고, 현재 활성화 상태일 때
        {
            gameObject.SetActive(false);  // 비활성화
            isActive = false;
        }
        else if (distance <= deactivateDistance && !isActive)  // 거리가 기준보다 작아지고, 현재 비활성화 상태일 때
        {
            gameObject.SetActive(true);  // 다시 활성화
            isActive = true;
        }
    }
}

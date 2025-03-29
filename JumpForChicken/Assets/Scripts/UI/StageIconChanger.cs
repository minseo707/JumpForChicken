using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIconChanger : MonoBehaviour
{
    private List<GameObject> iconList;

    void Start()
    {
        iconList = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            iconList.Add(transform.GetChild(i).gameObject);
        }
        ChangeIcon();
    }

    void OnEnable()
    {   
        ChangeIcon();
    }

    private void ChangeIcon(){
        if (iconList == null) return;
        for (int i = 0; i < iconList.Count; i++)
        {
            // 자신 빼고 비활성화
            if (i != GameManager.stage - 1) iconList[i].SetActive(false);
            else iconList[i].SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    static List<GameObject> tailList = new();

    void Awake()
    {
        tailList = new();
        for (int i = 0; i < DataManager.Instance.gameData.hasMotorcycle.Length; i++)
        {
            tailList.Add(transform.GetChild(i).gameObject);
        }

        tailList[DataManager.Instance.gameData.equippedGoods[2]].SetActive(true);
    }

    public static void ChangeMotorcycleTail(int motorcycleCode){
        for (int i = 0; i < tailList.Count; i++)
        {
            tailList[i].SetActive(false);
        }
        tailList[motorcycleCode].SetActive(true);
    }

}

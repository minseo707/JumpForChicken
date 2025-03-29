using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    private int currentPage = 1;

    [Header("Tutorial Pages")]
    public GameObject tutorialPage2;
    public GameObject tutorialPage3;
    public GameObject tutorialPage4;
    public GameObject tutorialPage5;

    public GameObject nextPageButton;
    
    void Start()
    {
        Time.timeScale = 0f; // 시간 정지
        currentPage = 1;
    }

    public void NextPage(){
        switch (++currentPage){
            case 2:
                tutorialPage2.SetActive(true);
                break;
            case 3:
                tutorialPage3.SetActive(true);
                break;
            case 4:
                tutorialPage4.SetActive(true);
                break;
            case 5:
                tutorialPage5.SetActive(true);
                Destroy(nextPageButton);
                break;
            default:
                Debug.LogWarning("[TutorialUIManage] Out of tutorial page");
                break;
        }
    }

    public void OnClickStartButton(){
        Time.timeScale = 1f; // 시간 다시 재개
        Destroy(gameObject);
    }
}

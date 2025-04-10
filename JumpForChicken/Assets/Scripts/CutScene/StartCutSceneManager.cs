using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class StartCutSceneManager : MonoBehaviour
{
    [Header("CutScene Config")]
    [SerializeField] private GameObject[] pages;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float pageDuration = 3f;

    private Image[] images;

    private int currentPage = 0;

    private float pageTimer = 0f;
    private float fadeTimer = 0f;


    public void NextPage(){
        fadeTimer = fadeDuration;
        pageTimer = 0f;
        currentPage++;
        pages[currentPage].SetActive(true);
        if (currentPage < pages.Length - 1){
            images[currentPage].color = new Color(1, 1, 1, 0);
        } else {
            images[currentPage].color = new Color(0, 0, 0, 0);
        }
    }

    void Start(){
        currentPage = 0;
        images = new Image[pages.Length];
        for (int i = 0; i < pages.Length; i++)
        {
            images[i] = pages[i].transform.GetChild(0).GetComponent<Image>();
        }

        Debug.Log($"{pages.Length}개의 페이지를 불러왔습니다.");

        NextPage();
    }

    void Update(){
        if (fadeTimer > 0f){
            fadeTimer -= Time.deltaTime;
            float fadeAlpha = 1f - fadeTimer / fadeDuration;
            if (currentPage < pages.Length - 1){
                images[currentPage].color = new Color(1, 1, 1, fadeAlpha);
            } else {
                images[currentPage].color = new Color(0, 0, 0, fadeAlpha);
            }
            
        }

        if (fadeTimer <= 0f && pageTimer == 0f){
            fadeTimer = 0f;
            pageTimer = pageDuration;
            if (currentPage == pages.Length - 1){
                LoadingSceneManager.LoadScene("SampleScene");
            }
        }

        if (pageTimer > 0f){
            pageTimer -= Time.deltaTime;
            if (pageTimer <= 0f){
                Debug.Log(currentPage);
                NextPage();
            }
        }
    }
}

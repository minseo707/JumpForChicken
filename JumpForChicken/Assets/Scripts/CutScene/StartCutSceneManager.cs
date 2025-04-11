using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시작 컷씬 재생 관리자
/// </summary>
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

    private bool skip = false;


    public void NextPage(){
        if (currentPage >= pages.Length - 1) return;
        if (fadeTimer > 0f) skip = true; // Skip 로직이 한 번만 실행되도록 트리거
        if (skip) return;

        currentPage++;
        fadeTimer = fadeDuration;
        pageTimer = 0f;

        // 다음 페이지 활성화
        pages[currentPage].SetActive(true);
        if (currentPage < pages.Length - 1){
            images[currentPage].color = new Color(1, 1, 1, 0);
        } else { /* 마지막 페이지는 검은 색으로 설정 */
            images[currentPage].color = new Color(0, 0, 0, 0);
        }
    }

    void Start(){
        currentPage = 0;
        skip = false;
        images = new Image[pages.Length];

        for (int i = 0; i < pages.Length; i++)
        {
            images[i] = pages[i].transform.GetChild(0).GetComponent<Image>();
        }

        Debug.Log($"{pages.Length}개의 페이지를 불러왔습니다.");

        // 첫 번째 페이지 활성화
        NextPage();
    }

    void Update(){
        if (fadeTimer > 0f){
            fadeTimer -= Time.deltaTime;
            float fadeAlpha = 1f - fadeTimer / fadeDuration;
            if (currentPage < pages.Length - 1){
                images[currentPage].color = new Color(1, 1, 1, fadeAlpha);
            } else { /* 마지막 페이지는 검은 색으로 설정 */
                images[currentPage].color = new Color(0, 0, 0, fadeAlpha);
            }
            
        }

        if (fadeTimer <= 0f && pageTimer == 0f){
            fadeTimer = 0f;

            if (!skip) pageTimer = pageDuration;
            else pageTimer = 0.0005f;
            
            if (currentPage == pages.Length - 1){
                // 마지막 페이지 Fade Out하고 나서 LoadingScene -> SampleScene으로 이동
                LoadingSceneManager.LoadScene("SampleScene");
            }
        }

        if (pageTimer > 0f){
            pageTimer -= Time.deltaTime;
            if (pageTimer <= 0f){
                skip = false;
                NextPage();
            }
        }
    }
}

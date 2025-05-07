using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class EndingCutSceneManager : MonoBehaviour
{

    [Header("CutScene Config")]
    [Description("All of the pages in the cutscene.")]
    [SerializeField] private GameObject[] pages;
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private float pageDuration = 3f;
    [SerializeField] private float fadeoutDuration = 1f;

    [Header("Gameover UI Connection")]
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameoverUIManager gameoverUIManager;

    [Header("Skip Button")]
    [SerializeField] private GameObject skipButton;

    private Image[][] images;

    private int currentPage = 0;

    private float waitTimer;
    private float fadeTimer;
    private float pageTimer;

    private int activePage = 0;

    private int part;
    private bool skip;
    private bool allSkip;


    /// <summary>
    /// 컷씬 스킵 (스크립트 내에서 재사용 가능)
    /// </summary>
    public void SkipCutScene(){
        if (part == 1){
            skip = true;
        }

        if (part == 2){
            if (currentPage >= 3){
                // 엔딩 컷씬 종료
                darkPanel.SetActive(false); // 미리 검은 색 불투명 배경 제거

                // TODO: Gameover UI 활성화
                gameoverUI.SetActive(true);

                fadeTimer = fadeoutDuration;
                pageTimer = 0f; // Goto (3)
            } else {
                // 다음 페이지 활성화
                currentPage++;
                skip = false;
                fadeTimer = fadeoutDuration;
                pageTimer = 0f;
                waitTimer = 0f; // Goto (1)
            }
            
        }
    }

    public void AllSkip(){
        if (allSkip) return;
        currentPage = pages.Length - 1;
        waitTimer = -1f;
        fadeTimer = 0f;
        pageTimer = pageDuration;
        skip = true;
        allSkip = true;
        skipButton.SetActive(false);
    }

    void Start()
    {
        images = new Image[pages.Length][];
        for (int i = 0; i < pages.Length; i++){
            images[i] = new Image[4];
        }
        
        skip = false;
        allSkip = false;
        currentPage = 0;

        for (int i = 0; i < pages.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                images[i][j] = pages[i].transform.GetChild(j).GetComponent<Image>();
            }
        }
    }

    void OnEnable()
    {
        part = 0;
        waitTimer = waitDuration;
        // GameManager에서 데이터 가져오기 (DataManager에서 데이터 저장도 함께 구현)
        activePage = GameManager.GetEndingPage();
    }

    void Update()
    {
        if (waitTimer > 0f){
            part = 0;
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f){
                pages[activePage].SetActive(true);
                skipButton.SetActive(true);
                for (int i = 0; i < 4; i++)
                {
                    images[activePage][i].color = new Color(1, 1, 1, 0); 
                }
                waitTimer = 0f;
                fadeTimer = fadeoutDuration; // Goto (1)
            }
        }

        if (pageTimer == 0f && fadeTimer > 0f && waitTimer != -1f){ // (1)
            part = 1;
            // 페이지 활성화
            fadeTimer -= Time.fixedDeltaTime;
            float fadeAlpha = 1f - fadeTimer / fadeoutDuration; // 0f -> 1f FadeIn
            images[activePage][currentPage].color = new Color(1, 1, 1, fadeAlpha);

            if (fadeTimer <= 0f){
                pageTimer = pageDuration;
                fadeTimer = 0f; // Goto (2)
            }
        }

        if (pageTimer > 0f && fadeTimer == 0f){ // (2)
            part = 2;
            pageTimer -= Time.fixedDeltaTime;
            waitTimer = -1f; // (3) -> (1)로 가는 것을 방지하는 트리거
            if ((pageTimer <= 0f || skip) && currentPage == 3){ // 마지막 페이지이면
                // 엔딩 컷씬 종료
                darkPanel.SetActive(false); // 미리 검은 색 불투명 배경 제거

                // TODO: Gameover UI 활성화
                gameoverUI.SetActive(true);

                fadeTimer = fadeoutDuration;
                pageTimer = 0f; // Goto (3)
            } else if ((pageTimer <= 0f || skip) && currentPage != 3){
                // 다음 페이지 활성화
                currentPage++;
                skip = false;
                fadeTimer = fadeoutDuration;
                pageTimer = 0f;
                waitTimer = 0f; // Goto (1)
            }
        }

        if (pageTimer == 0f && fadeTimer > 0f && waitTimer == -1f){ // (3)
            part = 3;
            fadeTimer -= Time.fixedDeltaTime;
            float fadeAlpha = fadeTimer / fadeoutDuration; // 1f -> 0f FadeOut
            for (int i = 0; i < 4; i++)
            {
                images[activePage][i].color = new Color(1, 1, 1, fadeAlpha);
            }
            if (fadeTimer <= 0f){
                fadeTimer = 0f; // Goto (4)
            }
        }

        if (pageTimer == 0f && fadeTimer == 0f && waitTimer == -1f){ // (4)
            part = 4;
            // 완전히 종료
            // TODO: Gameover UI 점수 증가 애니메이션 재생
            gameoverUIManager.increase = true;

            gameObject.SetActive(false);
        }
    }
}

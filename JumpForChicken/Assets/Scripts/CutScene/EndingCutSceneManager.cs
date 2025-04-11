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

    private Image[] images;

    private float waitTimer;
    private float fadeTimer;
    private float pageTimer;

    private int activePage = 0;


    /// <summary>
    /// 컷씬 스킵 (스크립트 내에서 재사용 가능)
    /// </summary>
    public void SkipCutScene(){

    }

    void Start()
    {
        images = new Image[pages.Length];

        for (int i = 0; i < pages.Length; i++)
        {
            images[i] = pages[i].transform.GetChild(0).GetComponent<Image>();
        }
    }

    void OnEnable()
    {
        Debug.Log("[EndingCutSceneManager] Point 0");
        waitTimer = waitDuration;
        // GameManager에서 데이터 가져오기 (DataManager에서 데이터 저장도 함께 구현)
        activePage = GameManager.GetEndingPage();
    }

    void Update()
    {
        if (waitTimer > 0f){
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0f){
                pages[activePage].SetActive(true);
                images[activePage].color = new Color(1, 1, 1, 0);
                waitTimer = 0f;
                fadeTimer = fadeoutDuration; // Goto (1)
            }
        }

        if (pageTimer == 0f && fadeTimer > 0f && waitTimer != -1f){ // (1)
            // 페이지 활성화
            fadeTimer -= Time.fixedDeltaTime;
            float fadeAlpha = 1f - fadeTimer / fadeoutDuration; // 0f -> 1f FadeIn
            images[activePage].color = new Color(1, 1, 1, fadeAlpha);

            if (fadeTimer <= 0f){
                pageTimer = pageDuration;
                fadeTimer = 0f; // Goto (2)
            }
        }

        if (pageTimer > 0f && fadeTimer == 0f){ // (2)
            pageTimer -= Time.fixedDeltaTime;
            waitTimer = -1f; // (3) -> (1)로 가는 것을 방지하는 트리거
            if (pageTimer <= 0f){
                // 엔딩 컷씬 종료
                darkPanel.SetActive(false); // 미리 검은 색 불투명 배경 제거

                // TODO: Gameover UI 활성화
                gameoverUI.SetActive(true);

                fadeTimer = fadeoutDuration;
                pageTimer = 0f; // Goto (3)
            }
        }

        if (pageTimer == 0f && fadeTimer > 0f && waitTimer == -1f){ // (3)
            fadeTimer -= Time.fixedDeltaTime;
            float fadeAlpha = fadeTimer / fadeoutDuration; // 1f -> 0f FadeOut
            images[activePage].color = new Color(1, 1, 1, fadeAlpha);
            if (fadeTimer <= 0f){
                fadeTimer = 0f; // Goto (4)
            }
        }

        if (pageTimer == 0f && fadeTimer == 0f && waitTimer == -1f){ // (4)
            // 완전히 종료
            // TODO: Gameover UI 점수 증가 애니메이션 재생
            gameoverUIManager.increase = true;

            gameObject.SetActive(false);
        }
    }
}

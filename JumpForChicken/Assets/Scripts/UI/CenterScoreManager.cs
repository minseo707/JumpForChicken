using UnityEngine;
using TMPro;

public class CenterScoreManager : MonoBehaviour
{
    public GameObject HighIcon; // 아이콘 (100x100 사이즈)
    public TextMeshProUGUI scoreText; // 점수 텍스트
    public TextMeshProUGUI unitText;  // "m" 텍스트
    public float iconSpacing = 50f;   // 아이콘과 점수 텍스트 사이의 간격
    public float unitSpacing = 75f;   // 점수와 "m" 텍스트 사이의 간격
    public float unitOffsetY = -25f;  // "m" 텍스트의 Y 좌표 조정값

    void Start()
    {
        SetAnchorsToCenter();
    }

    void Update()
    {
        AlignObjects();
    }

    void SetAnchorsToCenter()
    {
        // 모든 오브젝트의 앵커를 중앙으로 설정
        RectTransform highIconRect = HighIcon.GetComponent<RectTransform>();
        highIconRect.anchorMin = highIconRect.anchorMax = new Vector2(0.5f, 0.5f);

        RectTransform scoreRect = scoreText.rectTransform;
        scoreRect.anchorMin = scoreRect.anchorMax = new Vector2(0.5f, 0.5f);

        RectTransform unitRect = unitText.rectTransform;
        unitRect.anchorMin = unitRect.anchorMax = new Vector2(0.5f, 0.5f);
    }

    void AlignObjects()
    {
        // 아이콘, 점수 텍스트, m 텍스트의 총 너비 계산
        float highIconWidth = HighIcon.GetComponent<RectTransform>().rect.width;
        float scoreWidth = scoreText.preferredWidth;
        float unitWidth = unitText.preferredWidth;

        float totalWidth = highIconWidth + iconSpacing + scoreWidth + unitSpacing + unitWidth;

        // 중앙 기준 X 좌표
        float centerX = 0;

        // HighIcon의 위치 설정 (왼쪽에 배치)
        RectTransform highIconRect = HighIcon.GetComponent<RectTransform>();
        highIconRect.anchoredPosition = new Vector3(
            centerX - totalWidth / 2 + highIconWidth / 2,
            -222, // 중앙 기준 Y 좌표
            0
        );

        // 점수 텍스트의 위치 설정 (아이콘 오른쪽에 배치)
        float highIconEndX = highIconRect.anchoredPosition.x + highIconWidth / 2 + iconSpacing;
        scoreText.rectTransform.anchoredPosition = new Vector3(
            highIconEndX + scoreWidth / 2,
            -150, // 중앙 기준 Y 좌표
            0
        );

        // "m" 텍스트의 위치 설정 (점수 텍스트 오른쪽에 배치)
        float scoreEndX = scoreText.rectTransform.anchoredPosition.x + scoreWidth / 2;
        unitText.rectTransform.anchoredPosition = new Vector3(
            scoreEndX + unitSpacing + unitWidth / 2,
            -150 + unitOffsetY, // Y 오프셋 적용
            0
        );
    }
}
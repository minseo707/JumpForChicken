using UnityEngine;
using TMPro;

public class ScoreDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 점수 텍스트
    public TextMeshProUGUI unitText;  // m 텍스트
    public float unitOffsetY = -10f;  // m의 Y 좌표 조정값
    public float spacing = 5f;        // 점수와 m 사이의 간격

    private CameraController cc;

    void Start()
    {
        cc = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (cc == null) cc = Camera.main.GetComponent<CameraController>();
        if (cc == null) return;

        // 점수 텍스트의 우측 끝 위치
        Vector2 scoreEndPos = scoreText.rectTransform.anchoredPosition + new Vector2(scoreText.preferredWidth / 2, 0);

        // m 텍스트의 위치를 점수의 우측 끝에 맞춰 조정하고, 아래 첨자 느낌을 주기 위해 Y 좌표를 조정
        unitText.rectTransform.anchoredPosition = new Vector2(
            scoreEndPos.x + spacing,
            scoreText.rectTransform.anchoredPosition.y + unitOffsetY
        );
    }
}
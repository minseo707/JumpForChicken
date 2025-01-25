using UnityEngine;
using TMPro;

public class ScoreDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 점수 텍스트
    public TextMeshProUGUI unitText;  // m 텍스트
    public float unitOffsetY = -10f;  // m의 Y 좌표 조정값
    public float spacing = 5f;        // 점수와 m 사이의 간격

    void Update()
    {
        // 점수 텍스트의 우측 끝 위치
        Vector3 scoreEndPos = scoreText.rectTransform.position + new Vector3(scoreText.preferredWidth / 2, 0, 0);

        // m 텍스트의 위치를 점수의 우측 끝에 맞춰 조정하고, 아래 첨자 느낌을 주기 위해 Y 좌표를 조정
        unitText.rectTransform.position = new Vector3(
            scoreEndPos.x / 100 + spacing / 100,
            scoreText.rectTransform.position.y + unitOffsetY/100,
            scoreText.rectTransform.position.z
        );
    }
}
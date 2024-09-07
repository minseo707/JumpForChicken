using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = -1; // 바닥도 점수로 인식해서 0으로 시작
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        scoreText.text = "Score : 0";
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();  // 점수 변경 시 텍스트 업데이트
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score : " + score;
    }
}

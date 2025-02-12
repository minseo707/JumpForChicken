using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Collections;
using UnityEngine.Rendering.Universal;

/**ScoreManager에서 Chicken 개수와 Score 모두 관리합니다.*/
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0; // 바닥도 점수로 인식해서 0으로 시작
    public int chicken = 0; // 치킨 개수

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI chickenText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        scoreText.text = "0";
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();  // 점수 변경 시 텍스트 업데이트
    }

    public void AddChicken(int amount){
        chicken += amount;
        UpdateChickenText(); // 치킨 게수 변경 시 텍스트 업데이트
    }

    private void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateChickenText(){
        chickenText.text = "x " + chicken.ToString();
    }
}

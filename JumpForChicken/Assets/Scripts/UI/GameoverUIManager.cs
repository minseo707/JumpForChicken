using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverUIManager : MonoBehaviour
{

    public float increaseSecond = 1f;
    private float viewScore = 0;
    private int currentScore;

    public GameObject highScoreText;
    public GameObject chickenText;
    public TextMeshProUGUI scoreTMP;

    GameObject gm;

    void Awake()
    {
        gm = GameObject.Find("GameManager");
    }

    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.LoadGameData();
        currentScore = gm.GetComponent<ScoreManager>().score;
        int currentChicken = gm.GetComponent<ScoreManager>().chicken;
        viewScore = 0;

        if (highScoreText != null && chickenText != null){

            if (currentScore > DataManager.Instance.gameData.highScore){
                DataManager.Instance.gameData.highScore = currentScore;
            }

            highScoreText.GetComponent<TextMeshProUGUI>().text = $"High Score: {DataManager.Instance.gameData.highScore}m";
            chickenText.GetComponent<TextMeshProUGUI>().text = $"x {currentChicken}";

            DataManager.Instance.gameData.chickens += currentChicken;
            DataManager.Instance.SaveGameData();
        }

        
    }

    void Update()
    {
        if (viewScore < currentScore){
            viewScore += currentScore / increaseSecond * Time.unscaledDeltaTime;
        }
        if (viewScore >= currentScore){
            viewScore = currentScore;
        }
        scoreTMP.text = ((int)viewScore).ToString();
    }
}

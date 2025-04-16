using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverUIManager : MonoBehaviour
{

    public float increaseSecond = 3f;
    private float viewScore = 0;
    private int currentScore;

    internal bool increase;

    private bool soundPlayTrigger = false;

    private SoundPlayManager soundPlayManager;

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
        soundPlayManager = soundPlayManager = GameObject.Find("Sound Player").GetComponent<SoundPlayManager>();
        viewScore = 0;
        increase = false;
        soundPlayTrigger = false;

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
        if (!increase) return;

        if (!soundPlayTrigger){
            soundPlayTrigger = true;
            soundPlayManager.PlaySound("score");
        }
        
        if (viewScore < currentScore){
            viewScore += currentScore / increaseSecond * Time.unscaledDeltaTime;
        }
        if (viewScore >= currentScore){
            viewScore = currentScore;
        }
        scoreTMP.text = ((int)viewScore).ToString();
    }
}

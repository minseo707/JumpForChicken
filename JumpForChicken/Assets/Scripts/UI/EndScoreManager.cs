using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScoreManager : MonoBehaviour
{

    GameObject gm;
    public TextMeshProUGUI scoreText;
    void Awake()
    {
        gm = GameObject.Find("GameManager");
    }

    void OnEnable() {
        scoreText.text = gm.GetComponent<ScoreManager>().score.ToString();
    }
}

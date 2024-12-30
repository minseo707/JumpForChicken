using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject platformData;

    public GameObject pauseUICanvas;

    public GameObject endingUICanvas;

    public TextMeshProUGUI fpsText;

    private float deltaTime = 0f;

    private bool isPause;

    private void Awake() {
        platformData = GameObject.Find("Platform");
        deltaTime = 0f;
    }

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fpsText.text = $"FPS: {Mathf.Floor(10f/deltaTime)*0.1f}";

        // GamePause Logic
        if (isPause == true){
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    public void OnclickPauseButton(){
        isPause = true;
        pauseUICanvas.SetActive(true);
    }

    public void OnclickStartButton(){
        isPause = false;
        pauseUICanvas.SetActive(false);
    }

    public void OnClickRestartButton(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EndingUIActive(){
        Time.timeScale = 0f;
        isPause = true;
        endingUICanvas.SetActive(true);
        pauseUICanvas.SetActive(false);
    }
}

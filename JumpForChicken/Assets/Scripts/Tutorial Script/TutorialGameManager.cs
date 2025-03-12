using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 튜토리얼에서의 버튼 이벤트 및 UI 관리
/// </summary>
public class TutorialGameManager : MonoBehaviour
{
    public GameObject pauseUICanvas;

    public GameObject clearUICanvas;

    public GameObject realExitContainer;
    public GameObject settingsContainer;

    public TextMeshProUGUI fpsText;

    private float deltaTime = 0f;

    private bool isGameover = false;

    private ButtonSoundManager bsm;

    private void Awake() {
        deltaTime = 0f;
        isGameover = false;
        DataManager.Instance.LoadGameData();
    }

    private void Start()
    {
        bsm = ButtonSoundManager.Instance;
    }

    private void Update() {
        // 초당 프레임 수 관리
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fpsText.text = $"FPS: {Mathf.Round(1 / deltaTime)}";

        if (Input.GetKeyDown(KeyCode.Escape) && (!isGameover)){ // 뒤로가기 버튼 (모바일, 데스크탑 지원)
            OnclickPauseButton();
            OnClickSettingsCancelButton();
            OnClickExitCancelButton();
        }
    }

    public void OnclickPauseButton(){
        Time.timeScale = 0f;
        bsm.PlayButtonSound("button1");
        pauseUICanvas.SetActive(true);
    }

    public void OnClickExitButton(){
        bsm.PlayButtonSound("button1");
        realExitContainer.SetActive(true);
    }

    public void OnClickRealExitButton(){
        bsm.PlayButtonSound("button1");
        SceneManager.LoadScene("MainTitle");
    }

    public void OnClickExitCancelButton(){
        bsm.PlayButtonSound("button1");
        realExitContainer.SetActive(false);
    }

    public void OnClickPauseCancelButton(){
        Time.timeScale = 1f;
        bsm.PlayButtonSound("button1");
        pauseUICanvas.SetActive(false);
    }

    public void OnClickRestartButton(){
        bsm.PlayButtonSound("startButton", 2f);
        SceneManager.LoadScene("TutorialScene");
    }

    public void OnClickStartButton(){
        bsm.PlayButtonSound("startButton", 2f);
        LoadingSceneManager.LoadScene("SampleScene");
    }

    public void OnclickSettingsButton(){
        bsm.PlayButtonSound("button1");
        settingsContainer.SetActive(true);
    }

    public void OnClickSettingsCancelButton(){
        bsm.PlayButtonSound("button1");
        settingsContainer.SetActive(false);
    }

    public void EndingUIActive(){
        Time.timeScale = 0f;
        isGameover = true;
        clearUICanvas.SetActive(true);
        pauseUICanvas.SetActive(false);
    }

    public void TutorialClear(){
        Time.timeScale = 0f;
        clearUICanvas.SetActive(true);
        DataManager.Instance.gameData.isFirstGame = false;
        DataManager.Instance.SaveGameData();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 인게임에서의 버튼 이벤트 및 UI 관리
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject pauseUICanvas;

    public GameObject endingUICanvas;

    public GameObject realExitContainer;
    public GameObject settingsContainer;

    public TextMeshProUGUI fpsText;

    private float deltaTime = 0f;

    private bool isGameover = false;


    private void Awake() {
        deltaTime = 0f;
        isGameover = false;
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
        pauseUICanvas.SetActive(true);
    }

    public void OnClickExitButton(){
        realExitContainer.SetActive(true);
    }

    public void OnClickRealExitButton(){
        SceneManager.LoadScene("MainTitle");
    }

    public void OnClickExitCancelButton(){
        realExitContainer.SetActive(false);
    }

    public void OnclickStartButton(){
        Time.timeScale = 1f;
        pauseUICanvas.SetActive(false);
    }

    public void OnClickRestartButton(){
        LoadingSceneManager.LoadScene("SampleScene");
    }

    public void OnclickSettingsButton(){
        settingsContainer.SetActive(true);
    }

    public void OnClickSettingsCancelButton(){
        settingsContainer.SetActive(false);
    }

    public void EndingUIActive(){
        Time.timeScale = 0f;
        isGameover = true;
        endingUICanvas.SetActive(true);
        pauseUICanvas.SetActive(false);
    }
}

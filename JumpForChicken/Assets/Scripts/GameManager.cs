using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 인게임에서의 버튼 이벤트 및 UI 관리
/// </summary>
public class GameManager : MonoBehaviour
{
    public static int stage = 1;

    public GameObject pauseUICanvas;

    public GameObject endingUICanvas;

    public GameObject realExitContainer;
    public GameObject settingsContainer;

    public TextMeshProUGUI fpsText;

    private float deltaTime = 0f;

    private bool isGameover = false;

    private ButtonSoundManager bsm;

    private BackgroundManager backgroundManager;


    private void Awake() {
        deltaTime = 0f;
        isGameover = false;
    }

    void Start()
    {
        bsm = ButtonSoundManager.Instance;
        backgroundManager = GameObject.Find("Background").GetComponent<BackgroundManager>();
        stage = 1;
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

    /// <summary>
    /// 3 스테이지에 일정 간격 비행기 설치
    /// </summary>
    /// <param name="airplanePrefab">비행기 장애물 프리팹</param>
    /// <param name="n">설치 개수</param>
    public static void PlaceAirplane(GameObject airplanePrefab, int n)
    {
        int stage3Height = 220;
        float startHeight = 322f;
        GameObject airplanesParent = GameObject.Find("Airplanes"); // 씬에 미리 만들어둔 빈 게임오브젝트

        if (airplanesParent == null)
        {
            Debug.LogError("AirplanesParent not found in the scene!");
            return;
        }
        for (int i = 1; i <= n; i++)
        {
            GameObject airplane = Instantiate(airplanePrefab, new Vector3(0f, startHeight + (float)stage3Height / (n + 1) * i, 1), Quaternion.identity);
            airplane.transform.SetParent(airplanesParent.transform);
            Debug.Log($"Airplane created at position: {airplane.transform.position}");
        }

        Debug.Log("[GameManager] Airplane installed: " + n);
    }

    public void GoToNextStage(){
        BlockStore.SetAllNext();
        StartCoroutine(GameObject.Find("Player").GetComponent<PlayerController>().NextStageAnimaiton());
    }

    public void ChangeBackground(int currentStage){
        backgroundManager.ChangeBackgroundSprite(currentStage);
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

    public void OnclickStartButton(){
        Time.timeScale = 1f;
        bsm.PlayButtonSound("button1");
        pauseUICanvas.SetActive(false);
    }

    public void OnClickRestartButton(){
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
        endingUICanvas.SetActive(true);
        pauseUICanvas.SetActive(false);
    }
}

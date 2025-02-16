using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 로딩 씬에서 다음 오브젝트들을 미리 불러옴
/// </summary>
public class LoadingSceneManager : MonoBehaviour
{
    public static string nextSceneName = "SampleScene";
    public TextMeshProUGUI loadingText;

    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName = "SampleScene"){
        nextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene(){
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        // AsyncOperation의 progress는 0.9까지 불러오다가 allowSceneActivation = true일 때, 최종적으로 불러오기 시작하여 활성화 합니다.
        // 그 때 op.progress가 1f가 되고, op.isDone = true가 됩니다. 이때부터 오브젝트와 스크립트에 접근할 수 있습니다.
        while(op.progress < 0.9f){
            loadingText.text = $"Loading... {(int)Mathf.Round(op.progress * 250 / 9)}%";
            yield return null;
        }

        loadingText.text = $"Loading... {25}%";

        // 씬 활성화 허용
        op.allowSceneActivation = true;
        // 씬 로딩 완료 대기
        yield return new WaitUntil(() => op.isDone);
        
        Scene next = SceneManager.GetSceneByName(nextSceneName);
        Time.timeScale = 0;
        if (!next.IsValid()){
            Debug.LogError("[LoadingSceneManager] Failed to load scene");
        } else {
            Debug.Log("[LoadingSceneManager] Successfully loaded scene");
        }
        GameObject[] rootGameObjects = next.GetRootGameObjects();
        GameObject mainCamera = null;

        // 씬의 모든 카메라 비활성화
        foreach (var root in rootGameObjects)
        {
            if (root.name == "Main Camera"){
                mainCamera = root;
                mainCamera.SetActive(false);
                Debug.Log("[LoadingSceneManager] 카메라를 비활성화 했습니다.");
                break;
            }
        }
        
        GameObject platformGeneratorObj = null; // 예외 처리
        foreach (var root in rootGameObjects) // rootGameObjects는 반드시 존재하므로 예외 처리가 필요 없습니다.
        {
            if (root.name == "Platform")
            {
                platformGeneratorObj = root;
                break;
            }
        }

        float loadStartTime = Time.realtimeSinceStartup;

        if (platformGeneratorObj != null) // 예외 처리
        {
            PlatformGenerator platformGeneratorScript = platformGeneratorObj.GetComponent<PlatformGenerator>();
            if (platformGeneratorScript != null)
            {
                // 예) 직접 만든 초기화 메서드 호출
                platformGeneratorScript.GenerateInitialTiles();
                // 블록 로딩 시간 출력
                Debug.Log($"[LoadingSceneManager] Blocks loaded in {Mathf.Round((Time.realtimeSinceStartup - loadStartTime) * 1000)} ms.");
            }
        }
        else
        {
            Debug.LogWarning("'Platform' 오브젝트를 찾지 못했습니다.");
        }

        // 로딩 퍼센트가 순차적으로 증가하는 것을 인위적으로 제작
        // 추후 GenerateInitialTiles()와 연계하여 연동 예정
        float timer = 0f;
        while (timer < 1f){
            loadingText.text = $"Loading... {Mathf.Round(Mathf.Lerp(25, 100, timer))}%";
            timer += Time.unscaledDeltaTime * 2.25f;
            yield return null;
        }

        loadingText.text = "Loading... 100%";
        yield return new WaitForSecondsRealtime(0.08f);
        loadingText.text = "Load Complete!";
        yield return new WaitForSecondsRealtime(0.16f);

        // 설정 완료 후 카메라 재활성화, 인게임 시작
        mainCamera.SetActive(true);
        mainCamera.GetComponent<MainLetterboxCamera>().AspectControl();
        Time.timeScale = 1;

        SceneManager.UnloadSceneAsync("LoadingScene");
    }

}

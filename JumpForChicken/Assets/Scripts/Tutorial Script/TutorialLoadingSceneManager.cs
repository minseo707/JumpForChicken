using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 튜토리얼로의 로딩 씬에서 다음 오브젝트들을 미리 불러옴
/// </summary>
public class TutorialLoadingSceneManager : MonoBehaviour
{
    public static string nextSceneName = "TutorialScene";
    public TextMeshProUGUI loadingText;


    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName = "TutorialScene"){
        nextSceneName = sceneName;
        SceneManager.LoadScene("TutorialLoadingScene");
    }

    IEnumerator LoadScene(){
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f){
            loadingText.text = $"Loading... {(int)Mathf.Round(op.progress * 1000 / 9)}%";
            yield return null;
        }

        loadingText.text = $"Loading... {100}%";

        yield return new WaitForSecondsRealtime(0.16f);

        op.allowSceneActivation = true;

        SceneManager.UnloadSceneAsync("TutorialLoadingScene");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMusicManager : MonoBehaviour
{
    // singleton
    public static MainMusicManager Instance;

    private AudioSource audioSource;  // Inspector에서 할당하거나, 없으면 자동으로 가져옵니다.

    private bool playing;

    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // // 씬 로드 이벤트에 구독 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        } else {
            Destroy(gameObject);
        }

        DataManager.Instance.LoadSettingsData();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy(){
        // // 씬 로드 이벤트에 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if (scene.name != "MainTitle" && scene.name != "ShopScene"){
            Destroy(gameObject);
        }    
    }

    void Start()
    {
        playing = false;
    }


    public void MusicPlay(){
        if (playing) return;
        playing = true;
        audioSource.volume = DataManager.Instance.settingsData.volumes[0] * DataManager.Instance.settingsData.volumes[1];
        audioSource.Play();
        Debug.Log("Music Play");
    }

    public void MusicStop(){
        if (!playing) return;
        playing = false;
        audioSource.Pause();
        Debug.Log("Music Stop");
    }

    
}

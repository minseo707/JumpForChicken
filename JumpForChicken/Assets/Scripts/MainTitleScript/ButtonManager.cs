using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsContainer;
    public GameObject realQuitContainer;
    private MainMusicManager mmm;

    private bool settingsUIEnabled = false;

    void Start()
    {
        mmm = MainMusicManager.Instance;
    }

    private void Update(){
        // Escape가 눌리지 않았다면 탈출
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (settingsUIEnabled){
            SettingsCancelButton();
        } else {
            RealQuitButton();
        }
    }

    public void StartButton(){
        LoadingSceneManager.LoadScene("SampleScene");
    }

    public void SettingsButton(){
        settingsContainer.SetActive(true);
        settingsUIEnabled = true;
        mmm.MusicStop();
    }

    public void SettingsCancelButton(){
        settingsContainer.SetActive(false);
        settingsUIEnabled = false;
        mmm.MusicPlay();
    }

    void RealQuitButton(){
        realQuitContainer.SetActive(true);
        mmm.MusicStop();
    }

    public void RealQuitCancelButton(){
        realQuitContainer.SetActive(false);
        mmm.MusicPlay();
    }

    public void OnClickRealQuitButton(){
        Application.Quit();
    }

    public void OnClickShopButton(){
        SceneManager.LoadScene("ShopScene");
    }
}
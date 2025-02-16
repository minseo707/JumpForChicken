using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsContainer;
    public GameObject realQuitContainer;

    private bool settingsUIEnabled = false;

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
    }

    public void SettingsCancelButton(){
        settingsContainer.SetActive(false);
        settingsUIEnabled = false;
    }

    void RealQuitButton(){
        realQuitContainer.SetActive(true);
    }

    public void RealQuitCancelButton(){
        realQuitContainer.SetActive(false);
    }

    public void OnClickRealQuitButton(){
        Application.Quit();
    }

    public void OnClickShopButton(){
        SceneManager.LoadScene("ShopScene");
    }
}
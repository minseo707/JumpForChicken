using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsContainer;

    public void StartButton(){
        LoadingSceneManager.LoadScene("SampleScene");
    }

    public void SettingsButton(){
        settingsContainer.SetActive(true);
    }

    public void SettingsCancelButton(){
        settingsContainer.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public static ButtonSoundManager Instance;

    public GameObject button1;
    public GameObject startButton;


    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        DataManager.Instance.LoadSettingsData();
    }


    public void PlayButtonSound(string sound, float volume = 1f, float pitch = 1f){
        switch (sound) {
            case "button1":
                button1.GetComponent<AudioSource>().volume = 
                    volume * DataManager.Instance.settingsData.volumes[0] * DataManager.Instance.settingsData.volumes[2];
                button1.GetComponent<AudioSource>().pitch = pitch;
                button1.GetComponent<AudioSource>().Play();
                break;
            case "startButton":
                startButton.GetComponent<AudioSource>().volume = 
                    volume * DataManager.Instance.settingsData.volumes[0] * DataManager.Instance.settingsData.volumes[2];
                startButton.GetComponent<AudioSource>().pitch = pitch;
                startButton.GetComponent<AudioSource>().Play();
                break;
            default:
                Debug.LogWarning("No sound clip found with the name: " + sound);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public GameObject music1;
    public GameObject music2;
    public GameObject music3;
    public GameObject music4;

    private GameObject soundPlayManager;

    private bool isMusicPlaying = false;


    // GameManager와 연결 전 임시 변수
    public int currentStage = 1;

    void Start(){
        soundPlayManager = GameObject.Find("Sound Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMusicPlaying && (Time.timeScale != 0)) {
            switch (currentStage) {
                case 1:
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("bgm1");
                    isMusicPlaying = true;
                    break;
                case 2:
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("bgm2");
                    isMusicPlaying = true;
                    break;
                case 3:
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("bgm3");
                    isMusicPlaying = true;
                    break;
                case 4:
                    soundPlayManager.GetComponent<SoundPlayManager>().PlaySound("bgm4");
                    isMusicPlaying = true;
                    break;
                default:
                    Debug.Log("[Background Music Manager] Cannot play sound: " + currentStage);
                    break;
            }
        } else if (isMusicPlaying && (Time.timeScale == 0)){
            soundPlayManager.GetComponent<SoundPlayManager>().StopSound("allBgm");
            isMusicPlaying = false;
        }
    }
}

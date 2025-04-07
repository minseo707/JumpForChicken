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

    internal bool isMusicPlaying = false;

    internal float waitTime = 0f;


    // GameManager와 연결 전 임시 변수
    public int currentStage = 1;

    void Start(){
        soundPlayManager = GameObject.Find("Sound Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0) waitTime = Mathf.Max(0, waitTime - Time.deltaTime);

        if (!isMusicPlaying && (Time.timeScale != 0) && waitTime == 0f) {
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
        } else if (!isMusicPlaying && (Time.timeScale != 0) && waitTime > 0f){
            switch (currentStage) {
                case 2:
                    FadeAudio("bgm1", 3f);
                    break;
                case 3:
                    FadeAudio("bgm2", 3f);
                    break;
                case 4:
                    FadeAudio("bgm3", 3f);
                    break;
                default:
                    Debug.Log("[Background Music Manager] Cannot play sound: " + currentStage);
                    break;
            }
        }
    }

    bool fading = false;

    private void FadeAudio(string sound, float duration){
        if (soundPlayManager.GetComponent<SoundPlayManager>().bgmAudioSources.TryGetValue(sound, out AudioSource bgmSource))
        {
            if (fading) return;
            StartCoroutine(AudioFadeOut.FadeOut(bgmSource, duration));
            StartCoroutine(_fade(duration));
            fading = true;
        }
    }

    IEnumerator _fade(float duration){
        yield return new WaitForSeconds(duration);
        fading = false;
    }
}

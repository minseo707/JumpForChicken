using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayManager : MonoBehaviour
{

    public GameObject jump1;
    public GameObject jump2;
    public GameObject jumpReady;
    public GameObject landing;
    public GameObject breakFx;
    public GameObject failed;
    public GameObject flap;
    public GameObject tick;
    public GameObject gaugeDisappear;
    public GameObject acquire;
    public GameObject walking;
    public GameObject bgmManager;


    // Slider
    public Slider totalVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider fxVolumeSlider;

    public void PlaySound(string sound, float volume = 1f, float pitch = 1f){
        switch (sound) {
            case "jump1": // Fx
                jump1.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                jump1.GetComponent<AudioSource>().pitch = pitch;
                jump1.GetComponent<AudioSource>().Play();
                break;
            case "jump2": // Fx
                jump2.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                jump2.GetComponent<AudioSource>().pitch = pitch;
                jump2.GetComponent<AudioSource>().Play();
                break;
            case "jumpReady": // Fx
                jumpReady.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                jumpReady.GetComponent<AudioSource>().pitch = pitch;
                jumpReady.GetComponent<AudioSource>().Play();
                break;
            case "landing": // Fx
                landing.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                landing.GetComponent<AudioSource>().pitch = pitch;
                landing.GetComponent<AudioSource>().Play();
                break;
            case "breakFx": // Fx
                breakFx.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                breakFx.GetComponent<AudioSource>().pitch = pitch;
                breakFx.GetComponent<AudioSource>().Play();
                break;
            case "failed": // Fx
                failed.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                failed.GetComponent<AudioSource>().pitch = pitch;
                failed.GetComponent<AudioSource>().Play();
                break;
            case "flap": // Fx
                flap.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                flap.GetComponent<AudioSource>().pitch = pitch;
                flap.GetComponent<AudioSource>().Play();
                break;
            case "tick": // Fx
                tick.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                tick.GetComponent<AudioSource>().pitch = pitch;
                tick.GetComponent<AudioSource>().Play();
                break;
            case "gaugeDisappear": // Fx
                gaugeDisappear.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                gaugeDisappear.GetComponent<AudioSource>().pitch = pitch;
                gaugeDisappear.GetComponent<AudioSource>().Play();
                break;
            case "walking": // Fx
                walking.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                walking.GetComponent<AudioSource>().pitch = pitch;
                walking.GetComponent<AudioSource>().Play();
                break;
            case "acquire": // Fx
                acquire.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * fxVolumeSlider.value;
                acquire.GetComponent<AudioSource>().pitch = pitch;
                acquire.GetComponent<AudioSource>().Play();
                break;
            case "bgm1":
                bgmManager.transform.GetChild(0).gameObject.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * bgmVolumeSlider.value;
                bgmManager.transform.GetChild(0).gameObject.GetComponent<AudioSource>().pitch = pitch;
                bgmManager.transform.GetChild(0).gameObject.GetComponent<AudioSource>().Play();
                break;
            case "bgm2":
                bgmManager.transform.GetChild(1).gameObject.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * bgmVolumeSlider.value;
                bgmManager.transform.GetChild(1).gameObject.GetComponent<AudioSource>().pitch = pitch;
                bgmManager.transform.GetChild(1).gameObject.GetComponent<AudioSource>().Play();
                break;
            case "bgm3":
                bgmManager.transform.GetChild(2).gameObject.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * bgmVolumeSlider.value;
                bgmManager.transform.GetChild(2).gameObject.GetComponent<AudioSource>().pitch = pitch;
                bgmManager.transform.GetChild(2).gameObject.GetComponent<AudioSource>().Play();
                break;
            case "bgm4":
                bgmManager.transform.GetChild(3).gameObject.GetComponent<AudioSource>().volume = 
                    volume * totalVolumeSlider.value * bgmVolumeSlider.value;
                bgmManager.transform.GetChild(3).gameObject.GetComponent<AudioSource>().pitch = pitch;
                bgmManager.transform.GetChild(3).gameObject.GetComponent<AudioSource>().Play();
                break;
            default:
                Debug.LogWarning("No sound clip found with the name: " + sound);
                break;
        }
    }

    public void StopSound(string sound){
        switch (sound){
            case "walking":
                walking.GetComponent<AudioSource>().Pause();
                break;
            case "bgm1":
                bgmManager.transform.GetChild(0).gameObject.GetComponent<AudioSource>().Pause();
                break;
            case "bgm2":
                bgmManager.transform.GetChild(1).gameObject.GetComponent<AudioSource>().Pause();
                break;
            case "bgm3":
                bgmManager.transform.GetChild(2).gameObject.GetComponent<AudioSource>().Pause();
                break;
            case "bgm4":
                bgmManager.transform.GetChild(3).gameObject.GetComponent<AudioSource>().Pause();
                break;
            case "allBgm":
                for (int i = 0; i < 4; i++) bgmManager.transform.GetChild(i).gameObject.GetComponent<AudioSource>().Pause();
                break;
            default:
                Debug.LogWarning("Cannot Stop Sound: " + sound);
                break;
        }
    }
}

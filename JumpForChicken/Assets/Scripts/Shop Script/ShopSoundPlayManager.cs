using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSoundPlayManager : MonoBehaviour
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

    public GameObject purchase;

    public float totalVolume = 1f;
    public float bgmVolume = 1f;
    public float fxVolume = 1f;


    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.LoadSettingsData();
        totalVolume = DataManager.Instance.settingsData.volumes[0];
        bgmVolume = DataManager.Instance.settingsData.volumes[1];
        fxVolume = DataManager.Instance.settingsData.volumes[2];
    }

    public void PlaySound(string sound, float volume = 1f, float pitch = 1f){
        switch (sound) {
            case "jump1": // Fx
                jump1.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                jump1.GetComponent<AudioSource>().pitch = pitch;
                jump1.GetComponent<AudioSource>().Play();
                break;
            case "jump2": // Fx
                jump2.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                jump2.GetComponent<AudioSource>().pitch = pitch;
                jump2.GetComponent<AudioSource>().Play();
                break;
            case "jumpReady": // Fx
                jumpReady.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                jumpReady.GetComponent<AudioSource>().pitch = pitch;
                jumpReady.GetComponent<AudioSource>().Play();
                break;
            case "landing": // Fx
                landing.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                landing.GetComponent<AudioSource>().pitch = pitch;
                landing.GetComponent<AudioSource>().Play();
                break;
            case "breakFx": // Fx
                breakFx.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                breakFx.GetComponent<AudioSource>().pitch = pitch;
                breakFx.GetComponent<AudioSource>().Play();
                break;
            case "failed": // Fx
                failed.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                failed.GetComponent<AudioSource>().pitch = pitch;
                failed.GetComponent<AudioSource>().Play();
                break;
            case "flap": // Fx
                flap.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                flap.GetComponent<AudioSource>().pitch = pitch;
                flap.GetComponent<AudioSource>().Play();
                break;
            case "tick": // Fx
                tick.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                tick.GetComponent<AudioSource>().pitch = pitch;
                tick.GetComponent<AudioSource>().Play();
                break;
            case "gaugeDisappear": // Fx
                gaugeDisappear.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                gaugeDisappear.GetComponent<AudioSource>().pitch = pitch;
                gaugeDisappear.GetComponent<AudioSource>().Play();
                break;
            case "walking": // Fx
                walking.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                walking.GetComponent<AudioSource>().pitch = pitch;
                walking.GetComponent<AudioSource>().Play();
                break;
            case "acquire": // Fx
                acquire.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                acquire.GetComponent<AudioSource>().pitch = pitch;
                acquire.GetComponent<AudioSource>().Play();
                break;
            case "purchase":
                purchase.GetComponent<AudioSource>().volume = 
                    volume * totalVolume * fxVolume;
                purchase.GetComponent<AudioSource>().pitch = pitch;
                purchase.GetComponent<AudioSource>().Play();
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
            default:
                Debug.LogWarning("Cannot Stop Sound: " + sound);
                break;
        }
    }
}

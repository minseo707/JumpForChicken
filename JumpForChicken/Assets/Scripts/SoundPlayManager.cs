using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SoundPlayManager : MonoBehaviour
{

    public GameObject jump1;
    public GameObject jump2;
    public GameObject jumpReady;
    public GameObject landing;
    public GameObject breakFx;
    public GameObject failed;
    public GameObject flap;
    public GameObject acquire;
    public GameObject walking;
    public GameObject bgmManager;

    public void PlaySound(string sound, float volume = 1f, float pitch = 1f){
        switch (sound) {
            case "jump1":
                jump1.GetComponent<AudioSource>().volume = volume;
                jump1.GetComponent<AudioSource>().pitch = pitch;
                jump1.GetComponent<AudioSource>().Play();
                break;
            case "jump2":
                jump2.GetComponent<AudioSource>().volume = volume;
                jump2.GetComponent<AudioSource>().pitch = pitch;
                jump2.GetComponent<AudioSource>().Play();
                break;
            case "jumpReady":
                jumpReady.GetComponent<AudioSource>().volume = volume;
                jumpReady.GetComponent<AudioSource>().pitch = pitch;
                jumpReady.GetComponent<AudioSource>().Play();
                break;
            case "landing":
                landing.GetComponent<AudioSource>().volume = volume;
                landing.GetComponent<AudioSource>().pitch = pitch;
                landing.GetComponent<AudioSource>().Play();
                break;
            case "breakFx":
                breakFx.GetComponent<AudioSource>().volume = volume;
                breakFx.GetComponent<AudioSource>().pitch = pitch;
                breakFx.GetComponent<AudioSource>().Play();
                break;
            case "failed":
                failed.GetComponent<AudioSource>().volume = volume;
                failed.GetComponent<AudioSource>().pitch = pitch;
                failed.GetComponent<AudioSource>().Play();
                break;
            case "flap":
                flap.GetComponent<AudioSource>().volume = volume;
                flap.GetComponent<AudioSource>().pitch = pitch;
                flap.GetComponent<AudioSource>().Play();
                break;
            case "walking":
                walking.GetComponent<AudioSource>().volume = volume;
                walking.GetComponent<AudioSource>().pitch = pitch;
                walking.GetComponent<AudioSource>().Play();
                break;
            case "acquire":
                acquire.GetComponent<AudioSource>().volume = volume;
                acquire.GetComponent<AudioSource>().pitch = pitch;
                acquire.GetComponent<AudioSource>().Play();
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

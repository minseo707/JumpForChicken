using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPlayManager : MonoBehaviour
{
    [Header("FX GameObjects")]
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
    public GameObject taxi;
    
    [Header("BGM Manager")]
    public GameObject bgmManager;

    [Header("Sliders")]
    public Slider totalVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider fxVolumeSlider;

    private AudioSource taxiAudioSource;
    private Dictionary<string, AudioSource> fxAudioSources = new Dictionary<string, AudioSource>();
    public Dictionary<string, AudioSource> bgmAudioSources = new Dictionary<string, AudioSource>();

    private List<float> taxiDistances = new List<float>();

    private void Start() 
    {
        // 설정 데이터 불러오기 및 슬라이더 초기화
        DataManager.Instance.LoadSettingsData();
        totalVolumeSlider.value = DataManager.Instance.settingsData.volumes[0];
        bgmVolumeSlider.value = DataManager.Instance.settingsData.volumes[1];
        fxVolumeSlider.value = DataManager.Instance.settingsData.volumes[2];

        taxiAudioSource = taxi.GetComponent<AudioSource>();

        // FX AudioSource 캐싱
        fxAudioSources["jump1"] = jump1.GetComponent<AudioSource>();
        fxAudioSources["jump2"] = jump2.GetComponent<AudioSource>();
        fxAudioSources["jumpReady"] = jumpReady.GetComponent<AudioSource>();
        fxAudioSources["landing"] = landing.GetComponent<AudioSource>();
        fxAudioSources["breakFx"] = breakFx.GetComponent<AudioSource>();
        fxAudioSources["failed"] = failed.GetComponent<AudioSource>();
        fxAudioSources["flap"] = flap.GetComponent<AudioSource>();
        fxAudioSources["tick"] = tick.GetComponent<AudioSource>();
        fxAudioSources["gaugeDisappear"] = gaugeDisappear.GetComponent<AudioSource>();
        fxAudioSources["walking"] = walking.GetComponent<AudioSource>();
        fxAudioSources["acquire"] = acquire.GetComponent<AudioSource>();

        // BGM AudioSource 캐싱 (bgmManager 자식 오브젝트)
        for (int i = 0; i < bgmManager.transform.childCount; i++)
        {
            string key = "bgm" + (i + 1);
            bgmAudioSources[key] = bgmManager.transform.GetChild(i).GetComponent<AudioSource>();
        }
    }

    public void PlaySound(string sound, float volume = 1f, float pitch = 1f)
    {
        float totalVol = totalVolumeSlider.value;
        // FX 사운드 우선 처리
        if (fxAudioSources.TryGetValue(sound, out AudioSource fxSource))
        {
            fxSource.volume = volume * totalVol * fxVolumeSlider.value;
            fxSource.pitch = pitch;
            fxSource.Play();
        }
        // BGM 사운드 처리
        else if (bgmAudioSources.TryGetValue(sound, out AudioSource bgmSource))
        {
            bgmSource.volume = volume * totalVol * bgmVolumeSlider.value;
            bgmSource.pitch = pitch;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("No sound clip found with the name: " + sound);
        }
    }

    public void SoundDistance(string sound, float distance)
    {
        if (sound == "taxi")
        {
            taxiDistances.Add(distance);
        }
    }

    private void Update()
    {
        if (taxiDistances.Count > 0)
        {
            float maxVolume = 0f;
            foreach (float d in taxiDistances)
            {
                if (d >= 0f && d <= 3f)
                {
                    maxVolume = 1f;
                }
                else if (d > 3f && d <= 8f)
                {
                    maxVolume = Mathf.Max(maxVolume, -1f / 5f * d + 8f / 5f);
                }
                else if (d >= -5f && d < 0f)
                {
                    maxVolume = Mathf.Max(maxVolume, 1f / 5f * d + 1f);
                }
                else
                {
                    maxVolume = Mathf.Max(maxVolume, 0f);
                }
            }
            taxiAudioSource.volume = maxVolume * totalVolumeSlider.value * fxVolumeSlider.value;
            taxiDistances.Clear();
        }
    }

    public void StopSound(string sound)
    {
        // FX 사운드 정지
        if (fxAudioSources.TryGetValue(sound, out AudioSource fxSource))
        {
            fxSource.Pause();
            return;
        }
        // BGM 사운드 정지
        if (bgmAudioSources.TryGetValue(sound, out AudioSource bgmSource))
        {
            bgmSource.Pause();
            return;
        }
        // 모든 BGM 정지
        if (sound == "allBgm")
        {
            foreach (var source in bgmAudioSources.Values)
            {
                source.Pause();
            }
            return;
        }
        Debug.LogWarning("Cannot Stop Sound: " + sound);
    }
}
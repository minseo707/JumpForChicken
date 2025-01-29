using UnityEngine;
using UnityEngine.UI;

public class SettingsDataController : MonoBehaviour
{
    public Slider totalVolumeSlider;
    public Slider bgmVolumeSlider;
    public Slider fxVolumeSlider;


    public void LoadSettingsDataForUI()
    {
        DataManager.Instance.LoadSettingsData();
        totalVolumeSlider.value = DataManager.Instance.settingsData.volumes[0];
        bgmVolumeSlider.value = DataManager.Instance.settingsData.volumes[1];
        fxVolumeSlider.value = DataManager.Instance.settingsData.volumes[2];
    }

    public void SaveSettingsDataForUI(){
        DataManager.Instance.settingsData.volumes[0] = totalVolumeSlider.value;
        DataManager.Instance.settingsData.volumes[1] = bgmVolumeSlider.value;
        DataManager.Instance.settingsData.volumes[2] = fxVolumeSlider.value;
        DataManager.Instance.SaveSettingsData();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsContainer;
    public GameObject realQuitContainer;
    public GameObject ThanksToContainer;
    public GameObject CreditContainer;
    private MainMusicManager mmm;

    private bool settingsUIEnabled = false;

    private bool isThanksToButtonStay = false;
    private float clickTime;

    void Start()
    {
        mmm = MainMusicManager.Instance;
        DataManager.Instance.LoadGameData();
    }

    private void Update(){
        if (clickTime >= 2f){ /* 클릭이 2초 이상 지속되면 */
            ThanksToContainer.SetActive(true);
            ThanksToButtonUp();
        }
        if (isThanksToButtonStay) clickTime += Time.deltaTime;
        
        // Escape가 눌리지 않았다면 탈출
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (settingsUIEnabled){
            SettingsCancelButton();
        } else {
            RealQuitButton();
        }
    }

    public void StartButton(){
        // 시작 시 무조건 컷신 실행
        SceneManager.LoadScene("StartCutScene");
    }

    public void SettingsButton(){
        settingsContainer.SetActive(true);
        settingsUIEnabled = true;
        ButtonSoundManager.Instance.PlayButtonSound("button1", 2f);
        mmm.MusicStop();
    }

    public void SettingsCancelButton(){
        settingsContainer.SetActive(false);
        settingsUIEnabled = false;
        ButtonSoundManager.Instance.PlayButtonSound("button1");
        mmm.MusicPlay();
    }

    void RealQuitButton(){
        realQuitContainer.SetActive(true);
        mmm.MusicStop();
    }

    public void RealQuitCancelButton(){
        realQuitContainer.SetActive(false);
        ButtonSoundManager.Instance.PlayButtonSound("button1");
        mmm.MusicPlay();
    }

    public void OnClickRealQuitButton(){
        Application.Quit();
    }

    public void OnClickShopButton(){
        ButtonSoundManager.Instance.PlayButtonSound("button1");
        SceneManager.LoadScene("ShopScene");
    }

    public void ThanksToButtonDown(){
        isThanksToButtonStay = true;
    }

    public void ThanksToButtonUp(){
        isThanksToButtonStay = false;
        clickTime = 0f;
    }

    public void ExitThanksTo(){
        ThanksToContainer.SetActive(false);
    }

    public void CreditButton(){
        CreditContainer.SetActive(true);
        ButtonSoundManager.Instance.PlayButtonSound("button1");
    }

    public void CreditCancelButton(){
        CreditContainer.SetActive(false);
        ButtonSoundManager.Instance.PlayButtonSound("button1");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 상점 입퇴장 관리
/// </summary>
public class ShopButtonManager : MonoBehaviour
{
    public GameObject soundManager;
    private ShopSoundPlayManager shopSoundManager;

    void Start(){
        shopSoundManager = soundManager.GetComponent<ShopSoundPlayManager>();
    }

    private void Update(){
        // Escape가 눌리지 않았다면 탈출
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        OnclickBackwardButton();
    }

    public void OnclickBackwardButton(){
        ButtonSoundManager.Instance.PlayButtonSound("button1");
        SceneManager.LoadScene("MainTitle");
    }
}

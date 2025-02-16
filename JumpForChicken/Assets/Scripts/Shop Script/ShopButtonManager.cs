using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 상점 입퇴장 관리
/// </summary>
public class ShopButtonManager : MonoBehaviour
{
    private void Update(){
        // Escape가 눌리지 않았다면 탈출
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        OnclickBackwardButton();
    }

    public void OnclickBackwardButton(){
        SceneManager.LoadScene("MainTitle");
    }
}

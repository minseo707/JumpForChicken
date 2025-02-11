using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseButtonManager : MonoBehaviour
{
    /// <summary>
    /// 활성화
    /// 비활성화
    /// 폰트 컬러 빨간색
    /// 폰트 컬러 초록색
    /// 폰트 컬러 하얀색
    /// </summary>
    /// 

    private Button button;
    
    public GameObject priceText;
    private TextMeshProUGUI priceTextTMP;

    private void Awake() {
        button = GetComponent<Button>();
        priceTextTMP = priceText.GetComponent<TextMeshProUGUI>();
    }

    public void ActivateButton(){
        button.interactable = true;
    }

    public void DeactivateButton(){
        button.interactable = false;
    }

    public void ChangeTextColor(string colorText = "white"){
        switch (colorText) {
            case "red":
                // 폰트 컬러 빨간색 255 84 108
                priceTextTMP.color = new Color32(255, 84, 108, 255);
                break;
            case "green":
                // 폰트 컬러 초록색 85 255 85
                priceTextTMP.color = new Color32(85, 255, 85, 255);
                break;
            case "white":
                // 폰트 컬러 하얀색 255 249 230
                priceTextTMP.color = new Color32(255, 249, 230, 255);
                break;
            case "gray":
                priceTextTMP.color = new Color32(128, 128, 128, 255);
                break;
            default:
                Debug.LogError("Unknown color text: " + colorText);
                break;
        }
    }

    public void ChangePriceText(int price){
        if (price < 0){
            priceTextTMP.text = "x ___";
            return;
        }
        priceTextTMP.text = $"x {price}";
    }
}

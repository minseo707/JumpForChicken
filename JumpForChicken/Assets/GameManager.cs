using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject platformData;

    public TextMeshProUGUI daText;
    public TextMeshProUGUI mtText;
    public TextMeshProUGUI mmText;
    public TextMeshProUGUI sdText;
    public TextMeshProUGUI myText;
    public TextMeshProUGUI timeText;

    float time;

    private void Start() {
        platformData = GameObject.Find("Platform");
        time = 0f;
    }

    private void Update() {
        
        daText.text = $"제외 범위: {platformData.GetComponent<PlatformGenerator>().declineArea}";
        mtText.text = $"최소 타일: {platformData.GetComponent<PlatformGenerator>().minTile}";
        mmText.text = $"최소 이동: {platformData.GetComponent<PlatformGenerator>().minMove}";
        sdText.text = $"좌우 여백: {platformData.GetComponent<PlatformGenerator>().sideDecline}";
        myText.text = $"최소 높이: {platformData.GetComponent<PlatformGenerator>().minYSelect}";
        time += Time.deltaTime;
        timeText.text = $"플레이 타임: {Mathf.Floor(time*100f)*0.01f}s";
    }

    public void OnClickRestart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.LogError($"{platformData.GetComponent<PlatformGenerator>().declineArea} {platformData.GetComponent<PlatformGenerator>().minTile} {platformData.GetComponent<PlatformGenerator>().minMove} {platformData.GetComponent<PlatformGenerator>().sideDecline} {platformData.GetComponent<PlatformGenerator>().minYSelect}");
    }

    public void OnClickDADown(){
        platformData.GetComponent<PlatformGenerator>().DADown();
        daText.text = $"제외 범위: {platformData.GetComponent<PlatformGenerator>().declineArea}";
    }

    public void OnClickDAUp(){
        platformData.GetComponent<PlatformGenerator>().DAUp();
        daText.text = $"제외 범위: {platformData.GetComponent<PlatformGenerator>().declineArea}";
    }

    public void OnClickMTDown(){
        platformData.GetComponent<PlatformGenerator>().MTDown();
        mtText.text = $"최소 타일: {platformData.GetComponent<PlatformGenerator>().minTile}";
    }

    public void OnClickMTUp(){
        platformData.GetComponent<PlatformGenerator>().MTUp();
        mtText.text = $"최소 타일: {platformData.GetComponent<PlatformGenerator>().minTile}";

    }

    public void OnClickMMDown(){
        platformData.GetComponent<PlatformGenerator>().MMDown();
        mmText.text = $"최소 이동: {platformData.GetComponent<PlatformGenerator>().minMove}";
    }

    public void OnClickMMUp(){
        platformData.GetComponent<PlatformGenerator>().MMUp();
        mmText.text = $"최소 이동: {platformData.GetComponent<PlatformGenerator>().minMove}";
    }

    public void OnClickSDDown(){
        platformData.GetComponent<PlatformGenerator>().SDDown();
        sdText.text = $"좌우 여백: {platformData.GetComponent<PlatformGenerator>().sideDecline}";
    }

    public void OnClickSDUp(){
        platformData.GetComponent<PlatformGenerator>().SDUp();
        sdText.text = $"좌우 여백: {platformData.GetComponent<PlatformGenerator>().sideDecline}";
    }

    public void OnClickMYDown(){
        platformData.GetComponent<PlatformGenerator>().MYDown();
        myText.text = $"최소 높이: {platformData.GetComponent<PlatformGenerator>().minYSelect}";
    }

    public void OnClickMYUp(){
        platformData.GetComponent<PlatformGenerator>().MYUp();
        myText.text = $"최소 높이: {platformData.GetComponent<PlatformGenerator>().minYSelect}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstGameUpdater : MonoBehaviour
{
    public GameObject gameoverUI;
    public GameObject canvas; 

    GameObject resetButton;

    public bool isGameover = false;

    private static FirstGameUpdater _instance;

    public static FirstGameUpdater Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에 FirstGame가 없다면, 하나를 생성
                GameObject go = new GameObject("FirstGame");
                _instance = go.AddComponent<FirstGameUpdater>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        // Singleton 인스턴스가 이미 존재하는 경우, 새로운 것을 파괴
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 초기화 코드
        InitializeGame();
    }

    void InitializeGame()
    {
        // 초기화 작업
        PlayerPrefs.SetFloat("declineArea", 3f);
        PlayerPrefs.SetFloat("sideDecline", 2.0f);
        PlayerPrefs.SetFloat("minYSelect", 2.0f);
        PlayerPrefs.SetFloat("minMove", 16f);
        PlayerPrefs.SetInt("minTile", 4);
    }

    private void Update() {

    }
}

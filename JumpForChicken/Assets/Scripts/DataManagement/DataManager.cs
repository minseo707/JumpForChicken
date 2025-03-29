using System.IO;
using UnityEngine;

/// <summary>
/// 데이터 저장/로드 관리자
/// : 첫 실행 시 Imstance에 접근
/// </summary>
public class DataManager : MonoBehaviour
{
    static GameObject container;
    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new();
                container.name = "DataManager";
                instance = container.AddComponent<DataManager>();
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    // --- 게임 데이터 파일명을 설정 ("원하는 이름(영문).json") ---
    readonly string gameDataFileName = "GameData.json";
    readonly string settingsDataFileName = "SettingsData.json";

    // --- 저장용 클래스 변수 ---
    public GameData gameData = new GameData();
    public SettingsData settingsData = new SettingsData();

    // GameData 불러오기
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일을 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string fromJsonData = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(fromJsonData);
            Debug.Log("[DataManager] GameData Loaded");
        }
    }

    // GameData 저장하기
    public void SaveGameData()
    {
        // 클래스를 Json 형식으로 전환 (true: 가독성 좋게 작성)
        string toJsonData = JsonUtility.ToJson(gameData, true);
        string filePath = Application.persistentDataPath + "/" + gameDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, toJsonData);

        Debug.Log("[DataManager] GameData Saved");
    }

    public void ResetGameData(){
        gameData = new GameData();
        Debug.Log("[DataManager] GameData Reseted");
        SaveGameData();
    }

    // SettingsData 불러오기
    // GameData 불러오기
    public void LoadSettingsData()
    {
        string filePath = Application.persistentDataPath + "/" + settingsDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일을 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string fromJsonData = File.ReadAllText(filePath);
            settingsData = JsonUtility.FromJson<SettingsData>(fromJsonData);
            Debug.Log("[DataManager] SettingsData Loaded");
        }
    }

    // GameData 저장하기
    public void SaveSettingsData()
    {
        // 클래스를 Json 형식으로 전환 (true: 가독성 좋게 작성)
        string toJsonData = JsonUtility.ToJson(settingsData, true);
        string filePath = Application.persistentDataPath + "/" + settingsDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, toJsonData);

        Debug.Log("[DataManager] SettingsData Saved");
    }

    public void ResetSettingsData(){
        settingsData = new SettingsData();
        Debug.Log("[DataManager] SettingsData Reseted");
        SaveSettingsData();
    }
}
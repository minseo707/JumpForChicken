using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileLoader : MonoBehaviour
{
    public GameObject[] RandomTiles { get; private set; } // 외부에서 접근할 수 있는 랜덤 프리팹 배열

    public int tileCount = 70; // 타일 개수

    public void LoadRandomTiles()
    {
        string folderPath = "Assets/Resources/Prefabs/Tiles/CityTiles"; // 프리팹이 있는 폴더 경로
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        GameObject[] allPrefabs = new GameObject[prefabGUIDs.Length];

        // 모든 프리팹을 배열에 로드
        for (int i = 0; i < prefabGUIDs.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
            allPrefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }

        // 랜덤 프리팹을 저장할 리스트
        List<GameObject> selectedPrefabs = new List<GameObject>();

        // 설정된 횟수만큼 반복하여 랜덤 프리팹 선택 (임시로 무작위 랜덤)
        for (int i = 0; i < tileCount; i++)
        {
            int randomIndex = Random.Range(0, allPrefabs.Length);
            GameObject selectedPrefab = allPrefabs[randomIndex];

            if (selectedPrefab != null)
            {
                selectedPrefabs.Add(selectedPrefab);
            }
        }

        // 배열로 변환하여 외부에서 사용할 수 있도록 저장
        RandomTiles = selectedPrefabs.ToArray();
        Debug.Log("Random prefabs loaded. Total count: " + RandomTiles.Length);
    }
}
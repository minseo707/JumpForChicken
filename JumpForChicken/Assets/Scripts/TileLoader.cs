using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PrefabName;

/// <summary>
/// 블록 선택 클래스 (가중치 적용)
/// </summary>
public class TileLoader : MonoBehaviour
{
    private readonly string[] folderPathsEditer = {
        "Assets/Resources/Prefabs/Tiles/CityTiles",
        "Assets/Resources/Prefabs/Tiles/MountainTiles",
        "Assets/Resources/Prefabs/Tiles/SkyTiles",
        "Assets/Resources/Prefabs/Tiles/SpaceTiles"
    };

    private readonly string[] folderPathsBuild = {
        "Prefabs/Tiles/CityTiles",
        "Prefabs/Tiles/MountainTiles",
        "Prefabs/Tiles/SkyTiles",
        "Prefabs/Tiles/SpaceTiles"
    };

    private readonly GameObject[][] allPrefabArray = { new GameObject[] {},
                                                       new GameObject[] {},
                                                       new GameObject[] {},
                                                       new GameObject[] {} };

    private GameObject[] allLastPrefab = {};

    private readonly float[][][] tileTypeWeight = { new float[][] {},
                                                    new float[][] {},
                                                    new float[][] {},
                                                    new float[][] {} };
    private readonly int[][] numberOfTileTypes = { new int[] {8, 8, 8, 8, 8, 8, 8, 8, 8, 8},
                                                   new int[] {8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8},
                                                   new int[] {1, 1, 2, 1, 1, 2},
                                                   new int[] {7, 4, 1, 1} }; // 각 타일 종류별 개수
    private readonly string[][] tileTypeCodes = { new string[] {"F2", "F3", "F4", "F5", "F5_L", "F6", "F6_L", "T2", "T3", "T4"},
                                                  new string[] {"F1", "F2", "F3", "F4", "F0", "F0_L", "F5", "F5_L", "F6", "F6_L", 
                                                                "T2", "T2_R", "T3", "T3_R", "T4", "T4_R", "T5", "T5_R"},
                                                  new string[] {"F1", "F2", "F3", "T1", "T2", "T3"},
                                                  new string[] {"F1", "F2", "T1", "F2"} }; // 각 타일 종류 코드


    /// <summary>
    /// 다음 랜덤 블럭을 반환하는 함수
    /// </summary>
    /// <param name="loadStage">블럭 로드 조건: 스테이지</param>
    /// <returns>랜덤으로 불어온 블럭 프리팹</returns>
    public GameObject GetNextBlock(int loadStage = 1){
        int[] blockIndex = RandomTile(Random.Range(0f, SumWeights(loadStage)), loadStage);
        AdjustWeights(blockIndex, loadStage);

        if (loadStage == 2 && blockIndex[0] >= 10 && blockIndex[0] <= 17){
            blockIndex[1] = 0;
        }

        string tileType = tileTypeCodes[loadStage - 1][blockIndex[0]];

        string prefabName = PrefabNameTranslator.ToPrefabName(loadStage, tileType, blockIndex[1] + 1);

        GameObject selectedPrefab = System.Array.Find(allPrefabArray[loadStage - 1], prefab => prefab != null && prefab.name == prefabName);

        // 예외 처리
        if (selectedPrefab != null) Debug.Log($"[TileLoader] 선택된 프리팹: {selectedPrefab.name}");
        else Debug.LogWarning($"[TileLoader] 프리팹을 찾을 수 없습니다: {prefabName}");

        return selectedPrefab;
    }

    public GameObject GetLastBlock(int loadStage){
        return loadStage switch
        {
            1 => NameToPrefab("cityBlock_LastBlock", true),
            2 => NameToPrefab("mountainBlock_LastBlock", true),
            3 => NameToPrefab("skyBlock_LastBlock", true),
            _ => new GameObject(),
        };
    }

    /// <summary>
    /// 가중치를 초기화하는 함수 (처음 실행 시 실행)
    /// </summary>
    private void InitializeWeights()
    {
        try
        {
            for (int s = 0; s < 4; s++)
            {
                tileTypeWeight[s] = new float[numberOfTileTypes[s].Length][];
                for (int i = 0; i < numberOfTileTypes[s].Length; i++)
                {
                    tileTypeWeight[s][i] = new float[numberOfTileTypes[s][i]];
                    for (int j = 0; j < numberOfTileTypes[s][i]; j++)
                    {
                        tileTypeWeight[s][i][j] = 1f;
                    }
                }
            }

            Debug.Log("가중치 초기화 성공.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"가중치 초기화 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 존재하는 모든 프리팹을 불러와 2차원 배열 allPrefabArray에 저장하는 함수
    /// </summary>
    public void LoadAllPrefabs(){
        for (int s = 0; s < 4; s++)
        {
            #if UNITY_EDITOR
            /* 에디터 환경에서 프리팹을 불러옴 */
            string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPathsEditer[s] });
            allPrefabArray[s] = new GameObject[prefabGUIDs.Length];

            for (int i = 0; i < prefabGUIDs.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
                allPrefabArray[s][i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }

            #else
            /* 빌드 환경에서 프리팹을 불러옴 */
            allPrefabArray[s] = Resources.LoadAll<GameObject>(folderPathsBuild[s]);

            #endif
        }

        #if UNITY_EDITOR

        string[] prefabGUIDs2 = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Tiles/LastBlocks" });
        allLastPrefab = new GameObject[prefabGUIDs2.Length];

        for (int i = 0; i < prefabGUIDs2.Length; i++)
        {
            string prefabPath2 = AssetDatabase.GUIDToAssetPath(prefabGUIDs2[i]);
            allLastPrefab[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath2);
        }

        #else

        allLastPrefab = Resources.LoadAll<GameObject>("Prefabs/Tiles/LastBlocks");

        #endif

        Debug.Log($"{allPrefabArray[0].Length + allPrefabArray[1].Length + allPrefabArray[2].Length + allPrefabArray[3].Length}개의 프리팹을 로드했습니다.");

        InitializeWeights();
    }


    /// <summary>
    /// 편향적 랜덤 값을 모두 더하는 함수
    /// </summary>
    /// <param name="loadStage">스테이지</param>
    /// <returns>편향적 랜덤 값의 합</returns>
    private float SumWeights(int loadStage = 1)
    {
        if (tileTypeWeight[loadStage - 1] == null)
        {
            Debug.LogError("tileTypeWeight가 null입니다. 가중치를 재초기화합니다.");
            InitializeWeights();
            if (tileTypeWeight[loadStage - 1] == null)
            {
                Debug.LogError("가중치 재초기화 실패.");
                return 0;
            }
        }

        float sum = 0;
        for (int i = 0; i < tileTypeWeight[loadStage - 1].Length; i++)
        {
            if (tileTypeWeight[loadStage - 1][i] == null)
            {
                Debug.LogError($"tileTypeWeight[{loadStage - 1}][{i}]가 null입니다");
                continue;
            }
            for (int j = 0; j < tileTypeWeight[loadStage - 1][i].Length; j++)
            {
                sum += Mathf.Pow(tileTypeWeight[loadStage - 1][i][j], 4);
            }
        }
        return sum;
    }

    /// <summary>
    /// 랜덤 값에 대응되는 블럭의 2차원 좌표를 반환하는 함수
    /// </summary>
    /// <param name="rScore">편향적 랜덤 값의 합</param>
    /// <param name="loadStage">스테이지</param>
    /// <returns>블럭의 2차원 좌표</returns>
    private int[] RandomTile(float rScore, int loadStage = 1)
    {
        float sum = 0;
        for (int i = 0; i < tileTypeWeight[loadStage - 1].Length; i++)
        {
            for (int j = 0; j < tileTypeWeight[loadStage - 1][i].Length; j++)
            {
                sum += Mathf.Pow(tileTypeWeight[loadStage - 1][i][j], 4);
                if (sum > rScore)
                {
                    return new int[] { i, j };
                }
            }
        }
        // 예외 처리
        return new int[] { tileTypeWeight[loadStage - 1].Length - 1, tileTypeWeight[loadStage - 1][^1].Length - 1 };
    }

    /// <summary>
    /// 타일들의 가중치를 조정하는 함수
    /// </summary>
    /// <param name="blockIndex">선택된 블록의 2차원 좌표</param>
    /// <param name="loadStage"스테이지></param>
    private void AdjustWeights(int[] blockIndex, int loadStage = 1)
    {
        int i = blockIndex[0];
        int j = blockIndex[1];

        if (loadStage == 1){
            // 같은 종류(타일 수 & L자 타일 여부 & 통과 가능 여부) 블록 가중치 조정
            for (int k = 0; k < tileTypeWeight[loadStage - 1][i].Length; k++)
            {
                tileTypeWeight[loadStage - 1][i][k] -= 0.42f;
            }

            // 통과 가능 여부가 같은 블록 (통과 불가능) 가중치 조정
            if (i <= 6)
            {
                for (int k = 0; k <= 6; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.14f;
                    }
                }
            }
            // 통과 가능 여부가 같은 블록 (통과 가능) 가중치 조정
            else
            {
                for (int k = 7; k <= 9; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.29f;
                    }
                }
            }

            // L자 모양 타일의 가중치 조정
            if (i >= 3 && i <= 6)
            {
                for (int k = 3; k <= 6; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.28f;
                    }
                }
            }

            // 모든 타일 가중치 증가
            for (int k = 0; k < tileTypeWeight[loadStage - 1].Length; k++)
            {
                for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                {
                    tileTypeWeight[loadStage - 1][k][l] += 0.168f;
                }
            }
        } else if (loadStage == 2){
            // 같은 종류(타일 수 & L자 타일 여부 & 통과 가능 여부) 블록 가중치 조정
            for (int k = 0; k < tileTypeWeight[loadStage - 1][i].Length; k++)
            {
                tileTypeWeight[loadStage - 1][i][k] -= 0.52f;
            }

            // 통과 가능 여부가 같은 블록 (통과 불가능) 가중치 조정
            if (i <= 9)
            {
                for (int k = 0; k <= 9; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.10f;
                    }
                }
            }
            // 통과 가능 여부가 같은 블록 (통과 가능) 가중치 조정
            else
            {
                for (int k = 10; k <= 17; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.32f;
                    }
                }
            }

            // L자 모양 타일의 가중치 조정
            if (i >= 4 && i <= 9)
            {
                for (int k = 4; k <= 9; k++)
                {
                    for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                    {
                        tileTypeWeight[loadStage - 1][k][l] -= 0.28f;
                    }
                }
            }
            
            // 한 칸 블록 선택 시 추가 감소
            if (i == 0){
                for (int k = 0; k < tileTypeWeight[loadStage - 1][0].Length; k++)
                {
                    tileTypeWeight[loadStage - 1][0][k] -= 0.5f;
                }
            }

            // 모든 타일 가중치 증가
            for (int k = 0; k < tileTypeWeight[loadStage - 1].Length; k++)
            {
                for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
                {
                    tileTypeWeight[loadStage - 1][k][l] += 0.168f;
                }
            }
        }

        // 바로 직전에 (마지막으로) 선택된 블록의 가중치를 0으로 설정
        tileTypeWeight[loadStage - 1][i][j] = 0;

        // 음수 가중치 방지
        for (int k = 0; k < tileTypeWeight[loadStage - 1].Length; k++)
        {
            for (int l = 0; l < tileTypeWeight[loadStage - 1][k].Length; l++)
            {
                tileTypeWeight[loadStage - 1][k][l] = Mathf.Max(0, tileTypeWeight[loadStage - 1][k][l]);
            }
        }
    }

    /// <summary>
    /// 프리팹 이름으로 프리팹을 반환하는 함수
    /// </summary>
    /// <param name="prefabName">프리팹 이름</param>
    /// <returns>추가 선택된 프리팹</returns>
    public GameObject NameToPrefab(string prefabName, bool isLastBlock = false){
        if (!isLastBlock){
            GameObject selectedPrefab = System.Array.Find(allPrefabArray[
                PrefabNameTranslator.ToPrefabAttribute(prefabName)[0] - 1
            ], prefab => prefab != null && prefab.name == prefabName);

            if (selectedPrefab != null)
            {
                Debug.Log($"[TileLoader] 추가 선택된 프리팹: {selectedPrefab.name}");
            }
            else
            {
                Debug.LogWarning($"[TileLoader] 프리팹을 찾을 수 없습니다: {prefabName}");
            }
            return selectedPrefab;
        } else {
            GameObject selectedPrefab = System.Array.Find(allLastPrefab, prefab => prefab != null && prefab.name == prefabName);

            if (selectedPrefab != null)
            {
                Debug.Log($"[TileLoader] 추가 선택된 프리팹: {selectedPrefab.name}");
            }
            else
            {
                Debug.LogWarning($"[TileLoader] 프리팹을 찾을 수 없습니다: {prefabName}");
            }
            return selectedPrefab;
        }
    }
}
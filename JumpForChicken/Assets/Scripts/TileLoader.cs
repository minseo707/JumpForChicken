using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileLoader : MonoBehaviour
{
    public GameObject[] RandomTiles { get; private set; } // 외부에서 접근할 수 있는 랜덤 프리팹 배열
    public int tileCount = 70; // 타일 개수

    public float typeWeight1 = 0.42f;
    public float typeWeight2 = 0.14f;
    public float typeWeight3 = 0.29f;
    public float typeWeight4 = 0.28f;
    public float typeWeight5 = 0.168f;

    private float[][] tileTypeWeight;
    private int[] numberOfTileTypes = { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 }; // 각 타일 종류별 개수
    private string[] tileTypeCodes = { "F2", "F3", "F4", "F5", "F5_L", "F6", "F6_L", "T2", "T3", "T4" }; // 각 타일 종류 코드

    private void Awake()
    {
        // InitializeWeights();
        // Loading Scene에서 실행합니다.
    }

    private void Start()
    {
        // LoadRandomTiles();
        // PrintRandomTiles();
        // Loading Scene에서 실행합니다.
    }

    private void InitializeWeights()
    {
        try
        {
            tileTypeWeight = new float[numberOfTileTypes.Length][];
            for (int i = 0; i < numberOfTileTypes.Length; i++)
            {
                tileTypeWeight[i] = new float[numberOfTileTypes[i]];
                for (int j = 0; j < numberOfTileTypes[i]; j++)
                {
                    tileTypeWeight[i][j] = 1f;
                }
            }
            Debug.Log("가중치 초기화 성공.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"가중치 초기화 중 오류 발생: {e.Message}");
        }
    }

    private float SumWeights()
    {
        if (tileTypeWeight == null)
        {
            Debug.LogError("tileTypeWeight가 null입니다. 가중치를 재초기화합니다.");
            InitializeWeights();
            if (tileTypeWeight == null)
            {
                Debug.LogError("가중치 재초기화 실패.");
                return 0;
            }
        }

        float sum = 0;
        for (int i = 0; i < tileTypeWeight.Length; i++)
        {
            if (tileTypeWeight[i] == null)
            {
                Debug.LogError($"tileTypeWeight[{i}]가 null입니다");
                continue;
            }
            for (int j = 0; j < tileTypeWeight[i].Length; j++)
            {
                sum += Mathf.Pow(tileTypeWeight[i][j], 4);
            }
        }
        return sum;
    }

    private int[] RandomTile(float rScore)
    {
        float sum = 0;
        for (int i = 0; i < tileTypeWeight.Length; i++)
        {
            for (int j = 0; j < tileTypeWeight[i].Length; j++)
            {
                sum += Mathf.Pow(tileTypeWeight[i][j], 4);
                if (sum > rScore)
                {
                    return new int[] { i, j };
                }
            }
        }
        return new int[] { tileTypeWeight.Length - 1, tileTypeWeight[tileTypeWeight.Length - 1].Length - 1 };
    }

    private void AdjustWeights(int[] blockIndex)
    {
        int i = blockIndex[0];
        int j = blockIndex[1];

        // 같은 종류(타일 수 & L자 타일 여부 & 통과 가능 여부) 블록 가중치 조정
        for (int k = 0; k < tileTypeWeight[i].Length; k++)
        {
            tileTypeWeight[i][k] -= typeWeight1; // 0.42f
        }

        // 통과 가능 여부가 같은 블록 (통과 불가능) 가중치 조정
        if (i <= 6)
        {
            for (int k = 0; k <= 6; k++)
            {
                for (int l = 0; l < tileTypeWeight[k].Length; l++)
                {
                    tileTypeWeight[k][l] -= typeWeight2; // 0.14f
                }
            }
        }
        // 통과 가능 여부가 같은 블록 (통과 가능) 가중치 조정
        else
        {
            for (int k = 7; k <= 9; k++)
            {
                for (int l = 0; l < tileTypeWeight[k].Length; l++)
                {
                    tileTypeWeight[k][l] -= typeWeight3; // 0.29f
                }
            }
        }

        // L자 모양 타일의 가중치 조정
        if (i >= 3 && i <= 6)
        {
            for (int k = 3; k <= 6; k++)
            {
                for (int l = 0; l < tileTypeWeight[k].Length; l++)
                {
                    tileTypeWeight[k][l] -= typeWeight4; // 0.28f
                }
            }
        }

        // 모든 타일 가중치 증가
        for (int k = 0; k < tileTypeWeight.Length; k++)
        {
            for (int l = 0; l < tileTypeWeight[k].Length; l++)
            {
                tileTypeWeight[k][l] += typeWeight5; // 0.168f
            }
        }

        // 바로 직전에 (마지막으로) 선택된 블록의 가중치를 0으로 설정
        tileTypeWeight[i][j] = 0;

        // 음수 가중치 방지
        for (int k = 0; k < tileTypeWeight.Length; k++)
        {
            for (int l = 0; l < tileTypeWeight[k].Length; l++)
            {
                tileTypeWeight[k][l] = Mathf.Max(0, tileTypeWeight[k][l]);
            }
        }
    }

    public GameObject[] allPrefabs;

    public GameObject NameToPrefab(string prefabName){
        GameObject selectedPrefab = System.Array.Find(allPrefabs, prefab => prefab != null && prefab.name == prefabName);

        if (selectedPrefab != null)
        {
            Debug.Log($"추가 선택된 프리팹: {selectedPrefab.name}");
        }
        else
        {
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {prefabName}");
        }
        return selectedPrefab;
    }

    public void LoadRandomTiles()
{
    List<GameObject> selectedPrefabs = new List<GameObject>();

#if UNITY_EDITOR
    // 에디터 환경에서 AssetDatabase 사용
    string folderPath = "Assets/Resources/Prefabs/Tiles/CityTiles"; // 프리팹이 있는 폴더 경로
    string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
    allPrefabs = new GameObject[prefabGUIDs.Length];

    for (int i = 0; i < prefabGUIDs.Length; i++)
    {
        string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
        allPrefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    Debug.Log($"{allPrefabs.Length}개의 프리팹을 로드했습니다. (에디터 환경)");
#else
    // 빌드 환경에서 Resources.Load 사용
    allPrefabs = Resources.LoadAll<GameObject>("Prefabs/Tiles/CityTiles");
    Debug.Log($"{allPrefabs.Length}개의 프리팹을 로드했습니다. (빌드 환경)");
#endif

    InitializeWeights();

    // 가중치를 적용하여 프리팹 선택
    for (int i = 0; i < tileCount; i++)
    {
        float rScore = Random.Range(0f, SumWeights());
        int[] blockIndex = RandomTile(rScore);
        AdjustWeights(blockIndex);

        string tileType = tileTypeCodes[blockIndex[0]];
        string prefabName = tileType.EndsWith("L")  // 종류가 L로 끝나는 거 구분
            ? $"cityBlock_{tileType}{blockIndex[1] + 1}"
            : $"cityBlock_{tileType}_{blockIndex[1] + 1}";

        GameObject selectedPrefab = System.Array.Find(allPrefabs, prefab => prefab != null && prefab.name == prefabName);

        if (selectedPrefab != null)
        {
            selectedPrefabs.Add(selectedPrefab);
            Debug.Log($"선택된 프리팹: {selectedPrefab.name}");
        }
        else
        {
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {prefabName}");
        }
    }

    // 배열로 변환하여 외부에서 사용할 수 있도록 저장
    RandomTiles = selectedPrefabs.ToArray();
    Debug.Log($"총 선택된 프리팹 수: {RandomTiles.Length}");
    PrintRandomTiles();
}

    private void PrintRandomTiles()
    {
        Debug.Log("랜덤 타일:");
        if (RandomTiles != null)
        {
            for (int i = 0; i < RandomTiles.Length; i++)
            {
                if (RandomTiles[i] != null)
                {
                    Debug.Log($"{i + 1}: {RandomTiles[i].name}");
                }
                else
                {
                    Debug.LogWarning($"{i + 1}: Null 프리팹");
                }
            }
        }
        else
        {
            Debug.LogError("RandomTiles 배열이 null입니다");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임에서의 블록 저장소 (Class: PlatformStateManager)
/// </summary>
public class BlockStore : MonoBehaviour
{

    private static int currentPlatformLayer;
    private static int nextPlatformLayer;
    private static int pastPlatformLayer;
    private static int defaultPlatformLayer;

    public static List<PlatformStateManager> blocks = new();

    private void Awake()
    {
        currentPlatformLayer = LayerMask.NameToLayer("CurrentPlatform");
        nextPlatformLayer = LayerMask.NameToLayer("NextPlatform");
        pastPlatformLayer = LayerMask.NameToLayer("PastPlatform");
        defaultPlatformLayer = LayerMask.NameToLayer("DefaultPlatform");
    }

    /// <summary>
    /// 모든 블록 레이어 변경
    /// </summary>
    /// <param name="blockIndex">CurrentPlatform Index</param>
    public void ChangeBlockStateLayer(int blockIndex){
        for (int i = 0; i < blocks.Count; i++)
        {
            if (i == blockIndex){
                blocks[i].SetOnlyPlatformLayer(currentPlatformLayer);
            } else if (i == blockIndex + 1){
                blocks[i].SetOnlyPlatformLayer(nextPlatformLayer);
            } else if (i > blockIndex + 1){
                blocks[i].SetOnlyPlatformLayer(defaultPlatformLayer);
            } else if (i < blockIndex) {
                blocks[i].SetOnlyPlatformLayer(pastPlatformLayer);
            }

            if(blocks[i] != null)
            {
                blocks[i].SetPlatformLayer();
            }
            else
            {
                Debug.LogWarning("[BlockStore] blocks[" + i + "]가 null입니다.");
            }
        }
    }

    internal static void ResetBlockStore(){
        blocks = new();
    }

    /// <summary>
    /// 프리팹의 PlatformStageManager 추가
    /// </summary>
    public static void AddPrefab(PlatformStateManager psm){
        blocks.Add(psm);
    }

    /// <summary>
    /// 다음 스테이지로 이동하기 위한 테스트 코드
    /// </summary>
    internal static void SetAllNext(){
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].SetOnlyPlatformLayer(currentPlatformLayer);
            blocks[i].SetPlatformLayer();
        }
    }
}

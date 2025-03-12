using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 튜토리얼에서의 블록 저장소 (테스트 과정)
/// </summary>
public class TutorialBlockStore : MonoBehaviour
{
    public List<GameObject> blockGameObjects = new List<GameObject>();
    public List<TutorialPlatformStateManager> blocks = new List<TutorialPlatformStateManager>();

    void Awake()
    {
        // 에디터 환경에서 드래그 앤 드랍으로 불가능한 조작 -> 스크립트에서 조작
        for (int i = 0; i < blockGameObjects.Count; i++) blocks.Add(blockGameObjects[i].GetComponent<TutorialPlatformStateManager>());

        Debug.Log("[TutorialBlockStore] Block Count: " + blocks.Count);
    }

    void Start(){
        ChangeBlockStateLayer(0);
    }
    

    /// <summary>
    /// 모든 블록 레이어 변경
    /// </summary>
    /// <param name="blockIndex">CurrentPlatform Index</param>
    public void ChangeBlockStateLayer(int blockIndex){
        Debug.Log("[TutorialBlockStore] ChangeBlockStateLayer Run for: " + blockIndex);
        for (int i = 0; i < blocks.Count; i++)
        {
            if (i == blockIndex){
                blockGameObjects[i].layer = LayerMask.NameToLayer("CurrentPlatform");
            } else if (i == blockIndex + 1){
                blockGameObjects[i].layer = LayerMask.NameToLayer("NextPlatform");
            } else if (i > blockIndex + 1){
                blockGameObjects[i].layer = LayerMask.NameToLayer("DefaultPlatform");
            } else if (i < blockIndex) {
                blockGameObjects[i].layer = LayerMask.NameToLayer("PastPlatform");
            }

            if(blocks[i] != null)
            {
                blocks[i].SetPlatformLayer();
            }
            else
            {
                Debug.LogWarning("[TutorialBlockStore] blocks[" + i + "]가 null입니다.");
            }
        }
    }
}

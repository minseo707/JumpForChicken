using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오토바이 스프라이트 관리
/// </summary>
public class BikeSpriteManager : MonoBehaviour
{
    /// <summary>
    /// 0: Default
    /// 1: Red1
    /// 2: White1
    /// 3: Red2
    /// 4: Black2
    /// </summary>
    public Sprite[] bikeSprite;
    
    private SpriteRenderer spriteRenderer;

    public float xScale = 1;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadBikeSprite();
    }

    public void LoadBikeSprite(bool dataLoad = true, int itemCode = 0){
        if (dataLoad == true){
            DataManager.Instance.LoadGameData();
            itemCode = DataManager.Instance.gameData.equippedGoods[2];
            spriteRenderer.sprite = bikeSprite[itemCode];
        } else {
            spriteRenderer.sprite = bikeSprite[itemCode];
        }
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
    }
}

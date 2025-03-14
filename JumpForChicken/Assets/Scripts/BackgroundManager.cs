using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 배경 높이 (시차) 관리자
/// </summary>
public class BackgroundManager : MonoBehaviour
{
    // 카메라 좌표를 불러오기 위함
    public GameObject cameras;

    public float scope = 13f;

    public List<Sprite> backgroundTextures;

    private float[] scopeArray = {13f, 13f, 13f, 13f};

    private float[] offsets = {0, 19.5f, 35.3f, 360f};

    private float[] realHeights = {120f, 170f, 220f, 400f};

    private SpriteRenderer spriteRenderer;

    int stageNumber = 1;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = backgroundTextures[0];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameras.transform.position.x, (realHeights[stageNumber - 1] - 24f)/realHeights[stageNumber - 1]*(cameras.transform.position.y-1) + 13 + offsets[stageNumber - 1], transform.position.z);
    }

    public void ChangeBackgroundSprite(int stage){
        spriteRenderer.sprite = backgroundTextures[stage - 1];
        scopeArray[stage - 1] = scope;
        stageNumber = stage;
    }
}
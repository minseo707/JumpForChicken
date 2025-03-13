using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    // 카메라 좌표를 불러오기 위함
    public GameObject cameras;

    public float scope = 13f;

    public List<Sprite> backgroundTextures;

    private float[] scopeArray = {13f, 13f, 13f, 13f};

    private float[] offsets = {0, 27f, 54f, 360f};

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
        transform.position = new Vector3(cameras.transform.position.x, 4*(cameras.transform.position.y-1)/5 + 13 + offsets[stageNumber - 1], transform.position.z);
    }

    public void ChangeBackgroundSprite(int stage){
        spriteRenderer.sprite = backgroundTextures[stage - 1];
        scopeArray[stage - 1] = scope;
        stageNumber = stage;
    }
}
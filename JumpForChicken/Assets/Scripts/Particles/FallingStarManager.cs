using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStarManager : MonoBehaviour
{
    [SerializeField] GameObject fallingStar;

    private float screenWidth = 9f;
    private float screenHeight = 16f;

    private int nextSpawnFrame = 0;

    void Start()
    {
        nextSpawnFrame = 15;
    }

    void FixedUpdate()
    {
        if (GameManager.stage == 4){
            nextSpawnFrame -= 1;
            if (nextSpawnFrame <= 0){
                nextSpawnFrame = Random.Range(67, 233);
                
                // Instantiate as a child so we can set localPosition directly
                GameObject star = Instantiate(fallingStar, transform);
                star.transform.localPosition = new Vector3(
                    Random.Range(-screenWidth / 2f, screenWidth / 2f) / 9f,
                    Random.Range(-screenHeight / 2f, screenHeight / 2f) / 9f - 1f,
                    0f
                );
                star.transform.localScale = 1 / 14f * new Vector3(2 * Random.Range(1, 3) - 3, 1, 1);
            }
        }
    }
}

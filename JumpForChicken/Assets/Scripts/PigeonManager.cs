using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : MonoBehaviour
{


    public GameObject chickenPrefab;

    GameObject cameras;

    public float moveSpeed = 0.06f;

    public float objectSize = 1f;

    public float surviveTime = 6f; // 초 단위

    private float xPos = 0;

    private int moveVector = 1;

    private float randomDropTime;    

    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.main.gameObject; // 메인 카메라를 찾음

        moveVector = Random.Range(1, 3) == 1 ? -1 : 1;
        xPos = Random.Range(-4.5f + objectSize/2, 4.5f - objectSize/2);

        surviveTime = 6f;

        randomDropTime = Random.Range(1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(xPos, cameras.transform.position.y + 8f - objectSize/2, 0);
        Move();

        if (surviveTime > 0){
            surviveTime -= Time.deltaTime;
            if (randomDropTime > surviveTime){
                ChickenDrop();
                randomDropTime = -1f;
            }
        } else {
            Disappear();
        }
    }

    void Move(){
        if (xPos + moveSpeed*moveVector >= 4.5f - objectSize/2){
            xPos = 4.5f - objectSize/2;
            moveVector *= -1;
        } else if (xPos + moveSpeed*moveVector <= -4.5f + objectSize/2){
            xPos = -4.5f + objectSize/2;
            moveVector *= -1;
        } else {
            xPos += moveSpeed * moveVector;
        }
    }

    void ChickenDrop(){
        GameObject chickens = Instantiate(chickenPrefab);
        chickens.transform.position = new Vector3(xPos, gameObject.transform.position.y - 0.3f, 0);
    }

    void Disappear(){
        Destroy(gameObject);
    }
}

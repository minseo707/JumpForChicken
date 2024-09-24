using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityDustManager : MonoBehaviour
{

    public GameObject player;

    public float liveTime = 0.2f;

    float leftDie = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        leftDie = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        leftDie -= Time.deltaTime;

        if (leftDie <= 0){
            gameObject.SetActive(false);
        }
    }

    public void LandingPtc(){
        leftDie = liveTime;
        gameObject.SetActive(true);
    }
}

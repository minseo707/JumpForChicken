using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonCooldownManager : MonoBehaviour
{

    public GameObject pigeonPrefab;

    public float cooldownTime = 14f;
    private float leftcooldownTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        leftcooldownTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        leftcooldownTime += Time.deltaTime;

        if (leftcooldownTime >= cooldownTime){
            GeneratePigeon();
            leftcooldownTime = 0;
        }
    }

    void GeneratePigeon(){
        Instantiate(pigeonPrefab);
    }
}

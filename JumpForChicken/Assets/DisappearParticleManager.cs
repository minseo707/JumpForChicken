using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearParticleManager : MonoBehaviour
{
    float times = 0f;
    // Update is called once per frame
    void Update()
    {
        times += Time.deltaTime;

        if (times > 0.5f){
            Destroy(gameObject);
        }
    }
}

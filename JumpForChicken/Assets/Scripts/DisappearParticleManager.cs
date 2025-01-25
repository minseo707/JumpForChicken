using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearParticleManager : MonoBehaviour
{

    public float disappearTime = 0.5f;

    float times = 0f;
    // Update is called once per frame
    void Update()
    {
        times += Time.deltaTime;

        if (times > disappearTime){
            Destroy(gameObject);
        }
    }
}

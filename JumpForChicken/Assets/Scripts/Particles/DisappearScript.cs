using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearScript : MonoBehaviour
{
    public float disappearTime = 1f;
    void Start()
    {
        Destroy(gameObject, disappearTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUIPositionManager : MonoBehaviour
{

    public float offset = 100f;
    GameObject cameras;

    private float originYPos;

    // Start is called before the first frame update
    void Start()
    {
        cameras = Camera.main.gameObject;
        originYPos = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, -16*offset, transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, -cameras.transform.position.y*offset + originYPos, transform.localPosition.z);
    }
}

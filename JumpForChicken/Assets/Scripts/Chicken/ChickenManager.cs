using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenManager : MonoBehaviour
{

    GameObject gm;

    Collider2D col;

    private void Awake() {
        gm = GameObject.Find("GameManager");
        col = gameObject.GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gm.GetComponent<ScoreManager>().AddChicken(1);
            Disappear();
        }
    }

    void Disappear(){
        col.enabled = false; // 유지보수
        Destroy(transform.parent.gameObject);
    }
}

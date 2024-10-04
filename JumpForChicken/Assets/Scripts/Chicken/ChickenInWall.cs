using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenInWall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Block"){
            if (other.gameObject.transform.position.y - 3 > gameObject.transform.position.y){
                gameObject.transform.position += new Vector3(0f, other.gameObject.transform.position.y - gameObject.transform.position.y - 3, 0f);
            }
        }
    }
}

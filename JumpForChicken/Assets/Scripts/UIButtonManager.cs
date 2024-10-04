using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIButtonManager : MonoBehaviour
{

    GameObject player;
    PlayerController playerController;

    public void Init(){
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    public void LeftButtonDown(){
        playerController.inputLeft = true;
    }

    public void LeftButtonUp(){
        playerController.inputLeft = false;
    }

    public void RightButtonDown(){
        playerController.inputRight = true;
    }

    public void RightButtonUp(){
        playerController.inputRight = false;
    }

    public void JumpButtonDown(){
        playerController.inputJump = true;
    }

    public void JumpButtonUp(){
        playerController.inputJump = false;
        playerController.firstJumpUp = true;
    }

}

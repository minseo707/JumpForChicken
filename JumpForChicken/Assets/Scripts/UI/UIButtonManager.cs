using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UIButtonManager : MonoBehaviour
{
    public bool buttonLock = false;

    GameObject player;
    PlayerController playerController;

    public void Init(){
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (buttonLock){
            LeftButtonUp();
            RightButtonUp();
            JumpButtonUp();
        }
    } 

    public void LeftButtonDown(){
        if (buttonLock) return;
        playerController.inputLeft = true;
    }

    public void LeftButtonUp(){
        playerController.inputLeft = false;
    }

    public void RightButtonDown(){
        if (buttonLock) return;
        playerController.inputRight = true;
    }

    public void RightButtonUp(){
        playerController.inputRight = false;
    }

    public void JumpButtonDown(){
        if (buttonLock) return;
        playerController.inputJump = true;
    }

    public void JumpButtonUp(){
        playerController.inputJump = false;
        if (buttonLock) return;
        playerController.firstJumpUp = true;
    }

}

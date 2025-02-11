using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopButtonManager : MonoBehaviour
{
    public void OnClickBuyButton(){
        Debug.Log("[ShopButtonManager] Buy Item");
    }

    public void OnclickBackwardButton(){
        SceneManager.LoadScene("MainTitle");
    }
}

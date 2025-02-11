using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectManager : MonoBehaviour
{
    private Image image;

    public bool isPurchased;

    private void Awake() {
        image = gameObject.GetComponent<Image>();
    }

    public void Select(){
        if (!isPurchased){
            SetColorByRGB(180, 180, 180);
        } else {
            SetColorByRGB(134, 191, 137);
        }
    }

    public void Unselect(){
        if (!isPurchased){
            SetColorByRGB(255, 255, 255);
        } else {
            SetColorByRGB(152, 225, 155);
        }
    }

    private void SetColorByRGB(int r, int g, int b, float a = 1f)
    {
        image.color = new Color(r / 255f, g / 255f, b / 255f, a);
    }
}

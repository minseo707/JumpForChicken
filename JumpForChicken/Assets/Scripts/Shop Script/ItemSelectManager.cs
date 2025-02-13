using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectManager : MonoBehaviour
{
    private Image image;
    private GameObject selectFx;
    private Animator selectFxAnim;

    public Sprite stopImage;

    public bool isPurchased;
    public bool isLastSelect;

    private void Awake() {
        image = gameObject.GetComponent<Image>();
        selectFx = transform.GetChild(1).gameObject;
        selectFxAnim = selectFx.GetComponent<Animator>();
        isLastSelect = false;
        ItsNotMe();
    }

    public void Select(){
        selectFx.SetActive(true);
        selectFxAnim.enabled = true;
        selectFx.GetComponent<Animator>().Rebind();
        selectFx.GetComponent<Animator>().Update(0);
        if (!isPurchased){
            SetColorByRGB(180, 180, 180);
        } else {
            SetColorByRGB(134, 191, 137);
        }
    }

    public void Unselect(){
        selectFx.SetActive(false);
        if (!isPurchased){
            SetColorByRGB(255, 255, 255);
        } else {
            SetColorByRGB(152, 225, 155);
        }
    }

    public void ItsNotMe(){
        selectFxAnim.enabled = false;
        selectFx.GetComponent<Image>().sprite = stopImage;
        Debug.Log("[ItemSelectManager] ItsNotMe");
    }

    private void SetColorByRGB(int r, int g, int b, float a = 1f)
    {
        image.color = new Color(r / 255f, g / 255f, b / 255f, a);
    }
}

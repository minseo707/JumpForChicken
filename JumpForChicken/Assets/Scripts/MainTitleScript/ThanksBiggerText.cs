using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThanksBiggerText : MonoBehaviour
{
    public float defalutFontSize = 30f;
    public float biggerFontSize = 40f;

    private const float clickTime = 2f;

    private float clickStartFontSize = 0f;

    private float fontSize;
    private bool onButtonDown = false;

    private TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void TextButtonDown(){
        onButtonDown = true;
        clickStartFontSize = fontSize;
    }

    public void TextButtonUp(){
        onButtonDown = false;
    }

    private void Update()
    {
        if (onButtonDown){
            fontSize += (biggerFontSize - clickStartFontSize) * Time.deltaTime / clickTime;
            fontSize = Mathf.Min(fontSize, biggerFontSize);
        } else {
            fontSize -= (biggerFontSize - defalutFontSize) * Time.deltaTime * clickTime;
            fontSize = Mathf.Max(fontSize, defalutFontSize);
        }

        tmp.fontSize = fontSize;
    }
}

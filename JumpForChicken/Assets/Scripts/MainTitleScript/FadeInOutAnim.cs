using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutAnim : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // CanvasGroup 컴포넌트가 없다면 추가합니다.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        // FadeIn 코루틴을 시작합니다.
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        canvasGroup.alpha = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}

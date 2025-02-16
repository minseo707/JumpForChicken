using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

/// <summary>
/// 인게임 제외에서 화면 비율을 고정하는 클래스
/// </summary>
[RequireComponent(typeof(Camera))]
public class LetterboxCamera : MonoBehaviour
{
    // 9:16
    public float targetAspectWidth = 9f;
    public float targetAspectHeight = 16f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // 목표 비율 계산
        float targetAspect = targetAspectWidth / targetAspectHeight;
        float windowAspect;

        #if UNITY_EDITOR
        // Editor에서는 Game 창의 해상도를 사용
        windowAspect = GetGameViewAspect();
        #else
        // 빌드된 환경에서는 실제 스크린 해상도를 사용
        windowAspect = (float)Screen.width / Screen.height;
        #endif

        float scaleHeight = windowAspect / targetAspect;

        Debug.Log($"[LetterBoxCamera] {targetAspect}, {windowAspect}, {scaleHeight}");

        if (scaleHeight < 1.0f)
        {
            // 세로가 더 긴 경우: 위아래에 레터박스 생성
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;  // 화면 중앙 정렬
            cam.rect = rect;
        }
        else
        {
            // 가로가 더 긴 경우: 좌우에 필러박스 생성
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;   // 화면 중앙 정렬
            rect.y = 0;
            cam.rect = rect;
        }
    }

    #if UNITY_EDITOR
    // Game 창의 해상도를 가져와 비율 계산에 사용하는 메서드 (리플렉션 이용)
    float GetGameViewAspect()
    {
        System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        MethodInfo getSizeOfMainGameView = gameViewType.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
        object result = getSizeOfMainGameView.Invoke(null, null);
        Vector2 gameSize = (Vector2)result;
        return gameSize.x / gameSize.y;
    }
    #endif
}
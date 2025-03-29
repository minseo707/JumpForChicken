using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChickenDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // "11" 텍스트
    public GameObject unitObject;     // "m" 이미지가 있는 GameObject
    public float unitOffsetY = -10f;  // "m"의 Y 좌표 조정값
    public float spacing = 5f;        // 텍스트와 이미지 사이의 간격

    private CameraController cc;

    private RectTransform unitRectTransform;

    void Start()
    {
        // unitObject에서 RectTransform 컴포넌트를 가져옵니다
        unitRectTransform = unitObject.GetComponent<RectTransform>();
        cc = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (cc == null) cc = Camera.main.GetComponent<CameraController>();
        if (cc == null) return;

        // 텍스트의 오른쪽 끝을 기준으로 이미지 위치 조정
        Vector2 scoreEndPos = scoreText.rectTransform.anchoredPosition + new Vector2(scoreText.preferredWidth, 0);

        // 이미지의 위치를 텍스트의 오른쪽 끝에서 왼쪽으로 간격을 두고 배치
        unitRectTransform.anchoredPosition = new Vector2(
            -scoreEndPos.x + spacing + unitRectTransform.rect.width,
            scoreText.rectTransform.anchoredPosition.y + unitOffsetY
        );
    }
}
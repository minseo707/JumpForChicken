using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChickenDisplayManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // "11" 텍스트
    public GameObject unitObject;     // "m" 이미지가 있는 GameObject
    public float unitOffsetY = -10f;  // "m"의 Y 좌표 조정값
    public float spacing = 5f;        // 텍스트와 이미지 사이의 간격

    private RectTransform unitRectTransform;

    void Start()
    {
        // unitObject에서 RectTransform 컴포넌트를 가져옵니다
        unitRectTransform = unitObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 텍스트의 오른쪽 끝을 기준으로 이미지 위치 조정
        Vector3 scoreEndPos = scoreText.rectTransform.position + new Vector3(scoreText.preferredWidth / 2, 0, 0);

        // 이미지의 위치를 텍스트의 오른쪽 끝에서 왼쪽으로 간격을 두고 배치
        unitRectTransform.position = new Vector3(
            (-scoreEndPos.x + spacing + (unitRectTransform.rect.width / 2))/60,
            scoreText.rectTransform.position.y + unitOffsetY/100,
            scoreText.rectTransform.position.z
        );
    }
}
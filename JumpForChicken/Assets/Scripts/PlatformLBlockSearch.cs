using UnityEngine;

public class PlatformLBlockSearch : MonoBehaviour
{
    public enum Direction { Left, Right }
    public Direction currentState;

    private float targetX;

    // PlatformGenerator에서 이전 플랫폼 X 좌표 가져오기
    public void SetXCoordinate(float targetX)
    {
        this.targetX = targetX;
    }

    // 프리팹 생성 후 외부에서 호출할 수 있도록 public으로 변경
    public void ReverseBlock()
    {
        if (targetX > transform.position.x && currentState == Direction.Right)
        {
            Debug.Log("스케일을 오른쪽에서 왼쪽으로 반전시킵니다.");
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (targetX < transform.position.x && currentState == Direction.Left)
        {
            Debug.Log("스케일을 왼쪽으로 오른쪽으로 반전시킵니다.");
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            Debug.Log("스케일을 반전시키지 않습니다.");
        }
    }
}

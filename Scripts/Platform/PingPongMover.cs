using UnityEngine;

// 한 축을 따라 왕복하는 오브젝트의 공통 부모 (움직이는 발판, 가시).
// 자식은 시작 위치를 정해 주고(SetRange) 매 프레임 PingPong을 부른다.
public abstract class PingPongMover : MonoBehaviour
{
    [SerializeField] protected float distance;
    [SerializeField] protected float moveSpeed;
    [SerializeField] private bool turnSwitch;

    private float turningPoint;

    // 왕복 범위의 아래쪽 끝을 정한다 (시작 위치 - 거리)
    protected void SetRange(float initPosition)
    {
        turningPoint = initPosition - distance;
    }

    // upperLimit에 닿으면 아래로, turningPoint에 닿으면 위로 방향을 바꾸며 axis 방향으로 움직인다
    protected void PingPong(Vector3 axis, float currentPosition, float upperLimit)
    {
        if (currentPosition >= upperLimit)
        {
            turnSwitch = false;
        }
        else if (currentPosition <= turningPoint)
        {
            turnSwitch = true;
        }

        Vector3 direction = turnSwitch ? axis : -axis;
        transform.position = transform.position + direction * moveSpeed * Time.deltaTime;
    }
}

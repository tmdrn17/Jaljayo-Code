using UnityEngine;

// 종류별로 다르게 회전하는 통나무 장애물. kind 값에 따라 회전 속도·움직임이 달라진다.
public enum LogKind
{
    SlidingSpin,    // -100도 회전하면서 z축으로 왕복
    Spin150,
    OrbitSpin,      // 100도 회전하면서 한 점을 중심으로 공전
    ReverseSpin170A,
    Spin100,
    Spin720,
    ReverseSpin170B,
}

public class RotatingLog : MonoBehaviour
{
    private const float SlideMaxZ = 43f;
    private const float SlideMinZ = 39f;
    private const float SlideSpeed = 0.6f;
    private static readonly Vector3 OrbitCenter = new Vector3(0, 2.7f, 58);

    [SerializeField] private LogKind kind;

    // 왕복 방향 (1 또는 -1). 처음엔 0이라 범위를 벗어나기 전까지 움직이지 않는다.
    private int slideDirection;

    private void Update()
    {
        switch (kind)
        {
            case LogKind.SlidingSpin:
                Spin(-100f);
                UpdateSlideDirection();
                transform.Translate(Vector3.forward * SlideSpeed * slideDirection * Time.deltaTime, Space.World);
                break;

            case LogKind.Spin150:
                Spin(150f);
                break;

            case LogKind.OrbitSpin:
                Spin(100f);
                transform.RotateAround(OrbitCenter, Vector3.right, 40 * Time.deltaTime);
                break;

            case LogKind.ReverseSpin170A:
            case LogKind.ReverseSpin170B:
                Spin(-170f);
                break;

            case LogKind.Spin100:
                Spin(100f);
                break;

            case LogKind.Spin720:
                Spin(720f);
                break;
        }
    }

    // x축 기준으로 초당 speed도 회전한다.
    private void Spin(float speed)
    {
        transform.Rotate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void UpdateSlideDirection()
    {
        float currentPosZ = transform.position.z;

        if (currentPosZ > SlideMaxZ)
        {
            slideDirection = -1;
        }
        else if (currentPosZ < SlideMinZ)
        {
            slideDirection = 1;
        }
    }
}

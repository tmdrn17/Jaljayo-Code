using UnityEngine;

// 오브젝트 이름에 따라 위아래·좌우·앞뒤로 왕복하거나 회전하는 발판
public class MoveFloor : PingPongMover
{
    private enum FloorMove
    {
        None,
        UpDown,
        Rotate,
        LeftRight,
        ForwardBack
    }

    private const string UpDownName = "UD_Floor";
    private const string RotateName = "RT_Floor";
    private const string LeftRightName = "LR_Floor";
    private const string ForwardBackName = "FB_Floor";

    [SerializeField] private float rotateSpeed;

    private FloorMove moveKind;
    private float initPosition;

    private void Awake()
    {
        switch (gameObject.name)
        {
            case UpDownName:
                moveKind = FloorMove.UpDown;
                initPosition = transform.position.y;
                SetRange(initPosition);
                break;
            case RotateName:
                moveKind = FloorMove.Rotate;
                break;
            case LeftRightName:
                moveKind = FloorMove.LeftRight;
                initPosition = transform.position.x;
                SetRange(initPosition);
                break;
            case ForwardBackName:
                moveKind = FloorMove.ForwardBack;
                initPosition = transform.position.z;
                SetRange(initPosition);
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (moveKind)
        {
            case FloorMove.UpDown:
                // 위아래 발판은 시작 위치가 위쪽 끝이다
                PingPong(Vector3.up, transform.position.y, initPosition);
                break;
            case FloorMove.Rotate:
                transform.Rotate(Vector3.left * rotateSpeed * Time.deltaTime);
                break;
            case FloorMove.LeftRight:
                PingPong(Vector3.right, transform.position.x, initPosition + distance);
                break;
            case FloorMove.ForwardBack:
                PingPong(Vector3.forward, transform.position.z, initPosition + distance);
                break;
        }
    }
}

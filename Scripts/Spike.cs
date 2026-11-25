using UnityEngine;

// 태그에 따라 위아래(Spike) 또는 좌우(SideSpike, SliceWall)로 왕복하는 가시·칼날
public class Spike : PingPongMover
{
    private enum SpikeMove
    {
        None,
        UpDown,
        LeftRight
    }

    private SpikeMove moveKind;
    private float initPosition;

    private void Awake()
    {
        switch (gameObject.tag)
        {
            case Tags.Spike:
                moveKind = SpikeMove.UpDown;
                initPosition = transform.position.y;
                SetRange(initPosition);
                break;
            case Tags.SliceWall:
            case Tags.SideSpike:
                moveKind = SpikeMove.LeftRight;
                initPosition = transform.position.x;
                SetRange(initPosition);
                break;
        }
    }

    private void Update()
    {
        switch (moveKind)
        {
            case SpikeMove.UpDown:
                PingPong(Vector3.up, transform.position.y, initPosition + distance);
                break;
            case SpikeMove.LeftRight:
                PingPong(Vector3.right, transform.position.x, initPosition + distance);
                break;
        }
    }
}

using UnityEngine;

// 다가오는 벽. WallMove가 켜지면 매 프레임 뒤로 밀려오고, 도착 지점에 닿으면 사라진다.
// 공개 멤버(wall, WallMove)는 다른 스크립트가 쓰므로 이름을 바꾸지 않는다.
public class Wall : MonoBehaviour
{
    [SerializeField] private float force = 1;
    public GameObject wall;
    public static bool WallMove;

    private void Update()
    {
        if (WallMove)
        {
            transform.position = transform.position + new Vector3(0, 0, -1) * force;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Destination)) Destroy(gameObject);
    }
}

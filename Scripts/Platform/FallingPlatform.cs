using UnityEngine;

// 플레이어가 밟으면 잠시 뒤 떨어지고, 일정 시간 뒤 사라지는 발판
public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallSec = 0.5f;
    [SerializeField] private float destroySec = 2f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals(ObjectNames.Player))
        {
            Invoke(nameof(FallPlatform), fallSec);
            Destroy(gameObject, destroySec);
        }
    }

    private void FallPlatform()
    {
        rb.isKinematic = false;
    }
}

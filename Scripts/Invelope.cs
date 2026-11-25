using UnityEngine;

// 편지 봉투. 플레이어가 닿으면 주운 것으로 기록하고 사라진다.
public class Invelope : MonoBehaviour
{
    private const string PlayerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag) == false) return;

        GameManager.Instance.PickEnvelope();
        Destroy(gameObject);
    }
}

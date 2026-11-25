using UnityEngine;

// 미로 스테이지 코인. 플레이어가 닿으면 점수를 올리고 위로 튀어 오른 뒤 사라진다.
public class Coin : MonoBehaviour
{
    [SerializeField] private int coinIndex;
    [SerializeField] private AudioClip coinSound;

    [Header("획득 연출")]
    [SerializeField] private Vector3 popForce = new Vector3(0, 3f, 0);
    [SerializeField] private float disableDelay = 0.5f;

    private AudioSource audioSource;
    private Rigidbody rigid;
    private SphereCollider col;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) == false) return;

        // 1. 소리와 점수
        audioSource.PlayOneShot(coinSound);
        GameManager.Instance.CollectCoin();
        CoinManager.MarkCollected(coinIndex);

        // 2. 위로 튀어 오른 뒤 잠시 후 끈다.
        rigid.AddForce(popForce, ForceMode.Impulse);
        rigid.useGravity = true;
        col.enabled = false;
        Invoke(nameof(Deactivate), disableDelay);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}

using UnityEngine;

// 메인홀의 층 문. 해당 층을 클리어하면 콜라이더를 끄고, 상호작용하면 문이 열린다.
public class MainHallDoor : MonoBehaviour
{
    private const string PlayerTag = "Player";
    private const string InteractionButton = "Interaction";
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private int doorNum;
    [SerializeField] private Collider doorCollider;
    [SerializeField] private Collider innerCollider;

    private Animator anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance.IsFloorClear(doorNum))
        {
            doorCollider.enabled = false;
            innerCollider.enabled = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(PlayerTag) == false) return;

        if (Input.GetButton(InteractionButton))
        {
            anim.SetBool(OpenHash, true);
            audioSource.Play();
        }
    }
}

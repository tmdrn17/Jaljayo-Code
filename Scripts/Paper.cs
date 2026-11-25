using UnityEngine;

// 쪽지 아이템. 먹으면 쪽지 수를 올리고, 연결된 문이 있으면 연다.
public class Paper : MonoBehaviour
{
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private Animator door;
    [SerializeField] private GameObject doorCollider;
    [SerializeField] private AudioSource audioSource;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) == false) return;

        if (door != null)
        {
            audioSource.Play();
            door.SetTrigger(OpenHash);
            doorCollider.GetComponent<LastDoor>().OpenDoor();
        }

        GameManager.Instance.CollectPaper();
        Destroy(gameObject);
    }
}

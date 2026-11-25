using UnityEngine;

// 미로의 열쇠. 마지막 열쇠(다른 열쇠가 남아 있지 않을 때)를 먹으면 문이 열린다.
public class MazeKey : MonoBehaviour
{
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private GameObject otherKey;
    [SerializeField] private Animator door;
    [SerializeField] private GameObject doorCollider;
    [SerializeField] private AudioSource audioSource;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) == false) return;

        if (otherKey == null)
        {
            audioSource.Play();
            door.SetTrigger(OpenHash);
            doorCollider.GetComponent<LastDoor>().OpenDoor();
        }
        Destroy(gameObject);
    }
}

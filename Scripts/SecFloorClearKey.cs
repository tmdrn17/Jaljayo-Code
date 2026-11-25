using UnityEngine;

// 2층 클리어 열쇠. 플레이어가 닿으면 문을 연다.
public class SecFloorClearKey : MonoBehaviour
{
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private Animator door;
    [SerializeField] private GameObject doorCollider;
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.Player) == false) return;

        if (door != null)
        {
            audioSource.Play();
            door.SetTrigger(OpenHash);
            doorCollider.GetComponent<LastDoor>().OpenDoor();
        }
        Destroy(gameObject);
    }
}

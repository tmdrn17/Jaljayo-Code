using UnityEngine;

// 튜토리얼 문. 가까이서 상호작용하면 문이 열렸다가 1초 뒤 콜라이더가 다시 켜진다. 한 번만 작동한다.
public class DoorController : MonoBehaviour
{
    private const string DoorOpenState = "DoorOpen";

    [SerializeField] private GameObject instruction;
    [SerializeField] private GameObject animObject;
    [SerializeField] private MeshCollider[] door;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float colliderOnDelay = 1f;

    private Animator doorAnim;
    private AudioSource audioSource;
    private bool canOpen;
    private bool hasOpened;

    void Start()
    {
        instruction.SetActive(false);
        doorAnim = animObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) && hasOpened == false)
        {
            instruction.SetActive(true);
            canOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        instruction.SetActive(false);
        canOpen = false;
    }

    void Update()
    {
        if (Input.GetButtonDown(InputNames.Interaction) == false) return;
        if (canOpen == false) return;

        doorAnim.Play(DoorOpenState);
        audioSource.PlayOneShot(openSound);
        instruction.SetActive(false);
        canOpen = false;

        SetDoorColliders(false);
        Invoke(nameof(DoorColliderOn), colliderOnDelay);

        hasOpened = true;
    }

    private void DoorColliderOn()
    {
        SetDoorColliders(true);
    }

    private void SetDoorColliders(bool isEnabled)
    {
        for (int i = 0; i < door.Length; i++)
        {
            door[i].enabled = isEnabled;
        }
    }
}

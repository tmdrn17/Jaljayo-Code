using System.Collections;
using UnityEngine;

public enum ButtonKind
{
    Teleport,
    Attack,
    Back,
    Lever,
    Final
}

// 포탈 스테이지의 버튼·레버. 플레이어가 상호작용하면 종류에 따라 순간이동, 상자 생성, 문 열기를 한다.
public class Button : MonoBehaviour
{
    private static readonly int PullHash = Animator.StringToHash("Pull");
    private static readonly int PushHash = Animator.StringToHash("Push");
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [Header("종류")]
    [SerializeField] private ButtonKind kind;
    [SerializeField] private AudioClip openSound;

    [Header("이동")]
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private Transform backPoint;
    [SerializeField] private float teleportDelay = 0.7f;

    [Header("레버")]
    [SerializeField] private GameObject crossBox;
    [SerializeField] private Transform boxPoint;
    [SerializeField] private GameObject paper;
    [SerializeField] private Animator door;

    private Animator anim;
    private AudioSource audioSource;
    private Vector3 boxPosition;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // TODO(리팩토링): boxPoint가 비어 있는 버튼(이동 버튼 등)은 여기서 NullReferenceException이 나고 audioSource도 비게 된다. 원래 동작 유지.
        boxPosition = boxPoint.position;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>플레이어가 상호작용 키를 누르면 Player가 부른다. 종류별 동작은 버튼이 정한다.</summary>
    public void Activate()
    {
        switch (kind)
        {
            case ButtonKind.Teleport:
                StartCoroutine(MoveSpace(teleportPoint));
                break;
            case ButtonKind.Back:
                StartCoroutine(MoveSpace(backPoint));
                break;
            case ButtonKind.Attack:
                GameSession.ApplyDamage(1);
                break;
            case ButtonKind.Lever:
                PullLever();
                break;
            case ButtonKind.Final:
                PullFinalLever();
                break;
        }
    }

    private void PullLever()
    {
        anim.SetTrigger(PullHash);
        Instantiate(crossBox, boxPosition, Quaternion.identity);
    }

    private void PullFinalLever()
    {
        anim.SetTrigger(PullHash);

        if (paper == null)
        {
            audioSource.PlayOneShot(openSound);
            door.SetBool(OpenHash, true);
        }
    }

    private IEnumerator MoveSpace(Transform point)
    {
        anim.SetTrigger(PushHash);
        yield return new WaitForSeconds(teleportDelay);
        GameManager.Instance.MoveCharacter(point.position);
    }
}

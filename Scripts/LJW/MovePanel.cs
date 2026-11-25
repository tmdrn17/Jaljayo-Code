using UnityEngine;

// 플레이어가 올라서면 목표 지점으로 움직이는 발판. 이름에 따라 문(dLow)을 돌린다.
public class MovePanel : MonoBehaviour
{
    private const string RightMoveName = "RightMove";
    private const string LeftMoveName = "LeftMove";
    private const float PanelSpeed = 2.5f;
    private const float DoorRotateSpeed = 100f;

    [SerializeField] private GameObject movePanel;
    [SerializeField] private Transform movePos;
    [SerializeField] private GameObject dLow;

    private Player player;
    private bool isMove;

    private void Update()
    {
        if (isMove)
        {
            movePanel.transform.position = Vector3.MoveTowards(movePanel.transform.position, movePos.position, PanelSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player enteredPlayer) == false) return;

        player = enteredPlayer;
        other.transform.SetParent(movePanel.transform);
        player.SetMovePanel(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Player _) == false) return;

        isMove = true;

        if (gameObject.name == RightMoveName)
        {
            dLow.transform.Rotate(transform.forward * -DoorRotateSpeed * Time.deltaTime, Space.World);
        }
        else if (gameObject.name == LeftMoveName)
        {
            dLow.transform.Rotate(transform.forward * DoorRotateSpeed * Time.deltaTime, Space.World);
        }
    }

    // TODO(리팩토링): 태그 검사 없이 나간 모든 물체의 부모를 해제하고 DontDestroyOnLoad를 건다.
    private void OnTriggerExit(Collider other)
    {
        isMove = false;
        other.transform.SetParent(null);
        if (player != null) player.SetMovePanel(false);

        DontDestroyOnLoad(other.gameObject);
    }
}

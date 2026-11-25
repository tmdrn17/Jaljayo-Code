using UnityEngine;

// 문 콜라이더를 끄는 문. 쪽지·열쇠가 OpenDoor를 불러 연다.
public class LastDoor : MonoBehaviour
{
    [SerializeField] private BoxCollider doorCollider;

    public void OpenDoor()
    {
        doorCollider.enabled = false;
    }
}

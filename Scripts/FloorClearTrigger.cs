using UnityEngine;

// 플레이어가 닿으면 1층 스테이지 하나를 클리어한 것으로 기록한다.
public class FloorClearTrigger : MonoBehaviour
{
    [SerializeField] private int floorIndex;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Player) == false) return;

        GameManager.Instance.SetFloorClear(floorIndex);
    }
}

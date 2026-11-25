using UnityEngine;

// 발판 위에 올라온 플레이어를 발판의 자식으로 붙여 같이 움직이게 한다
public class PlatformAttach : MonoBehaviour
{
    private Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player enteredPlayer) == false) return;

        player = enteredPlayer;
        other.transform.SetParent(transform);
        player.SetMovePanel(true);
    }

    // TODO(리팩토링): 태그 검사 없이 나간 모든 물체의 부모를 해제하고 DontDestroyOnLoad를 건다.
    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
        if (player != null) player.SetMovePanel(false);

        DontDestroyOnLoad(other.gameObject);
    }
}

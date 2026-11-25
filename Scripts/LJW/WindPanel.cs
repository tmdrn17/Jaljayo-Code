using UnityEngine;

// 바람 발판. 날개를 돌리고, 안에 있는 플레이어를 위로 띄운다.
public class WindPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] windPanel;

    [Header("바람")]
    [SerializeField] private float fanSpeed = 2000f;
    [SerializeField] private float liftImpulse = 0.6f;
    [SerializeField] private float fallLiftImpulse = 0.1f;

    private Player player;
    private bool isWindPanel;

    private void Update()
    {
        for (int i = 0; i < windPanel.Length; i++)
        {
            windPanel[i].transform.Rotate(Vector3.up * fanSpeed * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        if (isWindPanel == false) return;

        player.Rig.AddForce(Vector3.up * liftImpulse, ForceMode.Impulse);

        // 옆으로는 움직이지 못하게 가로 속도를 없앤다
        float y = player.Rig.velocity.y;
        player.Rig.velocity = new Vector3(0, y, 0);

        if (player.Rig.velocity.y < 0)
        {
            player.Rig.AddForce(Vector3.up * fallLiftImpulse, ForceMode.Impulse);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Player stayedPlayer) == false) return;

        player = stayedPlayer;
        isWindPanel = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isWindPanel = false;
        if (player != null) player.ResetJump();
    }
}

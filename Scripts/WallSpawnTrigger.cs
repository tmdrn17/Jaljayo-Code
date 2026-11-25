using UnityEngine;

// 플레이어가 처음 들어오면 벽을 한 번만 생성한다.
public class WallSpawnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private Transform spawnPoint;

    private bool canSpawn = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.name != ObjectNames.Player) return;

        if (canSpawn) Instantiate(wall, spawnPoint);
        canSpawn = false;
    }
}

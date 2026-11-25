using System.Collections;
using UnityEngine;

// 구멍에 빠뜨리는 퍼즐용 큐브. 목표 구멍에 넣으면 다리가 생기고, 다른 구멍에 빠지면 피해를 받고 되돌아간다.
public class Cube : MonoBehaviour
{
    private const string GoalHoleName = "Hole4GoalPost";
    private const string Hole1Name = "Hole1GoalPost";
    private const string Hole2Name = "Hole2GoalPost";
    private const string Hole3Name = "Hole3GoalPost";

    [SerializeField] private GameObject bridgePrefab;
    [SerializeField] private Transform bombPosition;
    [SerializeField] private float freezeTime = 0.2f;

    private Rigidbody rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.name)
        {
            case GoalHoleName:
                Destroy(gameObject);
                Instantiate(bridgePrefab, Vector3.zero, Quaternion.identity);
                break;
            case Hole1Name:
            case Hole2Name:
            case Hole3Name:
                ReturnToStart();
                break;
        }
    }

    // 피해를 받고 시작 위치로 돌아간 뒤 잠깐 멈춘다
    private void ReturnToStart()
    {
        GameSession.ApplyDamage(1);
        transform.position = bombPosition.position;
        transform.rotation = Quaternion.identity;
        StartCoroutine(Stop());
    }

    private IEnumerator Stop()
    {
        rig.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(freezeTime);
        rig.constraints = RigidbodyConstraints.None;
    }
}

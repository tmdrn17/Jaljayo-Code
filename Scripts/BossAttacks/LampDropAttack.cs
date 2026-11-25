using System.Collections;
using UnityEngine;

// 점프: 정해진 위치에서 전등을 떨어뜨린다.
[System.Serializable]
public class LampDropAttack : BossAttack
{
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly Quaternion LampRotation = Quaternion.Euler(0f, -90.0f, 180f);

    [SerializeField] private float dropForce;
    [SerializeField] private GameObject lampPrefab;
    [SerializeField] private Transform[] lampPoints;

    protected override int TriggerHash => JumpHash;

    protected override IEnumerator Attack()
    {
        for (int i = 0; i < lampPoints.Length; i++)
        {
            GameObject lamp = Object.Instantiate(lampPrefab, lampPoints[i].position, LampRotation);
            lamp.GetComponent<Rigidbody>().velocity = Vector3.down * dropForce;
        }

        yield break;
    }
}

using System.Collections;
using UnityEngine;

// 보스 공격 패턴 하나. 자기가 쓰는 프리팹·위치·시간 값을 직접 가진다.
// 흐름(애니메이션 → 공격 전 대기 → 공격 → 쿨타임)은 모든 패턴이 같고, 공격 내용만 자식이 정한다.
[System.Serializable]
public abstract class BossAttack
{
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float cooldown = 3f;

    /// <summary>패턴 시작 때 보스 애니메이터에 보낼 트리거</summary>
    protected abstract int TriggerHash { get; }

    public IEnumerator Run(Boss owner)
    {
        // 1. 애니메이션
        owner.PlayTrigger(TriggerHash);

        // 2. 공격 전 대기
        yield return new WaitForSeconds(attackDelay);

        // 3. 공격
        yield return owner.StartCoroutine(Attack());

        // 4. 쿨타임
        yield return new WaitForSeconds(cooldown);
    }

    protected abstract IEnumerator Attack();
}

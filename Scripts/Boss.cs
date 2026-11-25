using System.Collections;
using UnityEngine;

// 1층 보스. 잠깐 생각한 뒤 공격 패턴(점프·고함·내려찍기) 중 하나를 무작위로 실행하기를 반복한다.
public class Boss : MonoBehaviour
{
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [Header("체력")]
    [SerializeField] private int hp;

    [Header("공격 패턴")]
    [SerializeField] private float thinkDelay = 0.1f;
    [SerializeField] private LampDropAttack jumpAttack = new LampDropAttack();
    [SerializeField] private FileFireAttack yellAttack = new FileFireAttack();
    [SerializeField] private TileStrikeAttack strikeAttack = new TileStrikeAttack();

    [Header("죽음")]
    [SerializeField] private Animator door;
    [SerializeField] private AudioClip dieSound;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float destroyDelay = 5f;

    private Animator anim;
    private AudioSource audioSource;
    private BossAttack[] attacks;

    public int Hp => hp;
    /// <summary>시작 체력. 체력바 비율 계산에 쓴다.</summary>
    public int MaxHp { get; private set; }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        MaxHp = hp;
        attacks = new BossAttack[] { jumpAttack, yellAttack, strikeAttack };
        StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        // TODO(리팩토링): 체력이 정확히 0일 때만 죽는다. 피해가 남은 체력보다 크면(음수) 죽지 않는다.
        if (hp == 0)
        {
            StopAllCoroutines();
            Die();
        }
    }

    /// <summary>패턴이 시작할 때 애니메이션 트리거를 보낸다.</summary>
    public void PlayTrigger(int triggerHash)
    {
        anim.SetTrigger(triggerHash);
    }

    #region 공격 패턴
    // 잠깐 생각하고 → 무작위 패턴 실행 → 반복
    private IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(thinkDelay);

            BossAttack attack = attacks[Random.Range(0, attacks.Length)];
            yield return StartCoroutine(attack.Run(this));
        }
    }
    #endregion

    #region 피격·죽음
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(Tags.Bullet) == false) return;

        if (col.TryGetComponent(out Bullet paperBullet)) hp -= paperBullet.Damage;
    }

    private void Die()
    {
        hp--;
        anim.SetTrigger(DieHash);
        door.SetTrigger(OpenHash);
        audioSource.PlayOneShot(dieSound);
        audioSource.PlayOneShot(openSound);
        Destroy(gameObject, destroyDelay);
    }
    #endregion
}

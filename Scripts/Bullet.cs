using UnityEngine;

public enum BulletType
{
    Envelope,
    File,
    Lamp,
    Spike
}

// 보스·함정이 쏘는 투사체와 플레이어가 던지는 봉투. 종류에 따라 닿았을 때의 처리가 다르다.
public class Bullet : MonoBehaviour
{
    private const string BossTag = "Boss";
    private const string FloorTag = "Floor";

    [SerializeField] private int damage;
    [SerializeField] private BulletType bulletType;
    [SerializeField] private AudioClip damagedSound;

    private AudioSource audioSource;

    public int Damage => damage;
    public BulletType BType => bulletType;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider col)
    {
        switch (bulletType)
        {
            case BulletType.Envelope:
                if (col.CompareTag(Tags.Wall) || col.CompareTag(BossTag)) Destroy(gameObject);
                break;

            case BulletType.File:
                HitPlayerOrDestroy(col, Tags.Wall);
                break;

            case BulletType.Lamp:
                HitPlayerOrDestroy(col, FloorTag);
                break;

            case BulletType.Spike:
                // 가시는 플레이어에게 피해만 주고 사라지지 않는다
                if (col.CompareTag(Tags.Player)) ApplyHit();
                break;
        }
    }

    // 플레이어에 닿으면 피해를 주고, 플레이어나 지정한 바닥(벽)에 닿으면 사라진다
    private void HitPlayerOrDestroy(Collider col, string groundTag)
    {
        bool isPlayer = col.CompareTag(Tags.Player);
        if (isPlayer == false && col.CompareTag(groundTag) == false) return;

        if (isPlayer) ApplyHit();
        Destroy(gameObject);
    }

    private void ApplyHit()
    {
        GameSession.ApplyDamage(damage);
        audioSource.PlayOneShot(damagedSound);
    }
}

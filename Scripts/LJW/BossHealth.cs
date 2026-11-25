using UnityEngine;
using UnityEngine.UI;

// 보스 체력바. 보스의 체력 비율에 맞춰 바를 부드럽게 줄인다.
public class BossHealth : MonoBehaviour
{
    private const string BossName = "Boss";
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject bar;

    [Header("연출")]
    [SerializeField] private float showBarDelay = 2f;
    [SerializeField] private float hideBarDelay = 2.5f;
    [SerializeField] private float lerpSpeed = 10f;

    private Boss boss;

    private void Awake()
    {
        boss = GameObject.Find(BossName).GetComponent<Boss>();
    }

    private void Start()
    {
        Invoke(nameof(ShowBar), showBarDelay);
        healthBar.value = 1f;
    }

    private void Update()
    {
        UpdateBar();
    }

    private void ShowBar()
    {
        bar.SetActive(true);
    }

    private void UpdateBar()
    {
        float hpRate = (float)boss.Hp / boss.MaxHp;

        healthBar.value = Mathf.Lerp(healthBar.value, hpRate, Time.deltaTime * lerpSpeed);

        if (healthBar.value <= 0) Destroy(bar, hideBarDelay);
    }
}

using UnityEngine;
using UnityEngine.UI;

// 보스 체력바. 보스의 체력 비율에 맞춰 바를 부드럽게 줄인다.
public class BossHealth : MonoBehaviour
{
    private const float ShowBarDelay = 2f;
    private const float HideBarDelay = 2.5f;
    private const float LerpSpeed = 10f;

    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject bar;

    private Boss boss;

    private void Awake()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }

    private void Start()
    {
        Invoke(nameof(ShowBar), ShowBarDelay);
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

        healthBar.value = Mathf.Lerp(healthBar.value, hpRate, Time.deltaTime * LerpSpeed);

        if (healthBar.value <= 0) Destroy(bar, HideBarDelay);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 타이틀 로고를 위에서 튕기듯 내려오게 하고, 글자들을 하나씩 키워 나타나게 한다.
public class TitleUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> titleChars = new List<GameObject>();

    private RectTransform rectTransform;
    private float titleTime;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        HideTitleChars();
    }

    void Update()
    {
        // TODO(리팩토링): Update에서 매 프레임 코루틴을 새로 시작한다. 여러 코루틴이 같은 titleTime을 더해 연출이 빨라지는데, 동작 유지를 위해 그대로 둔다.
        StartCoroutine(AnimateTitle());
    }

    void HideTitleChars()
    {
        foreach (GameObject titleChar in titleChars)
        {
            titleChar.transform.localScale = Vector3.zero;
        }
    }

    IEnumerator AnimateTitle()
    {
        yield return new WaitForSeconds(0.5f);

        // 1. 시간에 따라 로고 위치를 구간별로 계산 (아래→위 튕김, 살짝 내려감, 제자리)
        titleTime += Time.deltaTime;

        if (titleTime < 0.5f)
        {
            rectTransform.anchoredPosition = new Vector2(0, 1000 - 2000 * titleTime);
        }
        else if (titleTime < 0.6f)
        {
            rectTransform.anchoredPosition = new Vector2(0, (titleTime - 0.5f) * 1000); // 0 ~ 100
        }
        else if (titleTime < 0.7f)
        {
            rectTransform.anchoredPosition = new Vector2(0, (0.7f - titleTime) * 1000); // 100 ~ 0
            StartCoroutine(AnimateTitleChars());
        }
        else if (titleTime < 0.8f)
        {
            rectTransform.anchoredPosition = new Vector2(0, (titleTime - 0.7f) * 500); // 0 ~ 50
        }
        else if (titleTime < 0.9f)
        {
            rectTransform.anchoredPosition = new Vector2(0, (0.9f - titleTime) * 500); // 50 ~ 0
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        yield return new WaitForSeconds(2f);
    }

    IEnumerator AnimateTitleChars()
    {
        foreach (GameObject titleChar in titleChars)
        {
            titleChar.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.35f);
        }
    }
}

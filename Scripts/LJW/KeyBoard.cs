using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 튜토리얼 키보드 UI. 3초 뒤 키들이 하나씩 튀어나오듯 나타난다.
public class KeyBoard : MonoBehaviour
{
    private const float ShowDelay = 3f;
    private const float KeyShowDuration = 0.2f;
    private const float KeyInterval = 0.25f;

    [SerializeField] private List<GameObject> keys = new List<GameObject>();

    void Start()
    {
        StartCoroutine(ShowKeys());

        foreach (GameObject key in keys)
        {
            key.transform.localScale = Vector3.zero;
        }
    }

    private IEnumerator ShowKeys()
    {
        yield return new WaitForSeconds(ShowDelay);

        foreach (GameObject key in keys)
        {
            key.transform.DOScale(1f, KeyShowDuration).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(KeyInterval);
        }
    }
}

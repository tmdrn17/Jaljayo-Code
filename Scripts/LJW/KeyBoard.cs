using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 튜토리얼 키보드 UI. 3초 뒤 키들이 하나씩 튀어나오듯 나타난다.
public class KeyBoard : MonoBehaviour
{
    [SerializeField] private List<GameObject> keys = new List<GameObject>();

    [Header("연출")]
    [SerializeField] private float showDelay = 3f;
    [SerializeField] private float keyShowDuration = 0.2f;
    [SerializeField] private float keyInterval = 0.25f;

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
        yield return new WaitForSeconds(showDelay);

        foreach (GameObject key in keys)
        {
            key.transform.DOScale(1f, keyShowDuration).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(keyInterval);
        }
    }
}

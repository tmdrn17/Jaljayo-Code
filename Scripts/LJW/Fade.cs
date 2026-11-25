using System.Collections;
using UnityEngine;

// 씬 시작 때 막을 걷고 스테이지 이름을 잠깐 보여 준다. 플레이어가 닿으면 막을 덮는다.
public class Fade : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private GameObject startingSceneTransition;
    [SerializeField] private GameObject endingSceneTransition;
    [SerializeField] private Animator stageUI;

    void Start()
    {
        startingSceneTransition.SetActive(true);
        StartCoroutine(ShowStageUI());
    }

    IEnumerator ShowStageUI()
    {
        yield return new WaitForSeconds(1f);
        stageUI.SetBool("isShow", true);

        yield return new WaitForSeconds(5f);
        stageUI.SetBool("isShow", false);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(PlayerTag) == false) return;

        endingSceneTransition.SetActive(true);
    }
}

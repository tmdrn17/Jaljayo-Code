using UnityEngine;

// 쪽지 제단(메인홀). 모은 쪽지를 올려 4장을 채우면 완성된 쪽지가 나오고 보스 문이 열린다.
public class Altar : MonoBehaviour
{
    private const string InteractionButton = "Interaction";
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private GameObject[] deadPapers;
    [SerializeField] private GameObject intactPaper;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Animator bossDoor;

    // 올려 둔 쪽지 수, 마지막으로 올린 쪽지의 인덱스
    private int placedCount;
    private int lastIndex;

    void Start()
    {
        int paperNum = GameManager.Instance.PaperNum;

        if (paperNum == -1) lastIndex = paperNum;
        else if (paperNum == 0) lastIndex = paperNum - 1;

        for (int i = 0; i < lastIndex; i++)
        {
            deadPapers[i].SetActive(true);
            placedCount++;
        }
    }

    void Update()
    {
        if (placedCount != deadPapers.Length) return;

        Instantiate(intactPaper, spawnPoint);
        bossDoor.SetBool(OpenHash, true);
        Destroy(gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown(InteractionButton) == false) return;

        if (other.CompareTag(Tags.Player) && GameManager.Instance.PaperNum >= 0)
        {
            lastIndex++;
            deadPapers[lastIndex].SetActive(true);
            GameManager.Instance.HidePaper(lastIndex);
            placedCount++;
        }
    }
}

using UnityEngine;

public enum ColorKind
{
    White,
    Red
}

// 색 발판. 플레이어가 밟고 나가면 검게 변하고 순서를 기록한다. 순서가 틀리면 원래 색으로 돌아간다.
public class ColorBox : MonoBehaviour
{
    private const string AnswerObjectName = "ColorBoxAnswer";

    [SerializeField] private ColorKind colorKind;
    [SerializeField] private int blockNum;

    private Renderer meshRenderer;
    private BoxCollider boxCollider;
    private ColorBoxAnswer answer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
        answer = GameObject.Find(AnswerObjectName).GetComponent<ColorBoxAnswer>();
        // answer를 Start에서 찾으므로 Start/OnDestroy에서 구독한다.
        answer.OnWrongSequence += ResetBlock;
    }

    void OnDestroy()
    {
        if (answer != null) answer.OnWrongSequence -= ResetBlock;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player) == false) return;

        answer.AddNum(blockNum);
        ChangeToBlack();
    }

    private void ChangeToBlack()
    {
        meshRenderer.material.color = Color.black;
        answer.AnswerCheck();
    }

    private void RestoreColor()
    {
        if (colorKind == ColorKind.White) meshRenderer.material.color = Color.white;
        if (colorKind == ColorKind.Red) meshRenderer.material.color = Color.red;

        answer.AnswerCheck();
    }

    private void ResetBlock()
    {
        RestoreColor();
        boxCollider.enabled = true;
    }
}

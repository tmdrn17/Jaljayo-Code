using UnityEngine;

// 동물 석상. 종류와 정답 판정용 값을 가진다. Player가 집고 놓을 때 읽는다.
public class Statue : MonoBehaviour
{
    public enum StatueKind
    {
        Chicken,
        Cow,
        Pig,
        Sheep
    }

    [SerializeField] private StatueKind kind;
    [SerializeField] private int value;   // 제단(AnswerArea)의 answerValue와 같으면 정답

    public StatueKind Kind => kind;
    public int Value => value;
}

using UnityEngine;
using UnityEngine.UI;

// 숫자 키패드 UI. 버튼 OnClick에 PadNumber, Enter, Exit가 연결돼 있다.
public class KeyPad : MonoBehaviour
{
    private const string CorrectMessage = "Correct!";
    private const string WrongMessage = "Wrong!";

    [SerializeField] private Text answerText;
    [SerializeField] private string answer = "122151";

    private bool isWrong;

    public void PadNumber(int number)
    {
        // 틀린 뒤 첫 입력이면 화면을 비우고 시작한다.
        if (isWrong)
        {
            answerText.text = "";
            isWrong = false;
        }
        answerText.text += number.ToString();
    }

    public void Enter()
    {
        if (answerText.text == answer)
        {
            answerText.text = CorrectMessage;
            KeyPadAnswer.RequestShowPaper();
        }
        else
        {
            isWrong = true;
            answerText.text = WrongMessage;
            GameSession.ApplyDamage(1);
        }
    }

    public void Exit()
    {
        GameManager.Instance.ExitPuzzle(true);
    }
}

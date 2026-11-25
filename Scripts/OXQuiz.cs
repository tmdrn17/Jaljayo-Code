using System;
using UnityEngine;
using UnityEngine.UI;

// 퀴즈 한 문제
[Serializable]
public class QuizItem
{
    public string Question;
    public string Answer;
}

// 2층 퀴즈 미니게임. 10문제를 맞히면 미션 클리어, 목숨이 다 떨어지면 피해를 받고 처음부터 다시 한다.
public class OXQuiz : MonoBehaviour
{
    private const string SubmitButton = "Submit";
    private const int ClearAnswerCount = 10;

    [Header("입력·문제")]
    [SerializeField] private InputField inputAnswer;
    [SerializeField] private QuizItem[] quizItems;
    [SerializeField] private Text questionText;

    [Header("표시")]
    [SerializeField] private Text answerCountText;
    [SerializeField] private Text lifeCountText;
    [SerializeField] private Text timeText;

    [Header("제한 시간")]
    [SerializeField] private float timeLimit = 60.0f;
    [SerializeField] private int startLife = 3;

    private float currentTime;
    private int lifeCount;
    private int currentIndex = 0;
    private int correctCount = 0;

    private void Awake()
    {
        lifeCount = startLife;
    }

    private void OnEnable()
    {
        ShowLife();
    }

    private void Start()
    {
        currentTime = timeLimit;
        questionText.text = quizItems[0].Question;
    }

    private void Update()
    {
        if (Input.GetButtonDown(SubmitButton) == false) return;

        if (inputAnswer.text == quizItems[currentIndex].Answer)
        {
            CorrectAnswer();
        }
        else
        {
            WrongAnswer();
        }
        inputAnswer.text = "";
    }

    private void FixedUpdate()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }
        timeText.text = Mathf.Round(currentTime).ToString();
    }

    #region 정답·오답
    private void CorrectAnswer()
    {
        currentIndex++;
        correctCount++;
        answerCountText.text = correctCount.ToString();

        if (currentIndex == ClearAnswerCount)
        {
            SecondFloorManager.AddMissionClear();
            GameManager.Instance.ExitPuzzle(true);
            gameObject.SetActive(false);
            return;
        }

        questionText.text = quizItems[currentIndex].Question;
    }

    private void WrongAnswer()
    {
        lifeCount--;
        if (lifeCount != 0)
        {
            ShowLife();
            return;
        }

        // 목숨이 다 떨어지면 퀴즈를 닫고 처음 상태로 되돌린다
        gameObject.SetActive(false);
        GameManager.Instance.ExitPuzzle(false);
        // TODO(리팩토링): correctCount와 정답 표시(answerCountText)는 다시 시작해도 초기화되지 않는다. 원래 동작 유지.
        // TODO(리팩토링): 다시 시작해도 문제 글자(questionText)는 첫 문제로 돌아가지 않아 글자와 정답이 어긋난다. 원래 동작 유지.
        currentIndex = 0;
        lifeCount = startLife;
        ShowLife();
        currentTime = timeLimit;
        GameSession.ApplyDamage(1);
    }

    private void ShowLife()
    {
        lifeCountText.text = lifeCount.ToString();
    }
    #endregion

    // 닫기 버튼 OnClick에 연결되어 있다.
    public void Exit()
    {
        GameManager.Instance.UnlockMove();
    }
}

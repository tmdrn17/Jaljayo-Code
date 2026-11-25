using UnityEngine;
using UnityEngine.UI;

// 누적 코인 수 표시. 값은 GameSession이 가지고, 여기서는 바뀔 때 글자만 바꾼다.
[RequireComponent(typeof(Text))]
public class TotalScore : MonoBehaviour
{
    private const string LabelPrefix = "       : ";

    private Text totalScoreText;

    private void Awake()
    {
        totalScoreText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        GameSession.OnTotalCoinChanged += HandleTotalCoinChanged;
        HandleTotalCoinChanged(GameSession.TotalCoinAmount);
    }

    private void OnDisable()
    {
        GameSession.OnTotalCoinChanged -= HandleTotalCoinChanged;
    }

    private void HandleTotalCoinChanged(int amount)
    {
        totalScoreText.text = $"{LabelPrefix}{amount}";
    }
}

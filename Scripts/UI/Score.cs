using UnityEngine;
using UnityEngine.UI;

// 이번 판 코인 수 표시. 값은 GameSession이 가지고, 여기서는 바뀔 때 글자만 바꾼다.
[RequireComponent(typeof(Text))]
public class Score : MonoBehaviour
{
    private const string LabelPrefix = "          : ";

    private Text scoreText;

    private void Awake()
    {
        scoreText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        GameSession.OnCoinChanged += HandleCoinChanged;
        HandleCoinChanged(GameSession.CoinAmount);
    }

    private void OnDisable()
    {
        GameSession.OnCoinChanged -= HandleCoinChanged;
    }

    private void HandleCoinChanged(int amount)
    {
        scoreText.text = $"{LabelPrefix}{amount}";
    }
}

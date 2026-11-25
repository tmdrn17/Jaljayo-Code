using UnityEngine;
using UnityEngine.UI;

// 체력 표시. 값은 GameSession이 가지고, 여기서는 바뀔 때 글자만 바꾼다.
[RequireComponent(typeof(Text))]
public class Health : MonoBehaviour
{
    private Text hpText;

    private void Awake()
    {
        hpText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        GameSession.OnHpChanged += HandleHpChanged;
        HandleHpChanged(GameSession.Hp);
    }

    private void OnDisable()
    {
        GameSession.OnHpChanged -= HandleHpChanged;
    }

    private void HandleHpChanged(int hp)
    {
        hpText.text = hp.ToString();
    }
}

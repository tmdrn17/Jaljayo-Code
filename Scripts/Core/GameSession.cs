using System;

// 한 판 동안 유지되는 플레이어 진행 값(체력, 이번 판 코인, 누적 코인). 씬이 바뀌어도 남는다.
// 값은 메서드로만 바꾸고, 바뀔 때마다 이벤트로 알린다. 화면 표시는 Health·Score·TotalScore가 구독해서 한다.
public static class GameSession
{
    public static int Hp { get; private set; }
    public static int CoinAmount { get; private set; }
    public static int TotalCoinAmount { get; private set; }
    public static bool IsDead => (Hp <= 0);

    public static event Action<int> OnHpChanged;
    public static event Action<int> OnCoinChanged;
    public static event Action<int> OnTotalCoinChanged;

    #region 체력
    /// <summary>체력을 깎는다. 0 아래로도 내려간다 (원래 동작).</summary>
    public static void ApplyDamage(int damage)
    {
        SetHp(Hp - damage);
    }

    public static void Heal(int amount)
    {
        SetHp(Hp + amount);
    }

    /// <summary>부팅·새 게임·불러오기 때 체력을 정해진 값으로 맞춘다.</summary>
    public static void SetHp(int hp)
    {
        Hp = hp;
        OnHpChanged?.Invoke(Hp);
    }
    #endregion

    #region 코인
    /// <summary>코인 하나: 이번 판 코인과 누적 코인을 함께 올린다.</summary>
    public static void AddCoin()
    {
        SetCoinAmount(CoinAmount + 1);
        TotalCoinAmount++;
        OnTotalCoinChanged?.Invoke(TotalCoinAmount);
    }

    public static void SetCoinAmount(int amount)
    {
        CoinAmount = amount;
        OnCoinChanged?.Invoke(CoinAmount);
    }

    public static void ResetTotalCoin()
    {
        TotalCoinAmount = 0;
        OnTotalCoinChanged?.Invoke(TotalCoinAmount);
    }
    #endregion
}

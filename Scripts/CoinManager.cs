using UnityEngine;

// 미로 스테이지 코인의 획득 여부를 기억하고, 씬이 열릴 때 이미 먹은 코인을 숨긴다.
// 획득 여부는 씬이 바뀌어도 유지되도록 static으로 둔다.
public class CoinManager : MonoBehaviour
{
    private const int MaxCoinCount = 103;   // 세이브 배열 길이. 씬보다 먼저 static 배열을 만들어야 해서 const로 둔다.

    private static bool[] collectedFlags = new bool[MaxCoinCount];

    [SerializeField] private GameObject[] coins;

    private void Start()
    {
        if (coins.Length > MaxCoinCount)
        {
            Debug.LogWarning($"[{nameof(CoinManager)}] 코인 {coins.Length}개가 저장 칸 {MaxCoinCount}개보다 많습니다.", this);
        }

        // TODO(리팩토링): 조건이 coins.Length + 1이라 마지막 반복에서 IndexOutOfRangeException이 난다. 원래 동작 유지.
        for (int i = 0; i < coins.Length + 1; i++)
        {
            coins[i].SetActive(collectedFlags[i] == false);
        }
    }

    public static bool IsCollected(int index) => collectedFlags[index];

    public static void MarkCollected(int index)
    {
        collectedFlags[index] = true;
    }

    /// <summary>새 게임: 모든 코인을 안 먹은 상태로 되돌린다.</summary>
    public static void ResetCollected()
    {
        for (int i = 0; i < collectedFlags.Length; i++)
        {
            collectedFlags[i] = false;
        }
    }

    /// <summary>저장용. 배열 자체를 돌려준다.</summary>
    public static bool[] GetCollectedFlags() => collectedFlags;

    public static void RestoreCollectedFlags(bool[] flags)
    {
        collectedFlags = flags;
    }
}

using UnityEngine;

// 키패드 정답을 맞추면 쪽지를 보여 준다.
public class KeyPadAnswer : MonoBehaviour
{
    private static bool isPaperRequested;

    [SerializeField] private GameObject paper;

    public static void RequestShowPaper()
    {
        isPaperRequested = true;
    }

    void Update()
    {
        if (isPaperRequested == false) return;

        paper.SetActive(true);
        isPaperRequested = false;
    }
}

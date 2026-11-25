using System;
using System.Collections.Generic;
using UnityEngine;

// 색 발판을 밟은 순서를 기록하고 정답과 비교해 문을 연다. 순서가 틀리면 OnWrongSequence로 알린다.
public class ColorBoxAnswer : MonoBehaviour
{
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private Animator door;
    [SerializeField] private List<int> correctNum = new List<int>();
    [SerializeField] private AudioClip openSound;

    private readonly List<int> savedNum = new List<int>();
    private AudioSource audioSource;

    public event Action OnWrongSequence;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AddNum(int blockNum)
    {
        savedNum.Add(blockNum);
    }

    public void AnswerCheck()
    {
        if (savedNum.Count != correctNum.Count) return;

        if (IsCorrect())
        {
            DoorAnimation.MarkOpen();
            door.SetBool(OpenHash, true);
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            // 먼저 비워야 이벤트 안에서 다시 부르는 AnswerCheck가 바로 빠져나온다.
            savedNum.Clear();
            OnWrongSequence?.Invoke();
        }
    }

    private bool IsCorrect()
    {
        for (int i = 0; i < savedNum.Count; i++)
        {
            if (savedNum[i] != correctNum[i]) return false;
        }
        return true;
    }
}

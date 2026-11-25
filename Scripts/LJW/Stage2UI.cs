using UnityEngine;
using UnityEngine.UI;

// 2층 UI 관리. 퀘스트 창, 점수 표시, 미션 완료 이미지를 다룬다.
public class Stage2UI : MonoBehaviour
{
    private const string FourthRoomSpawnName = "4thRoomSpawn";
    private const int ItemMissionCount = 5;
    private const int DragMissionCount = 2;

    [Header("퀘스트")]
    [SerializeField] private GameObject[] quest;
    [SerializeField] private GameObject doingButton;
    [SerializeField] private GameObject questExplain;
    [SerializeField] private GameObject document;

    [Header("점수·클리어 표시")]
    [SerializeField] private Text score;
    [SerializeField] private Image clearImage3;
    [SerializeField] private Image clearImage4;
    [SerializeField] private Text roomEscape;

    private int correct = 0;

    /// <summary>번호에 맞는 퀘스트 창을 연다.</summary>
    public void ShowQuest(int index)
    {
        quest[index].SetActive(true);
    }

    #region 창 닫기·수락
    // 버튼 OnClick에 연결되어 있다.
    public void Close()
    {
        CloseQuest(0);
    }

    // 버튼 OnClick에 연결되어 있다.
    public void Close2()
    {
        CloseQuest(1);
    }

    // 수락 버튼 OnClick: 미션 내용/진행 버튼을 표시한다.
    public void Respawn()
    {
        document.SetActive(true);
        questExplain.SetActive(true);
        doingButton.SetActive(true);
    }

    private void CloseQuest(int index)
    {
        quest[index].SetActive(false);
        GameManager.Instance.ExitPuzzle(false);
    }
    #endregion

    #region 미션 진행
    /// <summary>문서를 하나 모았을 때 Player가 부른다. collectedCount는 늘린 뒤의 개수.</summary>
    public void ChangePoint(int collectedCount)
    {
        score.text = collectedCount.ToString();

        if (collectedCount != ItemMissionCount) return;

        SecondFloorManager.AddMissionClear();
        clearImage3.gameObject.SetActive(true);
    }

    /// <summary>끌어 놓기 퍼즐에서 정답을 하나 맞혔을 때 부른다.</summary>
    public void AddCorrect()
    {
        correct++;
        roomEscape.text = correct.ToString();
        Complete();
    }

    private void Complete()
    {
        if (correct != DragMissionCount) return;

        SecondFloorManager.AddMissionClear();
        clearImage4.gameObject.SetActive(true);
    }

    // 버튼 OnClick에 연결되어 있다.
    public void RoomRespawn()
    {
        Vector3 respawn = GameObject.Find(FourthRoomSpawnName).transform.position;
        GameManager.Instance.MovePlayer(respawn);
    }
    #endregion
}

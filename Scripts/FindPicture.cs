using UnityEngine;
using UnityEngine.UI;

// 틀린 그림 찾기 미니게임. 클릭한 곳에 O/X 표시를 만들고 기회를 센다.
public class FindPicture : MonoBehaviour
{
    private const string ImageTag = "Image";
    private const string RightButtonTag = "RightButton";
    private const string FindCanvasName = "FindCanvas";
    private const int WrongKind = 1;
    private const int RightKind = 2;

    [SerializeField] private Image wrongImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private GameObject findCanvas;

    [Header("조정 값")]
    [SerializeField] private int startChance = 3;

    private int chance;

    private void Awake()
    {
        chance = startChance;
    }

    /// <summary>미니게임을 시작할 때 기회를 채우고 남은 표시를 지운다.</summary>
    public void ResetPicture()
    {
        chance = startChance;
        DestroyMarks();
    }

    #region 버튼 입력
    // 버튼 OnClick에 연결되어 있다. kind: 1 = 틀린 곳, 2 = 맞는 곳
    public void ImageCreat(int kind)
    {
        if (chance == 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) == false) return;

        Transform canvas = GameObject.Find(FindCanvasName).transform;
        if (kind == WrongKind)
        {
            Instantiate(wrongImage, Input.mousePosition, Quaternion.identity, canvas);
        }
        if (kind == RightKind)
        {
            Instantiate(rightImage, Input.mousePosition, Quaternion.identity, canvas);
        }
    }

    public void WrongClick()
    {
        chance--;
        if (chance != 0) return;

        DestroyMarks();
        GameSession.ApplyDamage(1);
        findCanvas.SetActive(false);
        GameManager.Instance.ExitPuzzle(false);
    }

    public void RightClick()
    {
        // 맞는 곳 버튼이 하나만 남았을 때(마지막 정답)만 클리어
        if (GameObject.FindGameObjectsWithTag(RightButtonTag).Length != 1) return;

        SecondFloorManager.AddMissionClear();
        GameManager.Instance.ExitPuzzle(true);
        findCanvas.SetActive(false);
    }
    #endregion

    private void DestroyMarks()
    {
        GameObject[] marks = GameObject.FindGameObjectsWithTag(ImageTag);
        for (int i = 0; i < marks.Length; i++)
        {
            Destroy(marks[i]);
        }
    }
}

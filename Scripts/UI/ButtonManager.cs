using UnityEngine;
using UnityEngine.SceneManagement;

// 버튼 OnClick에서 호출되는 메뉴 동작 모음. 메서드 이름은 버튼 연결이 걸려 있어 바꾸지 않는다.
public class ButtonManager : MonoBehaviour
{
    [SerializeField] private float tutorialDelay = 2f;

    public string NextSceneName;

    public void NextStageButton()
    {
        Cursor.visible = false;
        SceneManager.LoadScene(NextSceneName);
    }

    public void NewStartButton()
    {
        // 1. 새 게임 값으로 초기화
        GameManager.Instance.ResetForNewGame();
        Cursor.visible = false;

        // 2. 잠깐 뒤 튜토리얼로 이동
        Invoke(nameof(LoadTutorial), tutorialDelay);
    }

    private void LoadTutorial()
    {
        SceneManager.LoadScene(SceneNames.Tutorial);
    }

    public void CountinueButton()
    {
        GameManager.Instance.Resume();
    }

    public void TitleButton()
    {
        GameManager.Instance.ResetPause();
        SceneManager.LoadScene(SceneNames.Title);
    }

    public void SaveButton()
    {
        SaveManager.Instance.SaveData();
    }

    public void LoadButton()
    {
        if (SaveManager.Instance.TryLoadData() == false) return;

        Cursor.visible = false;
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

// 스테이지 선택 화면 버튼 동작. 메서드 이름은 버튼 OnClick 연결이라 바꾸지 않는다.
public class StageSelect : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene(SceneNames.StageSelect);
    }

    public void Title()
    {
        SceneManager.LoadScene(SceneNames.Title);
        Time.timeScale = 1;
        GameSession.SetCoinAmount(0);
        GameSession.ResetTotalCoin();
    }

    public void Stage1()
    {
        SceneManager.LoadScene(SceneNames.Stage1);
    }

    public void Stage2()
    {
        SceneManager.LoadScene(SceneNames.Stage2);
    }

    public void Stage3()
    {
        SceneManager.LoadScene(SceneNames.Stage3);
    }

    public void Stage4()
    {
        SceneManager.LoadScene(SceneNames.Stage4);
    }

    public void Stage5()
    {
        SceneManager.LoadScene(SceneNames.Stage5);
    }

    public void Cancel()
    {
        SceneManager.LoadScene(SceneNames.Title);
    }
}

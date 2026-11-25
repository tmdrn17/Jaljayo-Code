using UnityEngine;
using UnityEngine.SceneManagement;

// 스테이지 선택 화면 버튼 동작. 메서드 이름은 버튼 OnClick 연결이라 바꾸지 않는다.
public class StageSelect : MonoBehaviour
{
    private const string StageSelectScene = "StageSelect";
    private const string Stage1Scene = "Stage1";
    private const string Stage2Scene = "Stage2";
    private const string Stage3Scene = "Stage3";
    private const string Stage4Scene = "Stage4";
    private const string Stage5Scene = "Stage5";

    public void SceneChange()
    {
        SceneManager.LoadScene(StageSelectScene);
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
        SceneManager.LoadScene(Stage1Scene);
    }

    public void Stage2()
    {
        SceneManager.LoadScene(Stage2Scene);
    }

    public void Stage3()
    {
        SceneManager.LoadScene(Stage3Scene);
    }

    public void Stage4()
    {
        SceneManager.LoadScene(Stage4Scene);
    }

    public void Stage5()
    {
        SceneManager.LoadScene(Stage5Scene);
    }

    public void Cancel()
    {
        SceneManager.LoadScene(SceneNames.Title);
    }
}

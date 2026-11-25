using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 남은 시간을 줄이며 화면에 표시하고, 0이 되면 GameManager에 게임 오버를 알린다.
// 씬별 제한 시간은 GameManager가 씬 로드 때 정한다.
public class CountDown : MonoBehaviour
{
    // 빌드에 없는 씬 이름
    private const string MainHallScene = "MainHall";
    private const string TestSceneName = "TestScene";

    [SerializeField] private Text countdownText;

    private void Start()
    {
        countdownText.text = GameManager.Instance.RemainTime.ToString();
    }

    private void FixedUpdate()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // 1. 시간이 흐르는 씬이면 줄이고, 0 이하가 되면 게임 오버 (GameManager가 한 번만 처리한다)
        if (IsTimerPausedScene(sceneName) == false)
        {
            if (GameManager.Instance.RemainTime > 0)
            {
                GameManager.Instance.ReduceRemainTime(Time.fixedDeltaTime);
            }
            else
            {
                GameManager.Instance.GameOver();
            }
            countdownText.text = Mathf.Round(GameManager.Instance.RemainTime).ToString();
        }

        // 2. 숫자를 숨길 씬에서는 텍스트를 끈다
        countdownText.enabled = (IsTextHiddenScene(sceneName) == false);
    }

    private bool IsTimerPausedScene(string sceneName)
    {
        return sceneName == MainHallScene || sceneName == SceneNames.Title || sceneName == SceneNames.GameFail
            || sceneName == SceneNames.BossRoom || sceneName == TestSceneName || sceneName == SceneNames.SecondFloor;
    }

    private bool IsTextHiddenScene(string sceneName)
    {
        return sceneName == MainHallScene || sceneName == SceneNames.Title || sceneName == TestSceneName
            || sceneName == SceneNames.BossRoom || sceneName == SceneNames.SecondFloor;
    }
}

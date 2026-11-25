using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어가 닿으면 잠시 뒤 지정한 씬으로 넘어간다.
public class SceneChanger : MonoBehaviour
{
    [SerializeField] private float loadDelay = 2.5f;

    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.Player) == false) return;

        Invoke(nameof(LoadNextScene), loadDelay);

        if (SceneManager.GetActiveScene().name == SceneNames.StageFail)
        {
            GameSession.ResetTotalCoin();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

// 플레이어가 닿으면 커서를 보이고 클리어 씬으로 넘어간다.
public class StageClear : MonoBehaviour
{
    [SerializeField] private string clearSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tags.Player) == false) return;

        Cursor.visible = true;
        SceneManager.LoadScene(clearSceneName);
    }
}

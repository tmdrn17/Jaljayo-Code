using UnityEngine;

// 타이틀에서 다음 화면으로 넘어갈 때 막을 덮는다. TitleFadeOut은 버튼 OnClick에 연결돼 있다.
public class FadeOut : MonoBehaviour
{
    [SerializeField] private GameObject endingSceneTransition;

    public void TitleFadeOut()
    {
        endingSceneTransition.SetActive(true);
    }
}

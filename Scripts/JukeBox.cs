using UnityEngine;

// 주크박스. 가까이에서 상호작용 버튼을 누르면 음악을 재생한다.
public class JukeBox : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(Tags.Player) == false) return;

        if (Input.GetButtonDown(InputNames.Interaction)) audioSource.Play();
    }
}

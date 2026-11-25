using UnityEngine;

// 제자리에서 빙글빙글 도는 표창 장식
public class Throwingstar : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 750f;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotateSpeed * Time.deltaTime));
    }
}

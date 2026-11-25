using UnityEngine;

// 하늘 배경을 시간에 따라 천천히 회전시킨다.
public class Skybox : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 0.3f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
    }
}

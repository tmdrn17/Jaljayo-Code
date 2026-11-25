using System;
using UnityEngine;

// 퍼즐을 풀면 문 콜라이더를 끈다. 열림 여부는 모든 문이 공유하는 static 값이다.
public class DoorAnimation : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    private static event Action OnOpened;

    [SerializeField] private Collider boxCollider;

    public static void MarkOpen()
    {
        IsOpen = true;
        OnOpened?.Invoke();
    }

    private void OnEnable()
    {
        OnOpened += DisableCollider;
        // 문이 켜지기 전에 이미 열렸을 수도 있으니 한 번 확인한다.
        if (IsOpen) DisableCollider();
    }

    private void OnDisable()
    {
        OnOpened -= DisableCollider;
    }

    private void DisableCollider()
    {
        boxCollider.enabled = false;
    }
}

using System;
using UnityEngine;

// 2층 미션 클리어 개수를 세고, 4개를 채우면 클리어 열쇠를 생성한다.
public class SecondFloorManager : MonoBehaviour
{
    [SerializeField] private int clearMissionCount = 4;
    [SerializeField] private GameObject secondFloorClear;
    [SerializeField] private Transform spawnPoint;

    public static int MissionClearNum { get; private set; } = 0;

    private static event Action OnMissionCleared;

    /// <summary>미션 하나를 클리어했을 때 부른다.</summary>
    public static void AddMissionClear()
    {
        MissionClearNum++;
        OnMissionCleared?.Invoke();
    }

    private void OnEnable()
    {
        OnMissionCleared += HandleMissionCleared;
        // 이 씬이 열리기 전에 이미 다 채웠을 수도 있으니 한 번 확인한다.
        HandleMissionCleared();
    }

    private void OnDisable()
    {
        OnMissionCleared -= HandleMissionCleared;
    }

    private void HandleMissionCleared()
    {
        if (MissionClearNum != clearMissionCount) return;

        Instantiate(secondFloorClear, spawnPoint);
        MissionClearNum = 0;
    }
}

// 코드에서 쓰는 씬 이름. 값은 씬 파일 이름과 같다.
public static class SceneNames
{
    // 빌드 세팅(EditorBuildSettings)에 있는 씬
    public const string Boot = "DontDestruyObjsScene";
    public const string Title = "Title";
    public const string Tutorial = "Tutorial";
    public const string MazeStage = "MazeStage";
    public const string PortalStage = "PortalStage";
    public const string RunningStage = "RunningStage";
    public const string EscapeStage = "EscapeStage";
    public const string BossRoom = "BossRoom";
    public const string SecondFloor = "2Floor";
    public const string FirstStageClear = "New_1StageClear";
    public const string SecondStageClear = "New_2StageClear";
    public const string ThirdStageClear = "New_3StageClear";
    public const string FourthStageClear = "New_4StageClear";
    public const string BossClear = "1FBossClear";
    public const string GameFail = "New_GameFail";
    public const string GameClear = "GameClear";

    // 빌드 세팅에 없는 씬 (옛 스테이지 선택·테스트용). 코드가 이름을 비교하거나 불러오므로 남겨 둔다.
    public const string MainHall = "MainHall";
    public const string TestScene = "TestScene";
    public const string StageFail = "StageFail";
    public const string StageSelect = "StageSelect";
    public const string Stage1 = "Stage1";
    public const string Stage2 = "Stage2";
    public const string Stage3 = "Stage3";
    public const string Stage4 = "Stage4";
    public const string Stage5 = "Stage5";
}

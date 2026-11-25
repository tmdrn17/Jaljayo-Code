using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// 저장 파일 형식. 필드 이름을 바꾸면 기존 세이브 JSON이 깨지므로 그대로 둔다.
[Serializable]
public class SaveList
{
    public int HP;
    public float Time;
    public int Score;
    public int PaperNum;
    public bool[] Clear;
    public string SceneName;
    public bool[] CoinCheck;
    public Vector3 SavePointPos;
}

// 진행 상황을 JSON 파일로 저장하고 불러온다. GameManager와 같은 오브젝트에 붙어 있다.
public class SaveManager : SingletonBase<SaveManager>
{
    private const string SaveFolderName = "/Save/";
    private const string SaveFileName = "/SaveFile.txt";

    private SaveList save = new SaveList();

    public string SaveDataDir { get; private set; }
    public string SaveFilePath => SaveDataDir + SaveFileName;
    public bool HasSaveFile => File.Exists(SaveFilePath);

    protected override void OnInitialize()
    {
        PrepareSaveFolder();
    }

    // 1. persistentDataPath에 폴더를 만든다.
    // 2. 예전 위치(Application.dataPath/Save)에 세이브가 있으면 한 번 복사해 온다.
    private void PrepareSaveFolder()
    {
        SaveDataDir = Application.persistentDataPath + SaveFolderName;
        if (Directory.Exists(SaveDataDir) == false)
        {
            Directory.CreateDirectory(SaveDataDir);
        }

        string oldFilePath = Application.dataPath + SaveFolderName + SaveFileName;
        if (File.Exists(SaveFilePath) == false && File.Exists(oldFilePath))
        {
            File.Copy(oldFilePath, SaveFilePath);
        }
    }

    // 버튼 OnClick 연결
    public void SaveData()
    {
        GameManager gameManager = GameManager.Instance;

        // 코인 상태는 코인이 있는 미로 스테이지에서만 갱신한다.
        if (SceneManager.GetActiveScene().name == SceneNames.MazeStage)
        {
            save.CoinCheck = CoinManager.GetCollectedFlags();
        }
        save.HP = GameSession.Hp;
        save.Clear = gameManager.GetFloorClearForSave();
        save.Score = GameSession.CoinAmount;
        save.Time = gameManager.RemainTime;
        save.PaperNum = gameManager.PaperNum;
        save.SceneName = SceneManager.GetActiveScene().name;
        save.SavePointPos = gameManager.PlayerPosition;

        try
        {
            File.WriteAllText(SaveFilePath, JsonUtility.ToJson(save));
        }
        catch (IOException e)
        {
            Debug.LogWarning($"[{nameof(SaveManager)}] 저장 실패: {e.Message}", this);
        }
    }

    /// <summary>세이브를 읽어 적용하고 저장된 씬을 연다. 파일이 없거나 읽지 못하면 false.</summary>
    public bool TryLoadData()
    {
        if (HasSaveFile == false)
        {
            Debug.LogWarning($"[{nameof(SaveManager)}] 세이브 파일이 없습니다.", this);
            return false;
        }

        // 1. 파일을 읽는다. 실패하면 아무것도 바꾸지 않는다.
        SaveList loaded;
        try
        {
            loaded = JsonUtility.FromJson<SaveList>(File.ReadAllText(SaveFilePath));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[{nameof(SaveManager)}] 불러오기 실패: {e.Message}", this);
            return false;
        }
        if (loaded == null) return false;
        save = loaded;

        GameManager gameManager = GameManager.Instance;

        // 2. 값을 되돌린다.
        GameSession.SetHp(save.HP);
        GameSession.SetCoinAmount(save.Score);
        // TODO(리팩토링): 미로 스테이지 밖에서만 저장한 세이브는 CoinCheck가 빈 배열이라, 불러온 뒤 코인을 먹으면 범위 밖 예외가 난다.
        CoinManager.RestoreCollectedFlags(save.CoinCheck);
        gameManager.RestoreFloorClear(save.Clear);
        gameManager.SetRemainTime(save.Time);
        gameManager.RestorePapers(save.PaperNum);

        // 3. 저장된 씬을 열고, 로드 뒤 저장 위치에 플레이어를 놓는다.
        gameManager.MarkLoadedFromSave(save.SavePointPos);
        SceneManager.LoadScene(save.SceneName);
        return true;
    }
}

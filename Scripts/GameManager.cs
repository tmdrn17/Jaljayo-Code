using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임 전체 진행(씬 로드 후 배치, 일시정지, 퍼즐 창, 제한 시간, 쪽지, 1층 클리어, 게임 오버)을 관리한다. 씬이 바뀌어도 유지된다.
public class GameManager : SingletonBase<GameManager>
{
    private const string SpawnPointTag = "SpawnPoint";
    private const string CancelButtonName = "Cancel";

    // 게임 UI(캔버스·얼굴 캠)를 숨기는 씬: 타이틀, 클리어, 실패
    private static readonly HashSet<string> MenuScenes = new HashSet<string>
    {
        SceneNames.Title,
        SceneNames.FirstStageClear, SceneNames.SecondStageClear, SceneNames.ThirdStageClear, SceneNames.FourthStageClear,
        SceneNames.BossClear, SceneNames.GameFail
    };

    // 커서를 보이는 씬: 2층, 클리어
    private static readonly HashSet<string> CursorVisibleScenes = new HashSet<string>
    {
        SceneNames.SecondFloor,
        SceneNames.FirstStageClear, SceneNames.SecondStageClear, SceneNames.ThirdStageClear, SceneNames.FourthStageClear,
        SceneNames.BossClear
    };

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject faceCam;

    [Header("캐릭터")]
    [SerializeField] private GameObject cameraArm;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject characterLight;

    [Header("진행")]
    [SerializeField] private GameObject[] papers;
    [SerializeField] private bool[] firstFloorClear = new bool[4];

    [Header("체력")]
    [SerializeField] private int bootHp = 10;        // 게임을 켰을 때 (옛 Health.startHp 씬 값)
    [SerializeField] private int newGameHp = 3;      // 새 게임 (옛 ButtonManager.NewStartHp)
    [SerializeField] private int healCoinCount = 10; // 코인 이만큼마다 체력 1 회복

    [Header("제한 시간")]
    [SerializeField] private float defaultTimeLimit = 100f;
    [SerializeField] private float shortStageTimeLimit = 260f;   // 미로·포탈
    [SerializeField] private float longStageTimeLimit = 350f;    // 러닝·탈출

    // 런타임 상태 (인스펙터에 두지 않는다. 씬에 남아 있던 테스트 값 paperNum 4, remainTime 1e+9가 더는 적용되지 않는다)
    private int paperNum;
    private float remainTime;
    private int healthCoinCount;
    private bool isGameOverLoading;
    private Vector3 loadedPosition;
    private Rigidbody playerRigidbody;

    public bool IsPause { get; private set; }
    public bool IsLoad { get; private set; }
    public bool IsMoveLocked { get; private set; }
    public bool HasEnvelope { get; private set; }
    public GameObject SpawnPoint { get; private set; }
    public int PaperNum => paperNum;
    public float RemainTime => remainTime;
    public bool HasAllPapers => (paperNum == papers.Length);
    public Vector3 PlayerPosition => playerCharacter.transform.position;

    protected override bool IsPersistent => true;

    protected override void OnInitialize()
    {
        IsPause = false;
        playerRigidbody = playerCharacter.GetComponent<Rigidbody>();

        DontDestroyOnLoad(canvas);
        DontDestroyOnLoad(cameraArm);
        DontDestroyOnLoad(playerCharacter);
        DontDestroyOnLoad(faceCam);

        // 씬을 넘어가며 유지되는 싱글톤이라 OnEnable/OnDisable 대신 생성/파괴에 맞춰 구독한다.
        GameSession.SetHp(bootHp);
        GameSession.OnHpChanged += HandleHpChanged;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        SceneManager.LoadScene(SceneNames.Title);
    }

    protected override void OnDestroy()
    {
        GameSession.OnHpChanged -= HandleHpChanged;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        base.OnDestroy();
    }

    private void Update()
    {
        HandlePauseInput();
    }

    #region 씬 로드
    // 단계 순서를 바꾸지 않는다. 앞 단계에서 예외가 나면 뒤 단계가 실행되지 않던 원래 동작과 같게 둔다.
    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // sceneLoaded는 시작 씬에서도 불린다. 예전 OnLevelWasLoaded는 시작 씬에서 불리지 않았으므로 건너뛴다.
        if (scene.name == SceneNames.Boot) return;

        isGameOverLoading = false;

        // 1. 플레이어·카메라를 저장 위치 또는 스폰 위치로 옮긴다.
        PlaceCharacter();

        // 2. 제한 시간 초기화
        remainTime = GetTimeLimit(scene.name);

        // 3. 씬별 상태
        if (scene.name == SceneNames.Title)
        {
            IsPause = false;
        }

        characterLight.SetActive(scene.name == SceneNames.MazeStage);

        // 4. 타이틀·클리어·실패 씬에서는 게임 UI를 숨긴다.
        bool isMenuScene = MenuScenes.Contains(scene.name);
        canvas.gameObject.SetActive(isMenuScene == false);
        faceCam.gameObject.SetActive(isMenuScene == false);

        // 5. 2층과 클리어 씬에서는 커서를 보인다.
        if (CursorVisibleScenes.Contains(scene.name))
        {
            Cursor.visible = true;
        }
    }

    private void PlaceCharacter()
    {
        SpawnPoint = GameObject.FindWithTag(SpawnPointTag);

        if (IsLoad)
        {
            MoveCharacter(loadedPosition);
            IsLoad = false;
        }
        else
        {
            MoveCharacter(SpawnPoint.transform.position);
        }

        // 이전 씬에서 떨어지던 속도가 남아 있으면 새 씬 바닥을 뚫고 떨어진다.
        // (클리어 씬은 스폰 위치가 공중이라 클리어 화면 동안 계속 낙하한다)
        playerRigidbody.velocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>다음 씬 로드 때 저장된 위치에 플레이어를 놓도록 표시한다.</summary>
    public void MarkLoadedFromSave(Vector3 savedPosition)
    {
        IsLoad = true;
        loadedPosition = savedPosition;
    }
    #endregion

    #region 캐릭터 이동
    /// <summary>플레이어와 카메라를 함께 옮긴다.</summary>
    public void MoveCharacter(Vector3 position)
    {
        playerCharacter.transform.position = position;
        cameraArm.transform.position = position;
    }

    /// <summary>플레이어만 옮긴다. 카메라는 따라가며 이동한다.</summary>
    public void MovePlayer(Vector3 position)
    {
        playerCharacter.transform.position = position;
    }
    #endregion

    #region 일시정지
    private void HandlePauseInput()
    {
        if (Input.GetButtonDown(CancelButtonName) == false) return;

        if (IsPause == false)
        {
            PauseGame();
        }
        else
        {
            Resume();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        IsPause = true;
        Wall.WallMove = false;
        pauseMenu.SetActive(true);
        Cursor.visible = true;
    }

    /// <summary>일시정지를 풀고 메뉴를 닫는다 (Esc, "계속" 버튼).</summary>
    public void Resume()
    {
        Time.timeScale = 1;
        IsPause = false;
        Wall.WallMove = true;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
    }

    /// <summary>일시정지 표시만 끈다 (시간·메뉴는 건드리지 않음).</summary>
    public void ResetPause()
    {
        IsPause = false;
    }

    // 버튼 OnClick 연결
    public void GameContinue()
    {
        IsPause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    // 버튼 OnClick 연결
    public void GameExit()
    {
        Application.Quit();
    }
    #endregion

    #region 퍼즐 창
    /// <summary>퍼즐·대화 창을 열 때: 시간을 멈추고 커서를 보인다. lockMove면 이동·카메라 회전도 막는다.</summary>
    public void EnterPuzzle(bool lockMove)
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        if (lockMove) IsMoveLocked = true;
    }

    /// <summary>퍼즐 창을 닫을 때: 시간을 되돌리고 이동을 푼다. hideCursor면 커서도 숨긴다.</summary>
    public void ExitPuzzle(bool hideCursor)
    {
        Time.timeScale = 1f;
        IsMoveLocked = false;
        if (hideCursor) Cursor.visible = false;
    }

    /// <summary>퍼즐 창이 닫힌 것을 보고 이동을 풀 때 쓴다 (Player.Update의 동작).</summary>
    public void UnlockMove()
    {
        IsMoveLocked = false;
    }
    #endregion

    #region 제한 시간
    private float GetTimeLimit(string sceneName)
    {
        switch (sceneName)
        {
            case SceneNames.MazeStage:
            case SceneNames.PortalStage:
                return shortStageTimeLimit;
            case SceneNames.RunningStage:
            case SceneNames.EscapeStage:
                return longStageTimeLimit;
            default:
                return defaultTimeLimit;
        }
    }

    public void SetRemainTime(float time)
    {
        remainTime = time;
    }

    public void ReduceRemainTime(float deltaTime)
    {
        remainTime -= deltaTime;
    }
    #endregion

    #region 쪽지
    /// <summary>다음 쪽지 UI를 켜고 쪽지 수를 늘린다.</summary>
    public void CollectPaper()
    {
        papers[paperNum].SetActive(true);
        paperNum++;
    }

    public void HidePaper(int index)
    {
        papers[index].SetActive(false);
    }

    /// <summary>새 게임: 쪽지 수를 0으로 하고 쪽지 UI를 모두 끈다.</summary>
    public void ResetPapers()
    {
        paperNum = 0;
        for (int i = 0; i < papers.Length; i++)
        {
            papers[i].SetActive(false);
        }
    }

    /// <summary>불러오기: 저장된 쪽지 수만큼 쪽지 UI를 켠다.</summary>
    public void RestorePapers(int count)
    {
        paperNum = count;
        if (paperNum < 0) return;

        for (int i = 0; i < count; i++)
        {
            papers[i].SetActive(true);
        }
    }

    /// <summary>봉투를 주웠다고 표시한다. 지금은 읽는 곳이 없다.</summary>
    public void PickEnvelope()
    {
        HasEnvelope = true;
    }
    #endregion

    #region 1층 클리어
    public bool IsFloorClear(int index) => firstFloorClear[index];

    public void SetFloorClear(int index)
    {
        firstFloorClear[index] = true;
    }

    /// <summary>저장용. 배열 자체를 돌려준다.</summary>
    public bool[] GetFloorClearForSave() => firstFloorClear;

    public void RestoreFloorClear(bool[] clearState)
    {
        firstFloorClear = clearState;
    }
    #endregion

    #region 코인·새 게임
    /// <summary>코인 하나를 먹었을 때 Coin이 부른다. 점수를 올리고, healCoinCount개마다 체력 1 회복.</summary>
    public void CollectCoin()
    {
        GameSession.AddCoin();

        healthCoinCount++;
        if (healthCoinCount < healCoinCount) return;

        GameSession.Heal(1);
        healthCoinCount = 0;
    }

    /// <summary>새 게임: 쪽지·체력·점수·코인을 처음 값으로 되돌린다.</summary>
    public void ResetForNewGame()
    {
        ResetPapers();
        healthCoinCount = 0;
        GameSession.SetHp(newGameHp);
        GameSession.SetCoinAmount(0);
        CoinManager.ResetCollected();
    }
    #endregion

    #region 게임 오버
    /// <summary>체력 0, 시간 0, 낙사 때 부른다. 실패 씬은 한 번만 연다.</summary>
    public void GameOver()
    {
        if (isGameOverLoading) return;

        isGameOverLoading = true;
        SceneManager.LoadScene(SceneNames.GameFail);
    }

    private void HandleHpChanged(int hp)
    {
        if (GameSession.IsDead) GameOver();
    }
    #endregion
}

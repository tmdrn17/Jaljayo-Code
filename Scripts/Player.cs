using System.Collections;
using UnityEngine;

// 플레이어 이동·점프·공격·상호작용·피격을 처리한다.
public class Player : MonoBehaviour
{
    #region 상수
    private const string StatueTag = "Statue";
    private const string AnswerAreaTag = "AnswerArea";
    private const string AnswerButtonTag = "AnswerButton";
    private const string KeyPadTag = "KeyPad";
    private const string ButtonTag = "Button";
    private const string WaterTag = "Water";
    private const string JumpItemTag = "Item_Jump";
    private const string DocumentItemTag = "Item_document";
    private const string EnemyTag = "Enemy";
    private const string GroundTag = "Ground";

    private const string GateOpenLeverName = "GateOpenLaber";
    private const string FirstNpcName = "Npc3";
    private const string SecondNpcName = "Npc4";
    private const string FloorRoomName = "4thRoomFloor";
    private const string OXQuizName = "OXQuiz";
    private const string FindPictureName = "FindPicture";
    private const string AltarName = "Altar";
    private const string UIManagerName = "UIManager";

    private static readonly int WalkHash = Animator.StringToHash("IsWalk");
    private static readonly int RunHash = Animator.StringToHash("IsRun");
    private static readonly int JumpHash = Animator.StringToHash("IsJump");
    private static readonly int AttackHash = Animator.StringToHash("IsAttack");
    private static readonly int BeShotHash = Animator.StringToHash("BeShot");
    #endregion

    #region 필드
    [Header("참조")]
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject keyPad;
    [SerializeField] private GameObject oxQuiz;
    [SerializeField] private GameObject findCanvas;
    [SerializeField] private ParticleSystem runEffect;
    [SerializeField] private ParticleSystem jumpEffect;

    [Header("이동·점프")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpPower = 15f;
    [SerializeField] private float runningJumpPower = 4f;
    [SerializeField] private float walkJumpForwardPower = 1.5f;

    [Header("피격·낙사")]
    [SerializeField] private float damageInvincibleTime = 1f;
    [SerializeField] private float fallLimitY = -9f; // 이 높이보다 떨어지면 게임 오버

    [Header("공격")]
    [SerializeField] private Transform bulletPos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletSpeed;
    [SerializeField] private float attackCoolTime = 0.5f;

    [Header("사운드")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip damagedSound;

    [Header("상호작용")]
    [SerializeField] private GameObject[] animals;
    [SerializeField] private bool[] nearNpc;

    public Rigidbody Rig { get; private set; }
    public GameObject NearObj { get; private set; }
    public bool IsJump { get; private set; }
    public bool IsMovePanel { get; private set; }
    public int ItemIndex { get; private set; }

    private Animator anim;
    private AudioSource audioSource;
    private MeshRenderer[] meshes;
    private Stage2UI uiManager;

    private float haxis;
    private float vaxis;
    private bool isJumpDown;
    private bool isInteractDown;

    private Vector3 moveInput;
    private float moveSpeed;
    private int jumpCount;
    private float curTime;

    private bool isRun;
    private bool isWalk;
    private bool isDamaged;
    private bool hasStatue;
    private bool isAreaIn;
    #endregion

    #region 유니티 이벤트
    void Awake()
    {
        Rig = GetComponent<Rigidbody>();
        moveSpeed = walkSpeed;
        anim = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        jumpCount = 0;
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsMoveLocked == false) Move();

        if (GameManager.Instance.HasAllPapers)
        {
            if (Input.GetMouseButtonDown(0)) Attack();
        }
    }

    void Update()
    {
        ReadInput();
        Interaction();
        CheckFall();
        Jump();
        Run();

        if (curTime > 0) curTime -= Time.deltaTime;

        // TODO(리팩토링): ||라서 둘 중 하나만 꺼져 있어도 이동이 풀린다 (원래 동작 유지)
        if (oxQuiz.activeSelf == false || findCanvas.activeSelf == false)
        {
            GameManager.Instance.UnlockMove();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(JumpItemTag)) EnterJumpItem();
        if (other.name == GateOpenLeverName) Destroy(wall);
        if (other.CompareTag(AnswerAreaTag)) isAreaIn = true;
        if (other.CompareTag(WaterTag)) transform.position = GameManager.Instance.SpawnPoint.transform.position;
        if (other.CompareTag(Tags.Bullet)) EnterBullet(other);
        if (other.CompareTag(DocumentItemTag)) EnterDocument(other.gameObject);
        if (other.CompareTag(Tags.Spike) || other.CompareTag(Tags.SliceWall)) EnterSpike();
    }

    void OnTriggerStay(Collider other)
    {
        StayStatue(other);
        StayKeyPad(other);
        StayNpc(other);
        StayQuiz(other);

        if (other.gameObject.name == FloorRoomName) Cursor.visible = true;
        if (other.CompareTag(ButtonTag)) StayButton(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(AnswerAreaTag)) isAreaIn = false;
        if (other.CompareTag(StatueTag)) NearObj = null;

        if (other.gameObject.name == FirstNpcName) nearNpc[0] = false;
        if (other.gameObject.name == SecondNpcName) nearNpc[1] = false;
        if (other.gameObject.name == FloorRoomName) Cursor.visible = false;
    }

    void OnCollisionEnter(Collision other)
    {
        // 적에게 닿으면 체력 감소
        if (other.gameObject.CompareTag(EnemyTag)) GameSession.ApplyDamage(1);

        // 바닥에 닿으면 점프 상태 해제
        if (other.gameObject.CompareTag(GroundTag)) IsJump = false;
    }
    #endregion

    #region 외부 접근
    public void SetMovePanel(bool value)
    {
        IsMovePanel = value;
    }

    public void ResetJump()
    {
        IsJump = false;
    }
    #endregion

    #region 이동
    private void ReadInput()
    {
        haxis = Input.GetAxisRaw("Horizontal");
        vaxis = Input.GetAxisRaw("Vertical");
        isJumpDown = Input.GetButtonDown("Jump");
        isInteractDown = Input.GetButtonDown("Interaction"); // 상호작용 E 버튼
    }

    private void Move()
    {
        moveInput = new Vector2(haxis, vaxis).normalized;

        if (moveInput.magnitude != 0)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
            characterBody.forward = moveDir;

            // 이동 발판 위에서는 물리 이동 대신 위치를 직접 옮긴다
            if (IsMovePanel)
            {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }
            else
            {
                Rig.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
            }

            isWalk = true;
        }

        if (moveInput == Vector3.zero) isWalk = false;

        anim.SetBool(WalkHash, isWalk);
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && IsJump == false && moveInput != Vector3.zero)
        {
            anim.SetBool(RunHash, true);
            moveSpeed = runSpeed;
            Invoke(nameof(PlayRunEffect), 0.2f);
            isRun = true;
            audioSource.PlayOneShot(runSound);
        }
        else
        {
            anim.SetBool(RunHash, false);
            moveSpeed = walkSpeed;
            isRun = false;
        }
    }

    private void PlayRunEffect()
    {
        runEffect.Play();
    }
    #endregion

    #region 점프
    private void Jump()
    {
        if (isJumpDown == false || IsJump) return;

        if (isRun == false && isWalk == false)
        {
            Rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
        else if (isRun)
        {
            Rig.AddForce((Vector3.up * jumpPower) + (transform.forward * runningJumpPower), ForceMode.Impulse);
        }
        else
        {
            Rig.AddForce((Vector3.up * jumpPower) + transform.forward * walkJumpForwardPower, ForceMode.Impulse);
        }

        jumpEffect.Play();
        anim.SetTrigger(JumpHash);
        IsJump = true;
        audioSource.PlayOneShot(jumpSound);
    }

    private void EnterJumpItem()
    {
        if (jumpCount <= 0) IsJump = true;

        if (jumpCount > 0)
        {
            Rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            jumpCount--;
        }
    }
    #endregion

    #region 공격
    private void Attack()
    {
        if (curTime > 0) return;

        anim.SetTrigger(AttackHash);
        GameObject instantBullet = Instantiate(bulletPrefab, bulletPos.position, Quaternion.Euler(90f, 0f, 360f));
        Rigidbody bulletRig = instantBullet.GetComponent<Rigidbody>();
        bulletRig.velocity = bulletPos.forward * bulletSpeed;
        audioSource.PlayOneShot(attackSound);
        curTime = attackCoolTime;
    }
    #endregion

    #region 상호작용
    private void Interaction()
    {
        // 1. 모형에 다가가 E를 누르고, 점프 중이 아닐 때
        if (isInteractDown && NearObj != null && IsJump == false)
        {
            if (NearObj.CompareTag(StatueTag)) PickUpStatue();
            if (isAreaIn) PlaceStatue();
        }

        // 2. NPC 앞에서 E를 누르면 퀘스트 창을 연다
        if (isInteractDown && nearNpc[0])
        {
            if (ItemIndex != 5) OpenQuest(0);
        }

        if (isInteractDown && nearNpc[1]) OpenQuest(1);
    }

    // 모형을 없애고 손에 든 모형을 켠다
    private void PickUpStatue()
    {
        Statue statue = NearObj.GetComponent<Statue>();

        hasStatue = true;
        NearObj.SetActive(false);
        animals[statue.Value].SetActive(true);
    }

    // 손에 든 모형을 끄고 제단에 모형 종류를 알린다
    private void PlaceStatue()
    {
        Statue statue = NearObj.GetComponent<Statue>();

        hasStatue = false;
        animals[statue.Value].SetActive(false);
        NearObj.SetActive(true);

        GameObject.Find(AltarName).GetComponent<AnswerArea>().PlaceStatue(statue);
        NearObj = null;
        isAreaIn = false;
    }

    private void OpenQuest(int index)
    {
        uiManager = GameObject.Find(UIManagerName).GetComponent<Stage2UI>();
        uiManager.ShowQuest(index);

        Time.timeScale = 0;
    }

    private void StayStatue(Collider other)
    {
        // 모형에 접촉 중이고 손이 비어 있으면 NearObj에 담는다
        if ((other.CompareTag(StatueTag) && hasStatue == false) || other.CompareTag(AnswerButtonTag))
        {
            NearObj = other.gameObject;
        }
    }

    private void StayKeyPad(Collider other)
    {
        if (other.CompareTag(KeyPadTag) == false || isInteractDown == false) return;

        Cursor.visible = true;
        keyPad.SetActive(true);
        Time.timeScale = 0f;
    }

    private void StayNpc(Collider other)
    {
        if (other.gameObject.name == FirstNpcName)
        {
            nearNpc[0] = true;
            Cursor.visible = true;
            Interaction();
        }

        if (other.gameObject.name == SecondNpcName)
        {
            nearNpc[1] = true;
            Cursor.visible = true;
            Interaction();
        }
    }

    private void StayQuiz(Collider other)
    {
        if (isInteractDown == false) return;

        if (other.gameObject.name == OXQuizName)
        {
            oxQuiz.SetActive(true);
            GameManager.Instance.EnterPuzzle(true);
        }

        if (other.gameObject.name == FindPictureName)
        {
            FindPicture findPicture = findCanvas.GetComponent<FindPicture>();

            findCanvas.SetActive(true);
            findPicture.ResetPicture();
            GameManager.Instance.EnterPuzzle(true);
        }
    }

    private void StayButton(Collider other)
    {
        if (isInteractDown == false) return;

        if (other.TryGetComponent(out Button button)) button.Activate();
    }

    private void EnterDocument(GameObject document)
    {
        uiManager = GameObject.Find(UIManagerName).GetComponent<Stage2UI>();

        document.SetActive(false);
        ItemIndex++;
        uiManager.ChangePoint(ItemIndex);
    }
    #endregion

    #region 피격
    private void EnterBullet(Collider other)
    {
        Bullet bossBullet = other.GetComponent<Bullet>();

        if (isDamaged) return;

        if (bossBullet.BType == BulletType.File || bossBullet.BType == BulletType.Lamp)
        {
            anim.SetTrigger(BeShotHash);
            GameSession.ApplyDamage(bossBullet.Damage);
            StartCoroutine(DamageInvincible());
        }
    }

    private void EnterSpike()
    {
        if (isDamaged) return;

        anim.SetTrigger(BeShotHash);
        GameSession.ApplyDamage(1);
        audioSource.PlayOneShot(damagedSound);
        StartCoroutine(DamageInvincible());
    }

    private IEnumerator DamageInvincible()
    {
        isDamaged = true;
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(damageInvincibleTime);

        isDamaged = false;
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }
    }

    // 맵 밖으로 떨어지면 실패 씬으로 이동한다
    private void CheckFall()
    {
        if (transform.position.y >= fallLimitY) return;

        Cursor.visible = true;
        GameManager.Instance.GameOver();
    }
    #endregion
}

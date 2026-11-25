using UnityEngine;

// 동물 제단. 플레이어가 석상을 올리면 해당 동물을 보여 주고 정답이면 문을 연다.
public class AnswerArea : MonoBehaviour
{
    private static readonly int OpenHash = Animator.StringToHash("Open");

    [SerializeField] private int answerValue;
    [SerializeField] private GameObject[] altarAnimals;
    [SerializeField] private Animator door;
    [SerializeField] private AudioClip openSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>석상을 제단에 올린다. 올린 동물을 보여 주고 정답이면 문을 연다. (옛 이름 Visivle)</summary>
    public void PlaceStatue(Statue statue)
    {
        // 1. 올린 동물만 켜고 나머지는 끈다
        int animalIndex = GetAnimalIndex(statue.Kind);
        if (animalIndex >= 0 && animalIndex < altarAnimals.Length)
        {
            for (int i = 0; i < altarAnimals.Length; i++)
            {
                altarAnimals[i].SetActive(i == animalIndex);
            }
        }

        // 2. 정답 판정
        if (statue.Value == answerValue)
        {
            DoorAnimation.MarkOpen();
            door.SetBool(OpenHash, true);
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            GameSession.ApplyDamage(1);
        }
    }

    // 제단 위 동물 모형의 순서 (Statue.StatueKind 순서와 다르다)
    private int GetAnimalIndex(Statue.StatueKind kind) => kind switch
    {
        Statue.StatueKind.Cow => 0,
        Statue.StatueKind.Pig => 1,
        Statue.StatueKind.Sheep => 2,
        Statue.StatueKind.Chicken => 3,
        _ => -1
    };
}

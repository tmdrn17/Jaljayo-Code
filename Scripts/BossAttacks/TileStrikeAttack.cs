using System.Collections;
using UnityEngine;

// 내려찍기: 타일 하나를 잠깐 껐다가 다시 켠다.
[System.Serializable]
public class TileStrikeAttack : BossAttack
{
    private static readonly int StrikeHash = Animator.StringToHash("Strike");

    [SerializeField] private GameObject[] tiles;
    [SerializeField] private float tileOffTime = 2f;

    protected override int TriggerHash => StrikeHash;

    protected override IEnumerator Attack()
    {
        int tileIndex = Random.Range(0, tiles.Length);
        tiles[tileIndex].SetActive(false);
        yield return new WaitForSeconds(tileOffTime);
        tiles[tileIndex].SetActive(true);
    }
}

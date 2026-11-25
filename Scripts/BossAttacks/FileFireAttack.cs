using System.Collections;
using UnityEngine;

// 고함: 서류를 두 번 연속으로 발사한다.
[System.Serializable]
public class FileFireAttack : BossAttack
{
    private static readonly int YellHash = Animator.StringToHash("Yell");

    [SerializeField] private Transform[] filePoints;
    [SerializeField] private GameObject[] filePrefabs;
    [SerializeField] private int fileSpeed;
    [SerializeField] private float fireInterval = 1f;

    protected override int TriggerHash => YellHash;

    protected override IEnumerator Attack()
    {
        FireFiles();
        yield return new WaitForSeconds(fireInterval);
        FireFiles();
    }

    private void FireFiles()
    {
        for (int i = 0; i < filePoints.Length; i++)
        {
            GameObject file = Object.Instantiate(filePrefabs[i], filePoints[i].position, filePoints[i].rotation);
            file.GetComponent<Rigidbody>().velocity = filePoints[i].forward * fileSpeed;
        }
    }
}

using System.Collections;
using UnityEngine;

// 일정 간격으로 가시를 발사하는 장치
public class SpikeLauncher : MonoBehaviour
{
    [SerializeField] private GameObject spikePrefab;
    [SerializeField] private Transform spikePos;
    [SerializeField] private int spikeSpeed;
    [SerializeField] private float delaySpeed;
    [SerializeField] private float startDelay = 0.1f;
    [SerializeField] private float launchDelay = 0.5f;

    private void Start()
    {
        StartCoroutine(LaunchLoop());
    }

    private IEnumerator LaunchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(startDelay);
            yield return new WaitForSeconds(launchDelay);

            GameObject spike = Instantiate(spikePrefab, spikePos.position, Quaternion.Euler(-90f, 0f, 0f));
            spike.GetComponent<Rigidbody>().velocity = spikePos.forward * spikeSpeed;

            yield return new WaitForSeconds(delaySpeed);
        }
    }
}

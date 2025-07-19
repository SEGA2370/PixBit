using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] private float startSpawnRate = 4.5f;
    [SerializeField] private float minSpawnRate = 1f;
    [SerializeField] private float spawnRateDecreaseAmount = 0.2f;
    [SerializeField] private float decreaseInterval = 10f;
    [SerializeField] private float minHeight = -1f;
    [SerializeField] private float maxHeight = 1f;

    private float currentSpawnRate;
    private Coroutine spawnLoopCoroutine;
    private Coroutine decreaseCoroutine;

    private void OnEnable()
    {
        StartSpawning();
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    public void StartSpawning()
    {
        StopSpawning(); // ОСТАНАВЛИВАЕМ старые корутины перед новым стартом

        ResetSpawnRate();
        spawnLoopCoroutine = StartCoroutine(SpawnLoop());
        decreaseCoroutine = StartCoroutine(DecreaseSpawnRateOverTime());
    }

    public void StopSpawning()
    {
        if (spawnLoopCoroutine != null)
        {
            StopCoroutine(spawnLoopCoroutine);
            spawnLoopCoroutine = null;
        }

        if (decreaseCoroutine != null)
        {
            StopCoroutine(decreaseCoroutine);
            decreaseCoroutine = null;
        }
    }

    public void ResetSpawnRate()
    {
        currentSpawnRate = startSpawnRate;
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    private IEnumerator DecreaseSpawnRateOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(decreaseInterval);
            currentSpawnRate = Mathf.Max(currentSpawnRate - spawnRateDecreaseAmount, minSpawnRate);
        }
    }

    public void Spawn()
    {
        GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }
}

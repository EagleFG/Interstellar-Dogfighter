using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _leftLimit, _rightLimit, _spawnHeight;

    [SerializeField]
    private GameObject _enemy;

    [SerializeField]
    private Transform _enemyParent;

    [SerializeField]
    private float _enemySpawnInterval = 5f;

    [SerializeField]
    private GameObject[] _upgradePickups;

    [SerializeField]
    private float _upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax;

    private bool _canSpawn = true;

    private void Start()
    {
        StartCoroutine(ContinuouslySpawnEnemies());
        StartCoroutine(ContinuouslySpawnUpgrades());
    }

    void SpawnEnemy()
    {
        Instantiate(_enemy, new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity, _enemyParent);
    }

    void SpawnUpgrade()
    {
        Instantiate(_upgradePickups[Random.Range(0, _upgradePickups.Length)], new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity);
    }

    IEnumerator ContinuouslySpawnEnemies()
    {
        while (_canSpawn)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(_enemySpawnInterval);
        }
    }

    IEnumerator ContinuouslySpawnUpgrades()
    {
        yield return new WaitForSeconds(Random.Range(_upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax));

        while (_canSpawn)
        {
            SpawnUpgrade();

            yield return new WaitForSeconds(Random.Range(_upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax));
        }
    }

    public void DisableSpawning()
    {
        _canSpawn = false;
    }
}

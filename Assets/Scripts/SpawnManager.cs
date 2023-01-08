using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private UIManager _uIManager;
    
    [SerializeField]
    private float _leftLimit, _rightLimit, _spawnHeight;

    [SerializeField]
    private GameObject _enemy;

    [SerializeField]
    private Transform _enemyParent;

    [SerializeField]
    private float _enemySpawnInterval = 5f;

    private int _enemiesLeftToSpawnThisWave = 0;

    [SerializeField]
    private GameObject[] _upgradePickups;

    [SerializeField]
    private GameObject[] _rareUpgradePickups;

    [SerializeField]
    private float _upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax;

    [SerializeField]
    private float _rareSpawnIntervalMin, _rareSpawnIntervalMax;

    private bool _canSpawn = false;

    private void Start()
    {
        StartCoroutine(ContinuouslySpawnUpgrades());
        StartCoroutine(ContinuouslySpawnRareUpgrades());
    }

    private void Update()
    {
        if (_enemyParent.childCount == 0 && _enemiesLeftToSpawnThisWave == 0)
        {
            _gameManager.IncreaseCurrentWaveNumber();
            _uIManager.UpdateWaveCounterUI(_gameManager.GetCurrentWaveNumber());
            StartCoroutine(SpawnEnemiesThisWave(_gameManager.GetCurrentWaveNumber()));
        }
    }

    void SpawnEnemy()
    {
        Instantiate(_enemy, new Vector3(0f, _spawnHeight, 0f), Quaternion.identity, _enemyParent);
    }

    void SpawnUpgrade()
    {
        Instantiate(_upgradePickups[Random.Range(0, _upgradePickups.Length)], new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity);
    }

    void SpawnRareUpgrade()
    {
        Instantiate(_rareUpgradePickups[Random.Range(0, _rareUpgradePickups.Length)], new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity);
    }

    IEnumerator SpawnEnemiesThisWave(int currentWave)
    {
        _enemiesLeftToSpawnThisWave = currentWave * 2;

        while (_canSpawn && _enemiesLeftToSpawnThisWave > 0)
        {
            SpawnEnemy();
            _enemiesLeftToSpawnThisWave--;

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

    IEnumerator ContinuouslySpawnRareUpgrades()
    {
        yield return new WaitForSeconds(Random.Range(_rareSpawnIntervalMin, _rareSpawnIntervalMax));

        while (_canSpawn)
        {
            SpawnRareUpgrade();

            yield return new WaitForSeconds(Random.Range(_rareSpawnIntervalMin, _rareSpawnIntervalMax));
        }
    }

    public void EnableSpawning()
    {
        _canSpawn = true;
    }

    public void DisableSpawning()
    {
        _canSpawn = false;
    }
}

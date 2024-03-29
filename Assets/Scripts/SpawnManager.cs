using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private UIManager _uIManager;
    
    [SerializeField]
    private float _leftLimit, _rightLimit, _spawnHeight;

    [SerializeField]
    private GameObject[] _commonEnemies, _rareEnemies;

    [SerializeField] [Range(0, 1)]
    private float _commonEnemySpawnPercentage;

    [SerializeField]
    private GameObject _bossEnemy;

    [SerializeField]
    private Transform _enemyParent;

    [SerializeField]
    private float _enemySpawnInterval = 5f;

    private int _enemiesLeftToSpawnThisWave = 0;

    [SerializeField]
    private Transform _upgradeParent;

    [SerializeField]
    private GameObject[] _commonUpgradePickups, _uncommonUpgradePickups, _rareUpgradePickups;

    [SerializeField] [Range(0, 1)]
    private float _commonUpgradeSpawnPercentage, _uncommonUpgradeSpawnPercentage;

    [SerializeField]
    private float _upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax;

    private bool _canSpawn = false;

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
        float spawnRoll = Random.Range(0f, 1f);

        GameObject[] spawnPool;

        if (spawnRoll <= _commonEnemySpawnPercentage)
        {
            spawnPool = _commonEnemies;
        }
        else
        {
            spawnPool = _rareEnemies;
        }

        Instantiate(spawnPool[Random.Range(0, spawnPool.Length)], new Vector3(0f, _spawnHeight, 0f), Quaternion.identity, _enemyParent);
    }

    void SpawnBoss()
    {
        Instantiate(_bossEnemy, Vector3.zero, Quaternion.identity, _enemyParent);
    }

    void SpawnUpgrade()
    {
        if (_player.GetAmmoCount() == 0)
        {
            Instantiate(_commonUpgradePickups[0], new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity, _upgradeParent);

            return;
        }

        float spawnRoll = Random.Range(0f, 1f);

        GameObject[] spawnPool;

        if (spawnRoll <= _commonUpgradeSpawnPercentage)
        {
            spawnPool = _commonUpgradePickups;
        }
        else if (spawnRoll <= _commonUpgradeSpawnPercentage + _uncommonUpgradeSpawnPercentage)
        {
            spawnPool = _uncommonUpgradePickups;
        }
        else
        {
            spawnPool = _rareUpgradePickups;
        }

        Instantiate(spawnPool[Random.Range(0, spawnPool.Length)], new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0f), Quaternion.identity, _upgradeParent);
    }

    IEnumerator SpawnEnemiesThisWave(int currentWave)
    {
        if (currentWave < 10)
        {
            _enemiesLeftToSpawnThisWave = currentWave * 2;

            while (_canSpawn && _enemiesLeftToSpawnThisWave > 0)
            {
                SpawnEnemy();
                _enemiesLeftToSpawnThisWave--;

                yield return new WaitForSeconds(_enemySpawnInterval);
            }
        }
        else if (currentWave == 10)
        {
            SpawnBoss();
        }
    }

    IEnumerator ContinuouslySpawnUpgrades()
    {
        yield return new WaitForSeconds(Random.Range(_upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax));

        while (_gameManager.GetGameOverState() == false)
        {
            if (_canSpawn)
            {
                SpawnUpgrade();

                yield return new WaitForSeconds(Random.Range(_upgradeSpawnIntervalMin, _upgradeSpawnIntervalMax));
            }
            else
            {
                yield return null;
            }
        }
    }

    public void EnableSpawning()
    {
        _canSpawn = true;
        StartCoroutine(ContinuouslySpawnUpgrades());
    }

    public void DisableSpawning()
    {
        StopAllCoroutines();
    }
}

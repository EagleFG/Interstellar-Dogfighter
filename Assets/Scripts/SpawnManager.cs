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
    private float _spawnInterval = 5;

    private bool _canSpawn = true;

    private void Start()
    {
        StartCoroutine(ContinuouslySpawnEnemies());
    }

    void SpawnEnemy()
    {
        Instantiate(_enemy, new Vector3(Random.Range(_leftLimit, _rightLimit), _spawnHeight, 0), Quaternion.identity, _enemyParent);
    }

    IEnumerator ContinuouslySpawnEnemies()
    {
        while (_canSpawn)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    public void DisableSpawning()
    {
        _canSpawn = false;
    }
}

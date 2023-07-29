using System.Collections;
using UnityEngine;

public class BossPushShieldAbilityBehavior : MonoBehaviour
{
    [SerializeField]
    private BossEnemyBehavior _bossScript;

    [SerializeField]
    private GameObject[] _pushShieldFormations;

    [SerializeField]
    private int _shieldFormationsPerAbility = 3;

    [SerializeField]
    private float _shieldSpawnInterval = 2f;

    private void OnEnable()
    {
        StartCoroutine(SpawnMultipleShieldFormations(_shieldFormationsPerAbility));
    }

    private void Update()
    {
        if (gameObject.transform.childCount == 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        _bossScript.DefensiveAbilityFinished();
    }

    IEnumerator SpawnMultipleShieldFormations(int numberToSpawn)
    {
        int numberSpawned = 0;

        while (numberSpawned < numberToSpawn)
        {
            SpawnShieldFormation();
            numberSpawned++;

            yield return new WaitForSeconds(_shieldSpawnInterval);
        }
    }

    private void SpawnShieldFormation()
    {
        Instantiate(_pushShieldFormations[Random.Range(0, _pushShieldFormations.Length)], gameObject.transform.position, Quaternion.identity, gameObject.transform);
    }
}

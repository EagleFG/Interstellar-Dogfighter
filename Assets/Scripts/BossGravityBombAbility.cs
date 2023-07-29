using UnityEngine;

public class BossGravityBombAbility : MonoBehaviour
{
    [SerializeField]
    private BossEnemyBehavior _bossScript;

    [SerializeField]
    private GameObject _target, _gravityBomb;

    private Transform _projectileParent, _playerTransform;

    private void Awake()
    {
        _projectileParent = GameObject.Find("Projectile List").transform;
        _playerTransform = GameObject.Find("Player").transform;
    }

    private void OnEnable()
    {
        Instantiate(_target, _playerTransform.position, Quaternion.identity, _projectileParent);
    }

    public void SpawnGravityBombAtLocation(Vector3 location)
    {
        Instantiate(_gravityBomb, location, Quaternion.identity, _projectileParent);
    }

    private void OnDisable()
    {
        _bossScript.OffensiveAbilityFinished();
    }
}

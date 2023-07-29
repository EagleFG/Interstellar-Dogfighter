using UnityEngine;

public class BossTurretBehavior : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 30f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _beamPrefab;

    private Transform _player;

    private Transform _projectileParent;

    private bool _canTargetPlayer = false;

    private void Start()
    {
        _player = GameObject.Find("Player").transform;

        _projectileParent = GameObject.Find("Projectile List").transform;
    }

    private void Update()
    {
        if (_canTargetPlayer && _player != null)
        {
            Vector2 targetAngle = transform.position - _player.position;
            float angleDifference = Vector2.SignedAngle(Vector2.up, targetAngle);

            Vector3 targetRotation = new Vector3(0, 0, angleDifference);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), _rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, transform.position - transform.up * 20);
    }

    public void EnablePlayerTargeting()
    {
        _canTargetPlayer = true;
    }

    public void DisablePlayerTargeting()
    {
        _canTargetPlayer = false;
    }

    public void FireLaser()
    {
        Instantiate(_laserPrefab, transform.position, transform.rotation, _projectileParent);
    }

    public void FireBeam()
    {
        Instantiate(_beamPrefab, transform.position, transform.rotation, _projectileParent);
    }
}

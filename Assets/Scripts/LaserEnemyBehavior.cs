using System.Collections;
using UnityEngine;

public class LaserEnemyBehavior : EnemyBehavior
{
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _forwardLaserYOffset, _forwardLaserXOffset, _backwardLaserYOffset;

    [SerializeField]
    private float _weaponEngageHeight = 5f;

    [SerializeField]
    private Vector2 _forwardTargetSearchBoxDimensions, _backwardTargetSearchBoxDimensions;

    [SerializeField]
    private float _targetSearchDistance = 20f;

    [SerializeField]
    private float _laserFiringDelay, _laserFiringCooldown;

    [SerializeField]
    private AudioSource _laserAudio;

    private Transform _projectileParent;
    
    [SerializeField]
    private LayerMask _targetLayerMask;

    private bool _waitingToFireLaser = false;

    private float _previousLaserFireTime = 0f;

    protected override void Start()
    {
        base.Start();

        _projectileParent = GameObject.Find("Projectile List").transform;
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time > _previousLaserFireTime + _laserFiringCooldown && transform.position.y < _weaponEngageHeight && _isAlive == true && _waitingToFireLaser == false)
        {
            SearchForPlayer();
        }
    }

    void SearchForPlayer()
    {
        RaycastHit2D forwardScanHit = Physics2D.BoxCast(gameObject.transform.position, _forwardTargetSearchBoxDimensions, gameObject.transform.rotation.z, -gameObject.transform.up, _targetSearchDistance, _targetLayerMask);

        if (forwardScanHit.collider != null)
        {
            StartCoroutine(WaitToFireLaser(true));
            return;
        }
        
        RaycastHit2D backwardScanHit = Physics2D.BoxCast(gameObject.transform.position, _backwardTargetSearchBoxDimensions, gameObject.transform.rotation.z, gameObject.transform.up, _targetSearchDistance, _targetLayerMask);

        if (backwardScanHit.collider != null)
        {
            StartCoroutine(WaitToFireLaser(false));
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = gameObject.transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(-Vector3.up * _targetSearchDistance / 2, new Vector2(_forwardTargetSearchBoxDimensions.x, _targetSearchDistance));

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Vector3.up * _targetSearchDistance / 2, new Vector2(_backwardTargetSearchBoxDimensions.x, _targetSearchDistance));
    }

    IEnumerator WaitToFireLaser(bool firingForward)
    {
        _waitingToFireLaser = true;

        yield return new WaitForSeconds(_laserFiringDelay);

        if (_isAlive)
        {
            FireLaser(firingForward);
        }
    }

    void FireLaser(bool isFiringForward)
    {
        if (isFiringForward == true)
        {
            Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(_forwardLaserXOffset, _forwardLaserYOffset, 0), Quaternion.identity, _projectileParent);
            Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(-_forwardLaserXOffset, _forwardLaserYOffset, 0), Quaternion.identity, _projectileParent);
        }
        else
        {
            Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(0, _backwardLaserYOffset, 0), Quaternion.Euler(0, 0, 180), _projectileParent);
        }

        _laserAudio.Play();
        _previousLaserFireTime = Time.time;
        _waitingToFireLaser = false;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

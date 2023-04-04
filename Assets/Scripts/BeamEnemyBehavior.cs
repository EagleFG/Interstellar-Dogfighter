using System.Collections;
using UnityEngine;

public class BeamEnemyBehavior : EnemyBehavior
{
    [SerializeField]
    private GameObject _beam;

    [SerializeField]
    private float _beamMovementPauseDuration = 2f;

    [SerializeField]
    private Vector2 _targetSearchBoxDimensions = new Vector2(.5f, .5f);

    [SerializeField]
    private float _targetSearchDistance = 20f;

    [SerializeField]
    private float _weaponEngageHeight = 4f;

    [SerializeField]
    private LayerMask _targetLayerMask;

    private bool _isBeamFiring = false;

    private GameObject _firedBeam;

    private Transform _projectileParent;

    private float _timePaused = 0f;

    protected override void Start()
    {
        base.Start();

        _projectileParent = GameObject.Find("Projectile List").transform;
    }

    protected override void Update()
    {
        base.Update();

        if (transform.position.y < _weaponEngageHeight && _isBeamFiring == false && _isAlive == true)
        {
            SearchForTarget();
        }
    }

    void SearchForTarget()
    {
        RaycastHit2D hit = Physics2D.BoxCast(gameObject.transform.position, _targetSearchBoxDimensions, gameObject.transform.rotation.z, -gameObject.transform.up, _targetSearchDistance, _targetLayerMask);
        
        if (hit.collider != null)
        {
            FireBeam();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = gameObject.transform.localToWorldMatrix;

        Gizmos.DrawWireCube(-Vector3.up * _targetSearchDistance / 2, new Vector2(_targetSearchBoxDimensions.x, _targetSearchDistance));
    }

    void FireBeam()
    {
        _firedBeam = Instantiate(_beam, gameObject.transform.position, Quaternion.identity, _projectileParent);

        StartCoroutine(PauseMovement());
    }

    protected override void CalculateMovement()
    {
        if (_isAlive && _isBeamFiring == false)
        {
            transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

            transform.position = new Vector3(Mathf.Sin((Time.time - _timePaused) * _swaySpeed) * _swayDistance + _startingXCoord, transform.position.y, 0f);

            if (gameObject.transform.position.y < _lowerLimit && _canWrapScreen == true)
            {
                RespawnEnemy();
            }
        }
    }

    IEnumerator PauseMovement()
    {
        _isBeamFiring = true;

        float _currentPause = 0f;

        while (_currentPause < _beamMovementPauseDuration)
        {
            _currentPause += Time.deltaTime;
            _timePaused += Time.deltaTime;

            yield return null;
        }

        _isBeamFiring = false;
    }

    protected override void DestroyThisEnemy()
    {
        base.DestroyThisEnemy();

        Destroy(_firedBeam);
    }
}

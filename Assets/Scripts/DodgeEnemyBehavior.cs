using System.Collections;
using UnityEngine;

public class DodgeEnemyBehavior : EnemyBehavior
{
    [SerializeField]
    private float _chargingSpeed = 6;

    [SerializeField]
    private float _engageHeight;

    [SerializeField]
    private Vector2 _searchBoxDimensions;

    [SerializeField]
    private float _searchDistance = 20;

    [SerializeField]
    private LayerMask _searchLayerMask;

    [SerializeField]
    private float _dodgeInitialSpeed = 5, _dodgeDistance = 2, _dodgeSlowdownDuration = 2, _dodgeAdditionalCooldown = .25f;

    private bool _isDodging = false;

    private float _startingSpeed;

    protected override void Start()
    {
        base.Start();

        _startingSpeed = _speed;
    }

    protected override void Update()
    {
        base.Update();
        
        if (_isAlive == true && _isDodging == false && gameObject.transform.position.y <= _engageHeight)
        {
            SearchForLaserOrPlayer();
        }
    }

    protected override void CalculateMovement()
    {
        if (_isAlive && _isDodging == false)
        {
            transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

            if (gameObject.transform.position.y < _lowerLimit && _canWrapScreen == true)
            {
                RespawnEnemy();
            }
        }
    }

    void SearchForLaserOrPlayer()
    {
        RaycastHit2D[] scanHits = Physics2D.BoxCastAll(gameObject.transform.position, _searchBoxDimensions, gameObject.transform.rotation.z, -gameObject.transform.up, _searchDistance, _searchLayerMask);

        if (scanHits.Length > 0)
        {
            for (int i = 0, l = scanHits.Length; i < l; i++)
            {
                if (scanHits[i].transform.gameObject.layer == LayerMask.NameToLayer("Laser"))
                {
                    // if laser is left of center
                    if (scanHits[i].transform.position.x <= gameObject.transform.position.x)
                    {
                        // if there is room to dodge to the right
                        if (gameObject.transform.position.x + _dodgeDistance <= _xRespawnMax)
                        {
                            StartCoroutine(Dodge(Vector3.right));
                            return;
                        }
                        else
                        {
                            StartCoroutine(Dodge(Vector3.left));
                            return;
                        }
                    }
                    // if laser is right of center
                    else if (scanHits[i].transform.position.x >= gameObject.transform.position.x)
                    {
                        // if there is room to dodge to the left
                        if (gameObject.transform.position.x - _dodgeDistance >= _xRespawnMin)
                        {
                            StartCoroutine(Dodge(Vector3.left));
                            return;
                        }
                        else
                        {
                            StartCoroutine(Dodge(Vector3.right));
                            return;
                        }
                    }
                }
            }

            for (int i = 0, l = scanHits.Length; i < l; i++)
            {
                if (scanHits[i].transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (_speed != _chargingSpeed)
                    {
                        _speed = _chargingSpeed;
                    }
                    return;
                }
            }
        }
        else
        {
            if (_speed != _startingSpeed)
            {
                _speed = _startingSpeed;
            }
        }
    }

    private IEnumerator Dodge(Vector3 dodgeDirection)
    {
        _isDodging = true;

        Vector3 startingPosition = gameObject.transform.position;
        _lerpTimeElapsed = 0;

        while (_isAlive == true && Vector3.Distance(startingPosition, gameObject.transform.position) < _dodgeDistance)
        {
            ReduceSpeed(_dodgeInitialSpeed, _dodgeSlowdownDuration);

            transform.Translate(dodgeDirection.normalized * _speed * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(_dodgeAdditionalCooldown);

        _speed = _startingSpeed;
        _isDodging = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = gameObject.transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(-Vector3.up * _searchDistance / 2, new Vector2(_searchBoxDimensions.x, _searchDistance));
    }
}

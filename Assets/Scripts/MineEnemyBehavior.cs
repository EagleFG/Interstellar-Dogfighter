using System.Collections;
using UnityEngine;

public class MineEnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float _lowerLimit, _xRespawnMin, _xRespawnMax, _yRespawnMin, _yRespawnMax;

    [SerializeField]
    private float _mineEnableWaitDuration = 2;

    [SerializeField]
    private Collider2D[] _colliders;

    [SerializeField]
    private float _speed = 5;

    [SerializeField]
    private float _rammingSpeed = 5;

    [SerializeField]
    private float _detectionRadius = 3;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private GameObject _explosion;

    private int _playerLayerMask;

    private bool _isEnabled = false;
    private bool _isAlive = true;

    private bool _playerDetectedOnPreviousFrame = false;

    private void Start()
    {
        _playerLayerMask = LayerMask.GetMask("Player");

        RespawnEnemy();
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser") || other.gameObject.CompareTag("Missile"))
        {
            if (GameObject.FindGameObjectWithTag("Player").TryGetComponent(out PlayerController player))
            {
                player.UpdateScore(10);
            }

            Destroy(other.gameObject);

            DestroyThisEnemy();
        }
        else if (other.gameObject.CompareTag("Player") && _isEnabled == true)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.DamagePlayer(1);
            }

            DestroyThisEnemy();
        }
    }

    private void RespawnEnemy()
    {
        gameObject.transform.position = new Vector3(Random.Range(_xRespawnMin, _xRespawnMax), Random.Range(_yRespawnMin, _yRespawnMax), 0f);
        _animator.SetTrigger("MineAppear");

        StartCoroutine(WaitToEnableMine());
    }

    IEnumerator WaitToEnableMine()
    {
        _isEnabled = false;

        yield return new WaitForSeconds(_mineEnableWaitDuration);

        _isEnabled = true;
    }

    private void CalculateMovement()
    {
        if (_isAlive && _isEnabled)
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(gameObject.transform.position, _detectionRadius, _playerLayerMask);

            if (playerCollider != null)
            {
                Vector3 directionVector = playerCollider.gameObject.transform.position - gameObject.transform.position;

                transform.Translate(directionVector.normalized * _rammingSpeed * Time.deltaTime);

                if (_playerDetectedOnPreviousFrame == false)
                {
                    _animator.SetBool("MineOn", true);
                    _playerDetectedOnPreviousFrame = true;
                }
            }
            else
            {
                transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

                if (_playerDetectedOnPreviousFrame == true)
                {
                    _animator.SetBool("MineOn", false);
                    _playerDetectedOnPreviousFrame = false;
                }
            }

            if (gameObject.transform.position.y < _lowerLimit)
            {
                RespawnEnemy();
            }
        }
    }

    private void DestroyThisEnemy()
    {
        transform.parent = GameObject.Find("Destroyed Enemy List").transform;

        for (int i = 0, l = _colliders.Length; i < l; i++)
        {
            _colliders[i].enabled = false;
        }

        _isAlive = false;

        _explosion.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(gameObject.transform.position, _detectionRadius);
    }
}

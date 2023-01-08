using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lowerLimit, _yRespawnPosition, _xRespawnMin, _xRespawnMax;

    [SerializeField]
    private float _maxSwayDistance, _maxSwaySpeed;

    private float _swayDistance, _swaySpeed;

    private float _startingXCoord;

    [SerializeField]
    private Collider2D _collider;

    private bool _isAlive = true;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private float _slowdownDuration = 2;

    private bool _canWrapScreen = true;

    private float _lerpTimeElapsed = 0;

    private Vector3 _previousPositionOneFrame;
    private Vector3 _previousPositionTwoFrames;

    private void Start()
    {
        RespawnEnemy();

        UpdatePreviousPositions(true);
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void LateUpdate()
    {
        if (_isAlive)
        {
            UpdatePreviousPositions(false);
        }
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
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.DamagePlayer(1);
            }

            DestroyThisEnemy();
        }
    }

    void UpdatePreviousPositions(bool justRespawned)
    {
        if (justRespawned == true)
        {
            _previousPositionTwoFrames = transform.position;
            _previousPositionOneFrame = transform.position;
        }
        else
        {
            _previousPositionTwoFrames = _previousPositionOneFrame;
            _previousPositionOneFrame = transform.position;
        }
    }

    void CalculateMovement()
    {
        if (_isAlive)
        {
            transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

            transform.position = new Vector3(Mathf.Sin(Time.time * _swaySpeed) * _swayDistance + _startingXCoord, transform.position.y, 0f);

            if (gameObject.transform.position.y < _lowerLimit && _canWrapScreen == true)
            {
                RespawnEnemy();
            }
        }
    }

    void RespawnEnemy()
    {
        SetSwayLimits();

        gameObject.transform.position = new Vector3(_startingXCoord, _yRespawnPosition, 0f);
        _swaySpeed = Random.Range(_maxSwaySpeed / 2f, _maxSwaySpeed);
    }

    void SetSwayLimits()
    {
        _startingXCoord = Random.Range(_xRespawnMin, _xRespawnMax);
        _swayDistance = Random.Range(_maxSwayDistance / 2f, _maxSwayDistance);

        if (_startingXCoord - _swayDistance < _xRespawnMin)
        {
            _startingXCoord += _xRespawnMin - (_startingXCoord - _swayDistance);
        }
        else if (_startingXCoord + _swayDistance > _xRespawnMax)
        {
            _startingXCoord -= (_startingXCoord + _swayDistance) - _xRespawnMax;
        }
    }

    private void DestroyThisEnemy()
    {
        transform.parent = GameObject.Find("Destroyed Enemy List").transform;
        _collider.enabled = false;
        _canWrapScreen = false;
        StartCoroutine(ContinuouslyReduceSpeed());
        _isAlive = false;
        _explosion.SetActive(true);
    }

    private IEnumerator ContinuouslyReduceSpeed()
    {
        Vector3 travelDirection = _previousPositionOneFrame - _previousPositionTwoFrames;
        float travelSpeed = travelDirection.magnitude / Time.deltaTime;

        while(true)
        {
            ReduceSpeed(travelSpeed);

            transform.Translate(travelDirection.normalized * _speed * Time.deltaTime);

            yield return null;
        }
    }

    private void ReduceSpeed(float startingSpeed)
    {
        if (_lerpTimeElapsed != 0)
        {
            _speed = Mathf.Lerp(startingSpeed, 0, _lerpTimeElapsed / _slowdownDuration);
        }

        _lerpTimeElapsed += Time.deltaTime;
    }
}

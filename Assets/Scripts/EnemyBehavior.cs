using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 5f;

    [SerializeField]
    protected float _lowerLimit, _yRespawnPosition, _xRespawnMin, _xRespawnMax;

    [SerializeField]
    private float _maxSwayDistance, _maxSwaySpeed;

    protected float _swayDistance, _swaySpeed;

    protected float _startingXCoord;

    [SerializeField]
    private Collider2D[] _colliders;

    [SerializeField]
    private GameObject _shield;

    [SerializeField]
    private AudioSource _shieldDownAudio;

    [SerializeField]
    private float _chanceToBeShielded = .15f;

    protected bool _isAlive = true;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private float _slowdownDuration = 2;

    protected bool _canWrapScreen = true;

    private float _previousDeltaTime = .016f;
    protected float _lerpTimeElapsed = 0;

    private Vector3 _previousPositionOneFrame;
    private Vector3 _previousPositionTwoFrames;

    protected virtual void Start()
    {
        RespawnEnemy();

        if (Random.Range(0f, 1f) <= _chanceToBeShielded)
        {
            _shield.SetActive(true);
        }
        else
        {
            _shield.SetActive(false);
        }

        UpdatePreviousPositions(true);
    }

    protected virtual void Update()
    {
        CalculateMovement();
    }

    private void LateUpdate()
    {
        UpdatePreviousPositions(false);

        _previousDeltaTime = Time.deltaTime;
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

            if (_shield.activeSelf == true)
            {
                _shield.SetActive(false);
                _shieldDownAudio.Play();
            }
            else
            {
                DestroyThisEnemy();
            }
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.DamagePlayer(1);
            }

            if (_shield.activeSelf == true)
            {
                _shield.SetActive(false);
                _shieldDownAudio.Play();
            }
            else
            {
                DestroyThisEnemy();
            }
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

    protected virtual void CalculateMovement()
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

    protected void RespawnEnemy()
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

    protected virtual void DestroyThisEnemy()
    {
        transform.parent = GameObject.Find("Destroyed Enemy List").transform;

        for (int i = 0, l = _colliders.Length; i < l; i++)
        {
            _colliders[i].enabled = false;
        }

        _canWrapScreen = false;
        _isAlive = false;
        StartCoroutine(ContinuouslyReduceSpeedOnDeath());
        _explosion.SetActive(true);
    }

    private IEnumerator ContinuouslyReduceSpeedOnDeath()
    {
        Vector3 travelDirection = _previousPositionOneFrame - _previousPositionTwoFrames;
        float travelSpeed = travelDirection.magnitude / Time.deltaTime;

        Debug.Log("Destroyed! " + gameObject.GetInstanceID() + "'s initial destruction speed: " + travelSpeed.ToString("F2"));

        while(true)
        {
            ReduceSpeed(travelSpeed, _slowdownDuration);

            transform.Translate(travelDirection.normalized * _speed * _previousDeltaTime);

            Debug.Log(gameObject.GetInstanceID() + "'s speed: " + _speed.ToString("F2"));

            yield return null;
        }
    }

    protected void ReduceSpeed(float startingSpeed, float slowdownDuration)
    {
        if (_lerpTimeElapsed != 0 && slowdownDuration != 0)
        {
            _speed = Mathf.Lerp(startingSpeed, 0, _lerpTimeElapsed / slowdownDuration);
        }

        _lerpTimeElapsed += Time.deltaTime;
    }
}

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int _score = 0;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _leftLimit, _rightLimit, _upperLimit, _lowerLimit;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private Transform _laserParent;

    [SerializeField]
    private float _laserSpawnSpacing;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _firingDelay = .25f;

    private float _previousFire = 0f;

    [SerializeField]
    private int _playerHealth = 3;

    [SerializeField]
    private SpawnManager _spawnManager;

    [SerializeField]
    private float _tripleShotDuration = 5f;

    private bool _tripleShotEnabled = false;

    [SerializeField]
    private float _speedUpDuration = 5f;

    [SerializeField]
    private float _speedUpFactor = 2f;

    private float _speedUpMultiplier = 1f;

    [SerializeField]
    private GameObject _shield;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _leftDamageTrail, _rightDamageTrail;

    [SerializeField]
    private GameObject _disableOnDeathObject;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private AudioSource _laserAudio;

    private bool _canAct = true;

    void Start()
    {
        _score = 0;
        transform.position = new Vector3(0f, -2f, 0f);
    }

    void Update()
    {
        if (_canAct)
        {
            CalculateMovement();

            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _previousFire + _firingDelay)
            {
                FireLaser();
            }
        }
    }

    void CalculateMovement()
    {
        transform.Translate(new Vector3(1f, 0f, 0f) * Input.GetAxis("Horizontal") * _speed * _speedUpMultiplier * Time.deltaTime);
        transform.Translate(new Vector3(0f, 1f, 0f) * Input.GetAxis("Vertical") * _speed * _speedUpMultiplier * Time.deltaTime);

        if (transform.position.x < _leftLimit)
        {
            transform.position = new Vector3(_rightLimit, transform.position.y, 0f);
        }
        else if (transform.position.x > _rightLimit)
        {
            transform.position = new Vector3(_leftLimit, transform.position.y, 0f);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerLimit, _upperLimit), 0f);
    }

    void FireLaser()
    {
        if (_tripleShotEnabled == false)
        {
            Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(0f, _laserSpawnSpacing, 0f), Quaternion.identity, _laserParent);
        }
        else
        {
            Instantiate(_tripleShotPrefab, gameObject.transform.position, Quaternion.identity, _laserParent);
        }

        _laserAudio.Play();

        _previousFire = Time.time;
    }

    public void UpdateScore(int scoreIncrement)
    {
        _score += scoreIncrement;
        _uiManager.UpdateScoreText(_score);
    }

    public void DamagePlayer(int damage)
    {
        if (_shield.activeSelf == true)
        {
            _shield.SetActive(false);
        }
        else
        {
            _playerHealth -= damage;
            _uiManager.UpdateLives(_playerHealth);

            if (_playerHealth == 2)
            {
                _leftDamageTrail.SetActive(true);
            }
            else if (_playerHealth == 1)
            {
                _leftDamageTrail.SetActive(true);
                _rightDamageTrail.SetActive(true);
            }
            if (_playerHealth <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public void TriggerGameOver()
    {
        _gameManager.SetGameOver(true);
        _canAct = false;
        _collider.enabled = false;
        _disableOnDeathObject.SetActive(false);
        _spawnManager.DisableSpawning();
        _uiManager.EnableGameOverText();
        _explosion.SetActive(true);
    }

    public void EnableTripleShot()
    {
        StartCoroutine(WaitToDisableTripleShot());
    }

    IEnumerator WaitToDisableTripleShot()
    {
        _tripleShotEnabled = true;

        yield return new WaitForSeconds(_tripleShotDuration);

        _tripleShotEnabled = false;
    }

    public void EnableSpeedUp()
    {
        StartCoroutine(WaitToDisableSpeedUp());
    }

    IEnumerator WaitToDisableSpeedUp()
    {
        _speedUpMultiplier = _speedUpFactor;

        yield return new WaitForSeconds(_speedUpDuration);

        _speedUpMultiplier = 1f;
    }

    public void EnableShield()
    {
        _shield.SetActive(true);
    }
}

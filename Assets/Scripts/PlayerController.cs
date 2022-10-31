using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    private float _firingDelay = .25f;

    private float _previousFire = 0f;

    [SerializeField]
    private int _playerHealth = 3;

    [SerializeField]
    private SpawnManager _spawnManager;

    void Start()
    {
        transform.position = new Vector3(0, -2, 0);
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _previousFire + _firingDelay)
        {
            FireLaser();
        }

        if (_playerHealth <= 0)
        {
            _spawnManager.DisableSpawning();
            Destroy(gameObject);
        }
    }

    void CalculateMovement()
    {
        transform.Translate(new Vector3(1, 0, 0) * Input.GetAxis("Horizontal") * _speed * Time.deltaTime);
        transform.Translate(new Vector3(0, 1, 0) * Input.GetAxis("Vertical") * _speed * Time.deltaTime);

        if (transform.position.x < _leftLimit)
        {
            transform.position = new Vector3(_rightLimit, transform.position.y, 0);
        }
        else if (transform.position.x > _rightLimit)
        {
            transform.position = new Vector3(_leftLimit, transform.position.y, 0);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerLimit, _upperLimit), 0);
    }

    void FireLaser()
    {
        Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(0, _laserSpawnSpacing, 0), Quaternion.identity, _laserParent);

        _previousFire = Time.time;
    }

    public void DamagePlayer(int damage)
    {
        _playerHealth -= damage;
    }
}

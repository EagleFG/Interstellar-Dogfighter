using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    private float _thrusterSpeed = 7.5f;

    [SerializeField]
    private Transform _thrusterGraphic;

    [SerializeField]
    private float _leftLimit, _rightLimit, _upperLimit, _lowerLimit;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _missilePrefab;

    [SerializeField]
    private Transform _projectileParent;

    [SerializeField]
    private float _laserSpawnSpacing;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _laserFiringDelay = .25f;

    [SerializeField]
    private float _missileFiringDelay = .5f;

    private float _firingDelay;

    private float _previousFire = 0f;

    [SerializeField]
    private int _playerHealth = 3;

    [SerializeField]
    private GameObject _camera;

    [SerializeField]
    private float _cameraShakeAmount = .5f;

    [SerializeField]
    private float _cameraShakeDuration = 1f;

    [SerializeField]
    private PostProcessVolume _postProcessing;

    [SerializeField]
    private int _shieldHealth = 0;

    [SerializeField]
    private Color _shieldColor, _shieldDisruptionColor;

    [SerializeField]
    private float _shieldDisruptionDuration = 3f;

    [SerializeField]
    private AudioSource _shieldDownAudio;

    private bool _isShieldDisrupted = false;

    private float _fuelRemaining = 100f;

    [SerializeField]
    private float _fuelUsageSpeed = 25f;

    [SerializeField]
    private float _fuelRechargeSpeed = 10f;

    [SerializeField]
    private float _fuelRechargeDelay = 1f;

    private IEnumerator _fuelRechargeCoroutine;

    [SerializeField]
    private int _maxAmmoCount = 15;

    public int _ammoCount;

    [SerializeField]
    private Transform _pickupParent;

    [SerializeField]
    private float _magnetizeCollectibleRadius, _magnetizeCollectibleSpeed;

    [SerializeField]
    private GameObject _magnetizeScanRangeIndicatorPrefab;

    [SerializeField]
    private LayerMask _collectiblesLayerMask;

    [SerializeField]
    private SpawnManager _spawnManager;

    [SerializeField]
    private float _tripleShotDuration = 5f;

    private bool _tripleShotEnabled = false;

    [SerializeField]
    private float _missileDuration = 10f;

    private bool _missileEnabled = false;

    [SerializeField]
    private float _speedUpDuration = 5f;

    [SerializeField]
    private float _speedUpFactor = 2f;

    private float _speedUpMultiplier = 1f;

    [SerializeField]
    private SpriteRenderer _shield;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _leftDamageTrail, _rightDamageTrail;

    [SerializeField]
    private GameObject _disableOnDeathObject;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private AudioSource _laserAudio, _missileAudio, _ammoEmptyAudio;

    private bool _canAct = true;

    void Start()
    {
        transform.position = new Vector3(0f, -2f, 0f);

        _score = 0;
        _uiManager.UpdateScoreUI(_score);

        _uiManager.UpdateFuelSlider(_fuelRemaining);

        _ammoCount = _maxAmmoCount;
        _uiManager.UpdateAmmoCountUI(_ammoCount, _maxAmmoCount);

        _firingDelay = _laserFiringDelay;
    }

    void Update()
    {
        if (_canAct)
        {
            CalculateMovement();

            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _previousFire + _firingDelay)
            {
                FireWeapon();
            }

            CalculateMagnetization();
        }
    }

    void CalculateMovement()
    {
        float moveSpeed;

        // stop recharging fuel if starting to use thrusters
        if (Input.GetKeyDown(KeyCode.LeftShift) == true)
        {
            if (_fuelRechargeCoroutine != null)
            {
                StopCoroutine(_fuelRechargeCoroutine);
            }
        }

        // check if using thrusters and enough fuel is remaining
        if (Input.GetKey(KeyCode.LeftShift) == true && _fuelRemaining > 0)
        {
            moveSpeed = _thrusterSpeed;

            _thrusterGraphic.localPosition = new Vector3(0f, -3.1f, 0f);
            _thrusterGraphic.localScale = new Vector3(.9f, .9f, .9f);

            _fuelRemaining = Mathf.Clamp(_fuelRemaining - _fuelUsageSpeed * Time.deltaTime, 0, 100);
            _uiManager.UpdateFuelSlider(_fuelRemaining);
        }
        else
        {
            moveSpeed = _speed;

            _thrusterGraphic.localPosition = new Vector3(0f, -2.9f, 0f);
            _thrusterGraphic.localScale = new Vector3(.8f, .8f, .8f);
        }

        // check when player stops using thrusters
        if (Input.GetKeyUp(KeyCode.LeftShift) == true)
        {
            if (_fuelRemaining > 0)
            {
                //start recharge delay
                _fuelRechargeCoroutine = ContinuouslyRechargeFuel(_fuelRechargeDelay);
                StartCoroutine(_fuelRechargeCoroutine);
            }
            else
            {
                //start doubled recharge delay
                _fuelRechargeCoroutine = ContinuouslyRechargeFuel(_fuelRechargeDelay * 2);
                StartCoroutine(_fuelRechargeCoroutine);
            }
        }

        transform.Translate(new Vector3(1f, 0f, 0f) * Input.GetAxis("Horizontal") * moveSpeed * _speedUpMultiplier * Time.deltaTime);
        transform.Translate(new Vector3(0f, 1f, 0f) * Input.GetAxis("Vertical") * moveSpeed * _speedUpMultiplier * Time.deltaTime);

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

    public int GetAmmoCount()
    {
        return _ammoCount;
    }

    void FireWeapon()
    {
        if (_ammoCount == 0)
        {
            _ammoEmptyAudio.Play();
            return;
        }

        if (_missileEnabled == true)
        {
            Instantiate(_missilePrefab, gameObject.transform.position + new Vector3(0f, _laserSpawnSpacing, 0f), Quaternion.identity, _projectileParent);
            _missileAudio.Play();
        }
        else if (_tripleShotEnabled == true)
        {
            Instantiate(_tripleShotPrefab, gameObject.transform.position, Quaternion.identity, _projectileParent);
            _laserAudio.Play();
        }
        else
        {
            Instantiate(_laserPrefab, gameObject.transform.position + new Vector3(0f, _laserSpawnSpacing, 0f), Quaternion.identity, _projectileParent);
            _laserAudio.Play();
        }

        _ammoCount -= 1;
        _uiManager.UpdateAmmoCountUI(_ammoCount, _maxAmmoCount);

        _previousFire = Time.time;
    }

    void CalculateMagnetization()
    {
        Collider2D[] collectibleColliders = Physics2D.OverlapCircleAll(gameObject.transform.position, _magnetizeCollectibleRadius, _collectiblesLayerMask);

        if (Input.GetKeyDown(KeyCode.C))
        {
            Instantiate(_magnetizeScanRangeIndicatorPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
        }

        if (Input.GetKey(KeyCode.C))
        {
            for (int i = 0, l = collectibleColliders.Length; i < l; i++)
            {
                if (collectibleColliders[i].TryGetComponent(out PickupBehavior pickupLogic))
                {
                    if (pickupLogic.IsMagnetized() == false)
                    {
                        pickupLogic.Magnetize();
                    }
                }

                collectibleColliders[i].transform.Translate((gameObject.transform.position - collectibleColliders[i].transform.position).normalized * _magnetizeCollectibleSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            for (int i = 0, l = collectibleColliders.Length; i < l; i++)
            {
                if (collectibleColliders[i].gameObject.TryGetComponent(out PickupBehavior pickupLogic))
                {
                    pickupLogic.Demagnetize();
                }
            }
        }

        // demagnetize if pickup leaves magnetize radius while being magnetized
        for (int i = 0, l =_pickupParent.childCount; i < l; i++)
        {
            if (DoesPickupExistInMagnetizeRadius(_pickupParent.GetChild(i), collectibleColliders) == false && _pickupParent.GetChild(i).TryGetComponent(out PickupBehavior pickupLogic))
            {
                if (pickupLogic.IsMagnetized() == true)
                {
                    pickupLogic.Demagnetize();
                }
            }
        }
    }

    bool DoesPickupExistInMagnetizeRadius(Transform pickup, Collider2D[] collidersInRadius)
    {
        for (int i = 0, l = collidersInRadius.Length; i < l; i++)
        {
            if (pickup.gameObject.GetInstanceID() == collidersInRadius[i].gameObject.GetInstanceID())
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(gameObject.transform.position, _magnetizeCollectibleRadius);
    }

    public void RefillAmmo()
    {
        _ammoCount = _maxAmmoCount;
        _uiManager.UpdateAmmoCountUI(_ammoCount, _maxAmmoCount);
    }

    public void UpdateScore(int scoreIncrement)
    {
        _score += scoreIncrement;
        _uiManager.UpdateScoreUI(_score);
    }

    public void HealPlayer(int healing)
    {
        _playerHealth = Mathf.Clamp(_playerHealth + healing, 0, 3);
        _uiManager.UpdateLivesUI(_playerHealth);

        if (_playerHealth == 3)
        {
            _leftDamageTrail.SetActive(false);
            _rightDamageTrail.SetActive(false);
            _postProcessing.weight = 0;
        }
        else if (_playerHealth == 2)
        {
            _rightDamageTrail.SetActive(false);
            _postProcessing.weight = .33f;
        }
    }

    public void DamagePlayer(int damage)
    {
        if (_shieldHealth > 0 && _isShieldDisrupted == false)
        {
            UpdateShieldHealth(-damage);
            _shieldDownAudio.Play();
        }
        else
        {
            _playerHealth -= damage;
            _uiManager.UpdateLivesUI(_playerHealth);

            if (_playerHealth == 2)
            {
                _leftDamageTrail.SetActive(true);
                _postProcessing.weight = .33f;
            }
            else if (_playerHealth == 1)
            {
                _leftDamageTrail.SetActive(true);
                _rightDamageTrail.SetActive(true);
                _postProcessing.weight = .66f;
            }
            if (_playerHealth <= 0)
            {
                _postProcessing.weight = 1f;
                TriggerGameOver();
            }
        }

        StartCoroutine(ShakeCamera());
    }

    public void StunPlayer()
    {
        _canAct = false;
    }

    public void StunPlayer(float stunDuration)
    {
        StartCoroutine(StunPlayerForDuration(stunDuration));
    }

    public void UnstunPlayer()
    {
        _canAct = true;
    }

    IEnumerator StunPlayerForDuration(float stunDuration)
    {
        StunPlayer();

        yield return new WaitForSeconds(stunDuration);

        UnstunPlayer();
    }

    IEnumerator ShakeCamera()
    {
        float startTime = Time.time;
        int frameCounter = 0;

        while (Time.time < startTime + _cameraShakeDuration)
        {
            if (frameCounter % (Application.targetFrameRate / 30) == 0)
            {
                Vector2 cameraPosition = Random.insideUnitCircle * _cameraShakeAmount;

                _camera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
            }

            frameCounter += 1;
            yield return null;
        }

        _camera.transform.position = new Vector3(0, 0, -10);
    }

    public void UpdateShieldHealth(int shieldHealthChange)
    {
        _shieldHealth = Mathf.Clamp(_shieldHealth + shieldHealthChange, 0, 3);

        if (_shieldHealth > 0)
        {
            if (_shield.gameObject.activeSelf == false)
            {
                _shield.gameObject.SetActive(true);
            }

            _shield.color = new Color(1, 1, 1, (float)_shieldHealth / 3);
        }
        else
        {
            if (_shield.gameObject.activeSelf == true)
            {
                _shield.gameObject.SetActive(false);
            }
        }

        _uiManager.UpdateShieldsUI(_shieldHealth);
    }

    public void DisruptShield()
    {
        StartCoroutine(TemporarilyDisruptShield());
    }

    IEnumerator TemporarilyDisruptShield()
    {
        _isShieldDisrupted = true;
        _shield.gameObject.SetActive(false);
        _uiManager.UpdateShieldsColorUI(_shieldDisruptionColor);

        yield return new WaitForSeconds(_shieldDisruptionDuration);

        _isShieldDisrupted = false;
        UpdateShieldHealth(0);
        _uiManager.UpdateShieldsColorUI(_shieldColor);
    }

    IEnumerator ContinuouslyRechargeFuel(float fuelRechargeDelay)
    {
        yield return new WaitForSeconds(fuelRechargeDelay);

        while (_fuelRemaining < 100)
        {
            _fuelRemaining = Mathf.Clamp(_fuelRemaining + _fuelRechargeSpeed * Time.deltaTime, 0, 100);
            _uiManager.UpdateFuelSlider(_fuelRemaining);
            yield return null;
        }
    }

    public void TriggerGameOver()
    {
        _gameManager.SetGameOverState(true);
        StunPlayer();
        _collider.enabled = false;
        _disableOnDeathObject.SetActive(false);
        _spawnManager.DisableSpawning();
        _uiManager.EnableGameOverUI();
        _explosion.SetActive(true);
    }

    public void EnableMissile()
    {
        RefillAmmo();
        StartCoroutine(WaitToDisableMissile());
    }

    IEnumerator WaitToDisableMissile()
    {
        _missileEnabled = true;
        _firingDelay = _missileFiringDelay;

        yield return new WaitForSeconds(_missileDuration);

        _firingDelay = _laserFiringDelay;
        _missileEnabled = false;
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
}

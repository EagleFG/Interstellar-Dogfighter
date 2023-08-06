using System.Collections;
using UnityEngine;

public class BossEnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private Vector3 _spawnPosition, _finalPosition, _smoothDampOffset;

    [SerializeField]
    private float _introAnimationDuration = 3;

    [SerializeField]
    private Collider2D[] _colliders;

    [SerializeField]
    private int _maxHealth = 60;

    private int _currentHealth;

    [SerializeField]
    private AudioClip _ramAudio;

    [SerializeField]
    private float _ramKnockbackDuration = .5f, _ramKnockbackDistance = 1;

    [SerializeField]
    private Transform _bossAbilityParent;

    [SerializeField]
    private GameObject _defensiveAbility;

    [SerializeField]
    private GameObject[] _offensiveAbilities;

    [SerializeField]
    private float _defensiveAbilityInterval = 9f, _offensiveAbilityInterval = 4f;

    private bool _canUseDefensiveAbility = false, _canUseOffensiveAbility = false;

    private float _previousDefensiveAbilityTime, _previousOffensiveAbilityTime;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _stunPulse;

    [SerializeField]
    private BossTurretBehavior[] _turrets;

    [SerializeField]
    private float _turretLaserFiringInterval = 2f;

    [SerializeField]
    private AudioSource _laserAudio;

    private PlayerController _player;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private MusicController _musicController;
    private Transform _projectileParent;
    private UIManager _uiManager;
    private int _pulseCount = 0;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _musicController = GameObject.Find("Music Controller").GetComponent<MusicController>();
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _projectileParent = GameObject.Find("Projectile List").transform;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        DisableBossFight();

        if (_player != null)
        {
            gameObject.transform.position = _spawnPosition;

            StartCoroutine(PlayBossIntro());
        }
    }

    IEnumerator PlayBossIntro()
    {
        _player.StunPlayer();
        _musicController.PlayBossMusic();
        DestroyAllProjectiles();

        Vector3 velocity = Vector3.zero;

        while (gameObject.transform.position.x < _finalPosition.x)
        {
            gameObject.transform.position = Vector3.SmoothDamp(transform.position, _finalPosition + _smoothDampOffset, ref velocity, _introAnimationDuration);

            yield return null;
        }

        gameObject.transform.position = _finalPosition;

        _player.UnstunPlayer();

        EnableBossFight();
    }

    private void DestroyAllProjectiles()
    {
        if (_projectileParent.childCount > 0)
        {
            for (int i = 0, l = _projectileParent.childCount; i < l; i++)
            {
                Destroy(_projectileParent.GetChild(i).gameObject);
            }
        }
    }

    private void EnableBossFight()
    {
        for (int i = 0, l = _colliders.Length; i < l; i++)
        {
            _colliders[i].enabled = true;
        }

        _uiManager.EnableBossHealthSlider(_maxHealth);
        _uiManager.UpdateBossHealthSlider(_currentHealth);

        _previousDefensiveAbilityTime = Time.time;
        _previousOffensiveAbilityTime = Time.time;
        _canUseDefensiveAbility = true;
        _canUseOffensiveAbility = true;
        ActivateTurrets();
        StartCoroutine(ConstantlyFireLasers());
    }

    private void DisableBossFight()
    {
        for (int i = 0, l = _colliders.Length; i < l; i++)
        {
            _colliders[i].enabled = false;
        }

        for (int i = 0, l = _bossAbilityParent.childCount; i < l; i++)
        {
            _bossAbilityParent.GetChild(i).gameObject.SetActive(false);
        }

        StopAllCoroutines();
        _canUseDefensiveAbility = false;
        _canUseOffensiveAbility = false;
        DeactivateTurrets();

        _uiManager.DisableBossHealthSlider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            DamageBoss(1);
        }
        else if (other.gameObject.CompareTag("Missile"))
        {
            Destroy(other.gameObject);

            DamageBoss(2);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            _player.DamagePlayer(1);

            AudioSource.PlayClipAtPoint(_ramAudio, Camera.main.transform.position, .5f);

            StartCoroutine(KnockBackPlayer());
        }
    }

    private void Update()
    {
        if (_canUseDefensiveAbility == true && Time.time > _previousDefensiveAbilityTime + _defensiveAbilityInterval && _gameManager.GetGameOverState() == false)
        {
            ActivateDefensiveAbility();
        }

        if (_canUseOffensiveAbility == true && Time.time > _previousOffensiveAbilityTime + _offensiveAbilityInterval && _gameManager.GetGameOverState() == false)
        {
            ActivateOffensiveAbility();
        }
    }

    private void ActivateDefensiveAbility()
    {
        _defensiveAbility.SetActive(true);

        _canUseDefensiveAbility = false;
    }

    public void DefensiveAbilityFinished()
    {
        _canUseDefensiveAbility = true;

        _previousDefensiveAbilityTime = Time.time;
    }

    private void ActivateOffensiveAbility()
    {
        _offensiveAbilities[Random.Range(0, _offensiveAbilities.Length)].SetActive(true);

        _canUseOffensiveAbility = false;
    }

    public void OffensiveAbilityFinished()
    {
        _canUseOffensiveAbility = true;

        _previousOffensiveAbilityTime = Time.time;
    }

    private void ActivateTurrets()
    {
        for (int i = 0, l = _turrets.Length; i < l; i++)
        {
            _turrets[i].EnablePlayerTargeting();
        }
    }

    private void DeactivateTurrets()
    {
        for (int i = 0, l = _turrets.Length; i < l; i++)
        {
            _turrets[i].DisablePlayerTargeting();
        }
    }

    public BossTurretBehavior[] GetBossTurretArray()
    {
        return _turrets;
    }

    IEnumerator ConstantlyFireLasers()
    {
        float timeSinceLastLaserFired = Time.time;

        while(_gameManager.GetGameOverState() == false)
        {
            if (_canUseOffensiveAbility && Time.time > timeSinceLastLaserFired + _turretLaserFiringInterval)
            {
                _turrets[Random.Range(0, _turrets.Length)].FireLaser();

                _laserAudio.Stop();
                _laserAudio.Play();

                timeSinceLastLaserFired = Time.time;
            }

            yield return null;
        }
    }

    private void DamageBoss(int damage)
    {
        _currentHealth -= damage;

        _uiManager.UpdateBossHealthSlider(_currentHealth);

        if (_currentHealth <= _maxHealth * .75f && _pulseCount == 0)
        {
            _stunPulse.SetActive(true);
            _pulseCount++;
        }
        else if (_currentHealth <= _maxHealth * .5f && _pulseCount == 1)
        {
            _stunPulse.SetActive(true);
            _pulseCount++;
        }
        else if (_currentHealth <= _maxHealth * .25f && _pulseCount == 2)
        {
            _stunPulse.SetActive(true);
            _pulseCount++;
        }
        else if (_currentHealth <= 0)
        {
            _explosion.SetActive(true);
            TriggerVictory();
        }
    }

    IEnumerator KnockBackPlayer()
    {
        _player.StunPlayer();

        Vector3 startingPosition = _player.gameObject.transform.position;
        Vector3 targetPosition = _player.gameObject.transform.position - new Vector3(0, _ramKnockbackDistance, 0);

        float timeElapsed = 0;

        while (timeElapsed < _ramKnockbackDuration)
        {
            _player.gameObject.transform.position = Vector3.Lerp(startingPosition, targetPosition, timeElapsed / _ramKnockbackDuration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        _player.UnstunPlayer();
    }

    private void TriggerVictory()
    {
        DisableBossFight();

        _player.UpdateScore(100);

        _musicController.PlayVictoryMusic();
        _spawnManager.DisableSpawning();

        _gameManager.SetVictoryState(true);

        DestroyAllProjectiles();

        _uiManager.EnableVictoryUI();
    }

    

    /*
    TO DO:

    +Boss entry animation
        +Take control away from player until animation ends
        +Restore ammo for player

    +Boss health
        +Health bar UI
        +Take damage from player projectiles
        +Damage and knock back player if rammed into
        +Upon defeat, explode
        +Upon defeat, add points to the score
        +Upon defeat, end game with victory screen

    Boss abilities
        +Intermittent lasers firing
        +Multi-beam attack
        +Gravity bomb that pulls everything in a radius toward itself right before exploding
            +Pulls player
            +Pulls upgrades
        +Shield that moves from boss downward, blocking lasers/missiles and pushing the player
        +Pulse that temporarily disables player's shield/movement when boss's health hits certain thresholds

    Audio
        +Switch to boss music during intro animation
        +Switch to victory music when defeated
        +Shield hit when player rams boss
        +Laser firing
        +Multi-beam attack
        +Gravity bomb
            +Target aim
            +Target flash
            +Bomb persistent
        +Stun pulse
    */
}

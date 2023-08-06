using UnityEngine;

public class BossTargetScript : MonoBehaviour
{
    [SerializeField]
    private AudioClip _lockedOnAudio;

    [SerializeField]
    private AudioSource _targetAudio;

    private BossGravityBombAbility _gravityBombAbilityScript;

    private Transform _playerTransform;

    private bool _isTrackingPlayer = true;

    private void Awake()
    {
        _gravityBombAbilityScript = GameObject.Find("Gravity Bomb Ability").GetComponent<BossGravityBombAbility>();

        _playerTransform = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        if (_isTrackingPlayer)
        {
            transform.position = _playerTransform.position;
        }
    }

    // called in animation event
    private void PlayerLockedOn()
    {
        _targetAudio.Stop();
        _targetAudio.clip = _lockedOnAudio;
        _targetAudio.Play();

        _isTrackingPlayer = false;
    }

    // called in animation event
    private void SpawnGravityBomb()
    {
        if (_gravityBombAbilityScript != null)
        {
            _gravityBombAbilityScript.SpawnGravityBombAtLocation(transform.position);
        }

        Destroy(gameObject);
    }
}

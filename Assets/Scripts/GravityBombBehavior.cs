using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBombBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform _visualRim, _visualVortex, _visualBackground;

    [SerializeField]
    private float _rimRotationSpeed, _vortexRotationSpeed, _backgroundRotationSpeed;

    [SerializeField]
    private float _minPullSpeed, _maxPullSpeed, _maxPullDistance;

    [SerializeField]
    private float _timeBeforeDetonation = 2f;

    [SerializeField]
    GameObject _explosion;

    private List<Transform> _touchedUpgrades;

    private bool _isTouchingPlayer = false;

    private Transform _playerTransform;

    private GameObject _gravityBombAbility;

    private void Awake()
    {
        _gravityBombAbility = GameObject.Find("Gravity Bomb Ability");

        _touchedUpgrades = new List<Transform>();
    }

    private void Start()
    {
        StartCoroutine(WaitToDetonate());
    }

    private void Update()
    {
        _visualRim.Rotate(Vector3.forward, _rimRotationSpeed * Time.deltaTime);
        _visualVortex.Rotate(Vector3.forward, _vortexRotationSpeed * Time.deltaTime);
        _visualBackground.Rotate(Vector3.forward, _backgroundRotationSpeed * Time.deltaTime);

        if (_isTouchingPlayer && _playerTransform != null)
        {
            PullTransformToCenter(_playerTransform);
        }

        if (_touchedUpgrades.Count > 0)
        {
            for (int i = 0, l = _touchedUpgrades.Count; i < l; i++)
            {
                if (_touchedUpgrades[i] != null)
                {
                    PullTransformToCenter(_touchedUpgrades[i]);
                }
            }
        }
    }

    private void PullTransformToCenter(Transform transformToPull)
    {
        float transformDistanceFromCenter = Vector3.Distance(transform.position, transformToPull.position);

        if (transformDistanceFromCenter < _maxPullSpeed * Time.deltaTime)
        {
            transformToPull.position = transform.position;
        }
        else
        {
            float pullFactor = Mathf.InverseLerp(_maxPullDistance, 0, transformDistanceFromCenter);

            float pullSpeed = Mathf.Lerp(_minPullSpeed, _maxPullSpeed, pullFactor);

            Vector3 pullVector = (transform.position - transformToPull.position).normalized * pullSpeed * Time.deltaTime;

            transformToPull.Translate(pullVector);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isTouchingPlayer = true;

            _playerTransform = other.gameObject.transform;
        }

        if (other.gameObject.CompareTag("Pickup"))
        {
            _touchedUpgrades.Add(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isTouchingPlayer = false;
        }
    }

    IEnumerator WaitToDetonate()
    {
        yield return new WaitForSeconds(_timeBeforeDetonation);

        Detonate();
    }

    public void Detonate()
    {
        if (_isTouchingPlayer)
        {
            if (_playerTransform.TryGetComponent(out PlayerController player))
            {
                player.DamagePlayer(3);
            }
        }

        if (_touchedUpgrades.Count > 0)
        {
            for (int i = 0, l = _touchedUpgrades.Count; i < l; i++)
            {
                if (_touchedUpgrades[i] != null)
                {
                    Destroy(_touchedUpgrades[i].gameObject);
                }
            }
        }

        _minPullSpeed = 0;
        _maxPullSpeed = 0;

        if (_gravityBombAbility != null)
        {
            _gravityBombAbility.SetActive(false);
        }

        _explosion.SetActive(true);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    private float _maxSpeed;

    [SerializeField]
    private float _lowerLimit, _yRespawnPosition, _xRespawnMin, _xRespawnMax;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private float _slowdownDuration = 2;

    private bool _canWrapScreen = true;

    private float _lerpTimeElapsed = 0;

    private void Start()
    {
        _maxSpeed = _speed;
    }

    private void Update()
    {
        transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y < _lowerLimit && _canWrapScreen == true)
        {
            gameObject.transform.position = new Vector3(Random.Range(_xRespawnMin, _xRespawnMax), _yRespawnPosition, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser"))
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

    private void DestroyThisEnemy()
    {
        _collider.enabled = false;
        _canWrapScreen = false;
        //StartCoroutine(ContinuouslyReduceSpeed());
        _explosion.SetActive(true);
    }

    private IEnumerator ContinuouslyReduceSpeed()
    {
        while(true)
        {
            ReduceSpeed();
            yield return null;
        }
    }

    private void ReduceSpeed()
    {
        if (_lerpTimeElapsed != 0)
        {
            _speed = Mathf.Lerp(_maxSpeed, 0, _lerpTimeElapsed / _slowdownDuration);
        }

        _lerpTimeElapsed += Time.deltaTime;
    }
}

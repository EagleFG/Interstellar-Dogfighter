using UnityEngine;

public class BossPushShieldScript : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed, _pushSpeed;

    private Vector3 _movementVector, _pushVector;

    private bool _isTouchingPlayer = false;

    private Transform _playerTransform;

    private void Start()
    {
        _movementVector = new Vector3(0, -_movementSpeed, 0);
        _pushVector = new Vector3(0, -_pushSpeed, 0);
    }

    private void Update()
    {
        gameObject.transform.Translate(_movementVector * Time.deltaTime);

        if (gameObject.transform.position.y < -6.5f)
        {
            Destroy(gameObject);
        }

        if (_isTouchingPlayer && _playerTransform != null)
        {
            _playerTransform.Translate(_pushVector * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser") || other.gameObject.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            _isTouchingPlayer = true;

            _playerTransform = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isTouchingPlayer = false;
        }
    }
}

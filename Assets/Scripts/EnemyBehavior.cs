using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lowerLimit, _yRespawnPosition, _xRespawnMin, _xRespawnMax;

    private void Update()
    {
        transform.Translate(new Vector3(0, -1, 0) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y < _lowerLimit)
        {
            gameObject.transform.position = new Vector3(Random.Range(_xRespawnMin, _xRespawnMax), _yRespawnPosition, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.DamagePlayer(1);
            }

            Destroy(gameObject);
        }
    }
}

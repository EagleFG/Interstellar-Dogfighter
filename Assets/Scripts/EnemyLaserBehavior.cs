using UnityEngine;

public class EnemyLaserBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _upperBoundary, _lowerBoundary;

    void Update()
    {
        transform.Translate(new Vector3(0f, 1f, 0f) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y > _upperBoundary || gameObject.transform.position.y < _lowerBoundary)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
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

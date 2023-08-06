using UnityEngine;

public class AsteroidGameStart : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 5f;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private SpawnManager _spawnManager;

    [SerializeField]
    private MusicController _gameMusic;

    private Vector3 _rotationAxis = new Vector3(0, 0, 1);

    void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            DestroyThisObject();
        }
    }

    private void DestroyThisObject()
    {
        _collider.enabled = false;
        transform.parent = GameObject.Find("Destroyed Enemy List").transform;
        _spawnManager.EnableSpawning();
        _gameMusic.PlayBackgroundMusic();
        _explosion.SetActive(true);
    }
}

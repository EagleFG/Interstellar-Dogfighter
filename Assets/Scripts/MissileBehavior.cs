using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _rotationSpeed = 30f;

    [SerializeField]
    private float _duration = 5f;

    private float _spawnTime;

    [SerializeField]
    private Collider2D _collider;

    [SerializeField]
    private GameObject _explosion;

    [SerializeField]
    private GameObject _disableOnDeath;

    [SerializeField]
    private Transform _enemyList;

    private Transform _target = null;

    private bool _seeking = true;

    private void Start()
    {
        _spawnTime = Time.time;
        _enemyList = GameObject.Find("Enemy List").transform;
    }

    private void Update()
    {
        if (_seeking)
        {
            transform.Translate(new Vector3(0, _speed * Time.deltaTime, 0), Space.Self);

            FindTarget();

            if (_target != null)
            {
                Vector2 targetAngle = _target.position - transform.position;
                float angleDifference = Vector2.SignedAngle(Vector2.up, targetAngle);

                Vector3 targetRotation = new Vector3(0, 0, angleDifference);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), _rotationSpeed * Time.deltaTime);
            }

            if (Time.time > _spawnTime + _duration)
            {
                DestroyThisMissile();
            }
        }
    }

    void FindTarget()
    {
        if (_enemyList.childCount == 0)
        {
            _target = null;
        }
        else
        {
            _target = FindClosestEnemy();
        }
    }

    Transform FindClosestEnemy()
    {
        float shortestDistance = float.PositiveInfinity;
        int closestEnemy = 0;

        for (int i = 0, l = _enemyList.childCount; i < l; i++)
        {
            float enemyDistance = Vector3.Distance(gameObject.transform.position, _enemyList.GetChild(i).position);

            if (enemyDistance < shortestDistance)
            {
                shortestDistance = enemyDistance;
                closestEnemy = i;
            }
        }

        return _enemyList.GetChild(closestEnemy);
    }

    void DestroyThisMissile()
    {
        _collider.enabled = false;
        _seeking = false;
        _disableOnDeath.SetActive(false);
        _explosion.SetActive(true);
    }
}

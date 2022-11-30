using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _upperBoundary;

    void Update()
    {
        transform.Translate(new Vector3(0f, 1f, 0f) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y > _upperBoundary)
        {
            Destroy(gameObject);
        }
    }
}

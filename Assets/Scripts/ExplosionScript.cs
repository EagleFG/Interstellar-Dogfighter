using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _destroyedObject;

    [SerializeField]
    private SpriteRenderer _destroyedObjectSprite;

    public void DisableSprite()
    {
        _destroyedObjectSprite.enabled = false;
    }

    public void DestroyObject()
    {
        GameObject.Destroy(_destroyedObject);
    }
}

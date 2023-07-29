using UnityEngine;

public class BossExplosionScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _destroyedObject;

    [SerializeField]
    private SpriteRenderer[] _destroyedObjectSprites;

    public void DisableSprite()
    {
        for (int i = 0, l = _destroyedObjectSprites.Length; i < l; i++)
        {
            _destroyedObjectSprites[i].enabled = false;
        }
    }

    public void DestroyObject()
    {
        GameObject.Destroy(_destroyedObject);
    }
}

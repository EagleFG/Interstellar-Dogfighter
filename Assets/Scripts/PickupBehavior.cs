using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _lowerLimit;

    private enum UpgradeType { TripleShot, SpeedUp, Shield, Ammo, Health, Missile };

    [SerializeField]
    private UpgradeType _upgradeType;

    [SerializeField]
    private AudioClip _upgradeAudio;

    [SerializeField]
    private float _audioVolume;

    private void Update()
    {
        transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y < _lowerLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerController player))
            {
                switch(_upgradeType)
                {
                    case UpgradeType.TripleShot:
                        player.EnableTripleShot();
                        break;
                    case UpgradeType.SpeedUp:
                        player.EnableSpeedUp();
                        break;
                    case UpgradeType.Shield:
                        player.UpdateShieldHealth(1);
                        break;
                    case UpgradeType.Ammo:
                        player.RefillAmmo();
                        break;
                    case UpgradeType.Health:
                        player.HealPlayer(1);
                        break;
                    case UpgradeType.Missile:
                        player.EnableMissile();
                        break;
                }

                AudioSource.PlayClipAtPoint(_upgradeAudio, Camera.main.transform.position, _audioVolume);

                Destroy(gameObject);
            }
        }
    }
}

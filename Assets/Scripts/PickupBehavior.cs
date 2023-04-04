using System.Collections;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    [SerializeField]
    public float _speed = 5f;

    private float _startingSpeed;

    [SerializeField]
    private float _lowerLimit;

    private enum UpgradeType { TripleShot, SpeedUp, Shield, Ammo, Health, Missile, ShieldDisruption };

    [SerializeField]
    private UpgradeType _upgradeType;

    [SerializeField]
    private AudioClip _upgradeAudio;

    [SerializeField]
    private float _audioVolume;

    [SerializeField]
    private LineRenderer _magnetizeLineRenderer;

    private bool _isMagnetized = false;

    private Transform _playerTransform;

    private void Start()
    {
        _startingSpeed = _speed;

        _playerTransform = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        transform.Translate(new Vector3(0f, -1f, 0f) * _speed * Time.deltaTime);

        if (gameObject.transform.position.y < _lowerLimit)
        {
            Destroy(gameObject);
        }

        if (_magnetizeLineRenderer.enabled == true)
        {
            DrawMagnetizeLine();
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
                    case UpgradeType.ShieldDisruption:
                        player.DisruptShield();
                        break;
                }

                AudioSource.PlayClipAtPoint(_upgradeAudio, GameObject.Find("Audio Listener").transform.position, _audioVolume);

                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("EnemyLaser"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Beam"))
        {
            Destroy(gameObject);
        }
    }

    public bool IsMagnetized()
    {
        return _isMagnetized;
    }
    
    public void Magnetize()
    {
        _isMagnetized = true;
        _speed = 0;
        _magnetizeLineRenderer.enabled = true;
        DrawMagnetizeLine();
    }
    
    void DrawMagnetizeLine()
    {
        _magnetizeLineRenderer.SetPosition(0, gameObject.transform.position);
        _magnetizeLineRenderer.SetPosition(1, _playerTransform.position);
    }

    public void Demagnetize()
    {
        _speed = _startingSpeed;
        _magnetizeLineRenderer.enabled = false;
        _isMagnetized = false;
    }
}

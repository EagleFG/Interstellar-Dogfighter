using System.Collections;
using UnityEngine;

public class BossStunPulse : MonoBehaviour
{
    [SerializeField]
    private float _pulseFinalSizeScalar = 45;

    [SerializeField]
    private float _pulseDuration = 2;

    [SerializeField]
    private float _stunDuration = 2;

    [SerializeField]
    private SpriteRenderer _sprite;

    private Vector3 _pulseStartingSize;
    private Color _pulseStartingColor;

    private void Awake()
    {
        _pulseStartingSize = gameObject.transform.localScale;
        _pulseStartingColor = _sprite.color;
    }

    private void OnEnable()
    {
        PulseOut();
    }

    void PulseOut()
    {
        gameObject.transform.localScale = _pulseStartingSize;

        StartCoroutine(GrowSize());
    }

    IEnumerator GrowSize()
    {
        float timeElapsed = 0;
        Color transparentSpriteColor = new Color(_pulseStartingColor.r, _pulseStartingColor.g, _pulseStartingColor.b, 0);

        while (gameObject.transform.localScale.x < _pulseStartingSize.x * _pulseFinalSizeScalar)
        {
            gameObject.transform.localScale = Vector3.Lerp(_pulseStartingSize, _pulseStartingSize * _pulseFinalSizeScalar, timeElapsed / _pulseDuration);
            _sprite.color = Color.Lerp(_pulseStartingColor, transparentSpriteColor, timeElapsed / _pulseDuration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.TryGetComponent(out PlayerController playerController);

            playerController.StunPlayer(_stunDuration);
            playerController.DisruptShield();
        }
    }
}

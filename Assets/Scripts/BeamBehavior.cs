using UnityEngine;

public class BeamBehavior : MonoBehaviour
{
    [SerializeField]
    private Collider2D _beamHitbox;

    [SerializeField]
    private AudioClip _beamAudioClip;

    [SerializeField]
    private float _audioVolume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.DamagePlayer(2);
            }
        }
    }

    void PlayBeamAudio()
    {
        AudioSource.PlayClipAtPoint(_beamAudioClip, GameObject.Find("Audio Listener").transform.position, _audioVolume);
    }

    void EnableBeamHitbox()
    {
        _beamHitbox.enabled = true;
    }

    void DisableBeamHitbox()
    {
        _beamHitbox.enabled = false;
    }

    void EndFireBeam()
    {
        Destroy(gameObject);
    }
}

using System.Collections;
using UnityEngine;

public class BeamBehavior : MonoBehaviour
{
    [SerializeField]
    private Collider2D _beamHitbox;

    [SerializeField]
    private AudioSource _beamAudio;

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

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void PlayBeamAudio()
    {
        _beamAudio.Stop();
        _beamAudio.Play();
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
        StartCoroutine(WaitToDestroyBeam());
    }

    IEnumerator WaitToDestroyBeam()
    {
        while (_beamAudio.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}

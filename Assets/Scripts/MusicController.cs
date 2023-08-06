using System.Collections;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audio;

    [SerializeField]
    private AudioClip _backgroundMusic, _bossMusic, _victoryMusic;

    [SerializeField]
    private float _maxVolume;

    [SerializeField]
    private float _fadeDuration;

    public void PlayBackgroundMusic()
    {
        if (_audio.isPlaying)
        {
            StartCoroutine(TransitionMusic(_backgroundMusic));
        }
        else
        {
            PlayMusic(_backgroundMusic);
        }
    }

    public void PlayBossMusic()
    {
        if (_audio.isPlaying)
        {
            StartCoroutine(TransitionMusic(_bossMusic));
        }
        else
        {
            PlayMusic(_bossMusic);
        }
    }

    public void PlayVictoryMusic()
    {
        if (_audio.isPlaying)
        {
            StartCoroutine(TransitionMusic(_victoryMusic));
        }
        else
        {
            PlayMusic(_victoryMusic);
        }
    }

    IEnumerator TransitionMusic(AudioClip newSong)
    {
        float timeElapsed = 0;

        while (_audio.volume > 0)
        {
            _audio.volume = Mathf.Lerp(_maxVolume, 0, timeElapsed / _fadeDuration);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        PlayMusic(newSong);
    }

    private void PlayMusic(AudioClip newMusic)
    {
        _audio.clip = newMusic;
        _audio.volume = _maxVolume;
        _audio.Play();
    }
}

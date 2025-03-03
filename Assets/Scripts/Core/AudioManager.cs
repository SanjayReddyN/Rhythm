using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource soundEffectSource;

    private void Start()
    {
        PlaySound(backgroundMusic);
    }

    public void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void StopSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
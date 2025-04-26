using UnityEngine;

public static class AudioSourceExtensions
{
    public static void PlayRandom(this AudioSource audioSource, AudioClip[] clips, float volumeScale = 1f)
    {
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volumeScale);
    }
} 
using UnityEngine;

[CreateAssetMenu(menuName = "Juego/Gun", fileName = "RandomPitchShoot")]
public class RandomPitchShoot : ScriptableObject
{
    [Header("Configuración del Audio")]
    [SerializeField] private AudioClip audioClip;

    [Header("Rango de Pitch")]
    [SerializeField] private float minPitch = 0.85f;
    [SerializeField] private float maxPitch = 1.15f;

    [Header("Opciones")]
    [SerializeField] private bool randomPitchOnPlay = true;

    public AudioClip AudioClip => audioClip;

    /// <summary>
    /// Reproduce el audio con pitch aleatorio usando el AudioSource que le pases
    /// </summary>
    public void Play(AudioSource audioSource)
    {
        if (audioSource == null || audioClip == null) return;

        audioSource.clip = audioClip;

        if (randomPitchOnPlay)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        audioSource.PlayOneShot(audioClip);
    }

    public void PlayWithPitch(AudioSource audioSource, float pitch)
    {
        if (audioSource == null || audioClip == null) return;

        audioSource.clip = audioClip;
        audioSource.pitch = Mathf.Clamp(pitch, 0.5f, 2.0f);
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayWithCustomRange(AudioSource audioSource, float min, float max)
    {
        if (audioSource == null || audioClip == null) return;

        audioSource.clip = audioClip;
        audioSource.pitch = Random.Range(min, max);
        audioSource.PlayOneShot(audioClip);
    }

    public void SetAudioClip(AudioClip newClip)
    {
        audioClip = newClip;
    }
}
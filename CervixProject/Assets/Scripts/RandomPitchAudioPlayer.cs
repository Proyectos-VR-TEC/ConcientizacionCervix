using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomPitchAudioPlayer : MonoBehaviour
{
    [Header("Configuración del Audio")]
    [SerializeField] private AudioClip audioClip;
    
    [Header("Rango de Pitch")]
    [Tooltip("El pitch mínimo (0.8 = 80% de la velocidad original)")]
    [SerializeField] private float minPitch = 0.85f;
    
    [Tooltip("El pitch máximo (1.15 = 115% de la velocidad original)")]
    [SerializeField] private float maxPitch = 1.15f;
    
    [Header("Opciones")]
    [Tooltip("Si es true, el pitch cambiará cada vez que se reproduzca")]
    [SerializeField] private bool randomPitchOnPlay = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        
        // Configurar el AudioSource para que no se destruya al cambiar de escena (opcional)
        // audioSource.loop = false; // Por defecto es false, pero lo dejamos claro
    }

    /// <summary>
    /// Reproduce el audio con un pitch aleatorio
    /// </summary>
    public void Play()
    {
        if (audioClip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado en " + gameObject.name);
            return;
        }

        // Asignar el clip (por si cambia en tiempo de ejecución)
        if (audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
        }

        // Aplicar pitch aleatorio
        if (randomPitchOnPlay)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            audioSource.pitch = randomPitch;
        }

        // Reproducir
        audioSource.Play();
    }

    /// <summary>
    /// Reproduce el audio con un pitch específico (sobrescribe el aleatorio)
    /// </summary>
    public void PlayWithPitch(float pitch)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado en " + gameObject.name);
            return;
        }

        audioSource.clip = audioClip;
        audioSource.pitch = Mathf.Clamp(pitch, 0.5f, 2.0f); // Limitar el pitch para evitar distorsiones
        audioSource.Play();
    }

    /// <summary>
    /// Reproduce el audio con un pitch aleatorio en un rango específico (para variaciones puntuales)
    /// </summary>
    public void PlayWithCustomRange(float min, float max)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("No hay AudioClip asignado en " + gameObject.name);
            return;
        }

        audioSource.clip = audioClip;
        audioSource.pitch = Random.Range(min, max);
        audioSource.Play();
    }


    /// <summary>
    /// Cambia el AudioClip en tiempo de ejecución
    /// </summary>
    public void SetAudioClip(AudioClip newClip)
    {
        audioClip = newClip;
        audioSource.clip = newClip;
    }

}
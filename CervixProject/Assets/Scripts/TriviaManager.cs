using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Controla el mini-juego de trivia asociado a cada pieza del rompecabezas.
/// Guarda cuál es el valor (INT) correcto de la pieza que se acaba de colocar,
/// para que los botones de respuesta puedan comparar contra él.
/// </summary>
public class TriviaManager : MonoBehaviour
{
    public static TriviaManager Instance { get; private set; }

    [Header("Valor correcto de la pieza actualmente en trivia")]
    [SerializeField] private int currentCorrectValue;
    public int CurrentCorrectValue => currentCorrectValue;

    public AudioSource audioSource;

    [SerializeField] private AudioClip currentCorrectAudioClip;
    [SerializeField] private AudioClip currentIncorrectAudioClip;

    [Header("Panel de la UI de trivia")]
    public GameObject triviaPanel;

    [Header("Grab interactables de las piezas")]
    public XRGrabInteractable[] grabInteractables;
    
    [Header("Simple interactables de los botones")]
    public XRSimpleInteractable[] simpleInteractables;

    /// <summary>
    /// Llamar esto desde la pieza cuando se coloca en su lugar correcto.
    /// </summary>
    /// <param name="correctValue">El INT correcto que debe coincidir con el botón.</param>
    /// <param name="correctClip">El AudioClip que se reproduce cuando la respuesta es correcta.</param>
    /// <param name="incorrectClip">El AudioClip que se reproduce cuando la respuesta es incorrecta.</param>
    public void StartTrivia(int correctValue, AudioClip correctClip, AudioClip incorrectClip)
    {
        currentCorrectValue = correctValue;
        currentCorrectAudioClip = correctClip;
        currentIncorrectAudioClip = incorrectClip;

        if (triviaPanel != null)
        {
            triviaPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Llamar esto desde el evento "Correcto" de cualquier botón (o manualmente)
    /// cuando quieras cerrar el panel de trivia.
    /// </summary>
    public void EndTrivia()
    {
        if (triviaPanel != null)
        {
            triviaPanel.SetActive(false);
        }
    }

    public void PlayCorrectAudio()
    {
        if (currentCorrectAudioClip != null)
        {
            audioSource.PlayOneShot(currentCorrectAudioClip);
        }
    }

    public void PlayIncorrectAudio()
    {
        if (currentIncorrectAudioClip != null)
        {
            audioSource.PlayOneShot(currentIncorrectAudioClip);
        }
    }

    public void EnableGrabInteractables()
    {
        foreach (var grabInteractable in grabInteractables)
        {
            grabInteractable.enabled = true;
        }
    }

    public void DisableGrabInteractables()
    {
        foreach (var grabInteractable in grabInteractables)
        {
            grabInteractable.enabled = false;
        }
    }

    public void EnableSimpleInteractables()
    {
        foreach (var simpleInteractable in simpleInteractables)
        {
            simpleInteractable.enabled = true;
        }
    }

    public void DisableSimpleInteractables()
    {
        foreach (var simpleInteractable in simpleInteractables)
        {
            simpleInteractable.enabled = false;
        }
    }
}
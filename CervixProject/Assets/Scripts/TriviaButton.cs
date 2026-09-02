using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Este script se coloca en CADA UNO de los 3 botones de respuesta.
/// Cada botón representa un valor INT (ej: 1, 2 o 3).
/// Al hacer click, compara ese valor contra el valor correcto que
/// TriviaManager guardó al colocar la pieza, y dispara la lista de
/// eventos correspondiente.
/// </summary>
public class TriviaButton : MonoBehaviour
{
    [Header("Valor que representa este botón (debe coincidir con el de la pieza)")]
    public int buttonValue;

    [Header("Eventos que se disparan si ESTE botón es la respuesta correcta")]
    public UnityEvent onCorrectAnswer;

    [Header("Eventos que se disparan si ESTE botón NO es la respuesta correcta")]
    public UnityEvent onIncorrectAnswer;
    public TriviaManager triviaManager; // Referencia al TriviaManager


    public void CheckAnswer()
    {
        if (triviaManager == null)
        {
            Debug.LogWarning("TriviaButton: No se encontró un TriviaManager en la escena.");
            return;
        }

        int correctValue = triviaManager.CurrentCorrectValue;

        if (buttonValue == correctValue)
        {
            onCorrectAnswer.Invoke();
        }
        else
        {
            onIncorrectAnswer.Invoke();
        }
    }
}
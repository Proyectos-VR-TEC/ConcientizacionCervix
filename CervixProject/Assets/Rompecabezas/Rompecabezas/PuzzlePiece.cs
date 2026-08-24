using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;
    [Header("Valor correcto")]
    public int correctAnswerValue;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    public void RegresarAPosicionInicial()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
using UnityEngine;

public class LimiteRompecabezas : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PuzzlePiece"))
        {
            PuzzlePiece pieza = collision.gameObject.GetComponentInParent<PuzzlePiece>();
            if (pieza != null)
                pieza.RegresarAPosicionInicial();
        }
    }
}
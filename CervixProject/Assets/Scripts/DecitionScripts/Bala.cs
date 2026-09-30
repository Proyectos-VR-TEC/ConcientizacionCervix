using UnityEngine;

public class Bala : MonoBehaviour
{
    public float velocidad = 20f;
    public float vidaUtil = 50f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = transform.forward * velocidad;
        Destroy(gameObject, vidaUtil);
    }
}
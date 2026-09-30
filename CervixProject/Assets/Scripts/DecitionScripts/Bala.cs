using UnityEngine;

public class Bala : MonoBehaviour
{
    public float flotabilidad = 3f;
    public float vidaUtil = 4f;

    void FixedUpdate()
    {
        // Empuje hacia arriba constante
        GetComponent<Rigidbody>().AddForce(Vector3.up * flotabilidad, ForceMode.Force);
    }

    void Start()
    {
        Destroy(gameObject, vidaUtil);
    }
}
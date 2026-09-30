using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pistola : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject balaPrefab;
    public Transform spawnPoint;
    public float velocidadDisparo = 30f;

    [Header("Cadencia")]
    public float cadenciaDisparos = 0.08f; // segundos entre balas (0.08 ≈ 12 balas/seg)

    [Header("Vida de la bala")]
    public float vidaBala = 10f;

    private XRGrabInteractable grabbable;
    private bool gatilloPresionado = false;
    private float timerDisparo = 0f;

    void Awake()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(OnGatilloPresionado);
        grabbable.deactivated.AddListener(OnGatilloSoltado);
    }

    void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.activated.RemoveListener(OnGatilloPresionado);
            grabbable.deactivated.RemoveListener(OnGatilloSoltado);
        }
    }

    void OnGatilloPresionado(ActivateEventArgs args)
    {
        gatilloPresionado = true;
        timerDisparo = 0f; // dispara inmediatamente al presionar
    }

    void OnGatilloSoltado(DeactivateEventArgs args)
    {
        gatilloPresionado = false;
    }

    void Update()
    {
        if (!gatilloPresionado) return;

        timerDisparo += Time.deltaTime;
        if (timerDisparo >= cadenciaDisparos)
        {
            timerDisparo = 0f;
            Disparar();
        }
    }

    void Disparar()
    {
        if (balaPrefab == null || spawnPoint == null) return;

        GameObject nuevaBala = Instantiate(balaPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * velocidadDisparo;
        }

        Destroy(nuevaBala, vidaBala);
    }
}
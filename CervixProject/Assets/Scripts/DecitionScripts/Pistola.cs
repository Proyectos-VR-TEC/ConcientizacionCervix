using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pistola : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject balaPrefab;
    public Transform spawnPoint;
    public float velocidadDisparo = 30f;
    public RandomPitchShoot shootData;
    public AudioSource audioSource;

    [Header("Cadencia")]
    public float cadenciaDisparos = 0.08f;

    [Header("Vida de la bala")]
    public float vidaBala = 10f;

    [Header("Escala de la bala")]           // 🆕
    public float escalaMin = 0.03f;          // 🆕
    public float escalaMax = 0.10f;          // 🆕

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
        timerDisparo = 0f;
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

        // 🆕 Escala aleatoria entre min y max
        float escala = Random.Range(escalaMin, escalaMax);
        nuevaBala.transform.localScale = Vector3.one * escala;

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = spawnPoint.forward * velocidadDisparo;
        }

        if (shootData != null)
        {
            shootData.Play(audioSource);
        }

        Destroy(nuevaBala, vidaBala);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IntroExperienceManager : MonoBehaviour
{
    [Header("Intro UI")]
    public CanvasGroup introCanvasGroup;
    public GameObject introCanvasObject;

    [Header("XR Player")]
    public Transform xrOrigin;
    public Transform xrCamera;
    public Transform playerTableSpawn;

    [Header("Room Lighting")]
    public Light[] roomLights;
    public float darkIntensity = 0.05f;
    public float lightFadeTime = 1.5f;

    [Header("Objects")]
    public GameObject[] objectsToHideAtStart;
    public GameObject[] objectsToShowAfterStart;
    public GameObject puzzleGhost;

    [Header("Audio")]
    public AudioSource introNarration;
    public AudioSource startSound;

    private float[] originalLightIntensities;
    private bool experienceStarted;

    [Header("Panel Inicio Rompecabezas")]
    public GameObject panelInicioRompecabezas;
    public CanvasGroup panelInicioCanvasGroup;
    public AudioClip locucionInicioRompecabezas;
    public float duracionPanelInicio = 6f;
    public float fadePanelInicio = 1f;

    [Header("Piezas")]
    public XRGrabInteractable[] piezas;

    public void ComenzarRompecabezas()
    {
        StartCoroutine(FadeYComenzar());
    }

    IEnumerator FadeYComenzar()
    {
        float elapsed = 0f;
        while (elapsed < fadePanelInicio)
        {
            elapsed += Time.deltaTime;
            panelInicioCanvasGroup.alpha = 1f - (elapsed / fadePanelInicio);
            yield return null;
        }
        panelInicioRompecabezas.SetActive(false);

        foreach (var pieza in piezas)
        {
            if (pieza != null)
                pieza.gameObject.SetActive(true);
        }

        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
            audio.Stop();

    }

    private void Awake()
    {
        originalLightIntensities = new float[roomLights.Length];

        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null)
            {
                originalLightIntensities[i] = roomLights[i].intensity;
            }
        }
    }

    private void Start()
    {
        experienceStarted = false;

        if (introCanvasObject != null)
            introCanvasObject.SetActive(true);

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 1f;
            introCanvasGroup.interactable = true;
            introCanvasGroup.blocksRaycasts = true;
        }

        foreach (var pieza in piezas)
        {
            if (pieza != null)
                pieza.gameObject.SetActive(false);
        }

        foreach (GameObject obj in objectsToHideAtStart)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToShowAfterStart)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (puzzleGhost != null)
            puzzleGhost.SetActive(false);

        foreach (Light light in roomLights)
        {
            if (light != null)
                light.intensity = darkIntensity;
        }

        if (introNarration != null)
            introNarration.Play();
    }

    public void StartExperience()
    {
        if (experienceStarted)
            return;

        experienceStarted = true;

        if (startSound != null)
            startSound.Play();

        StartCoroutine(StartExperienceRoutine());
    }

    private IEnumerator StartExperienceRoutine()
    {
        // MovePlayerToTable();

        foreach (GameObject obj in objectsToShowAfterStart)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        float timer = 0f;

        while (timer < lightFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / lightFadeTime;

            if (introCanvasGroup != null)
                introCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                {
                    roomLights[i].intensity = Mathf.Lerp(
                        darkIntensity,
                        originalLightIntensities[i],
                        t
                    );
                }
            }

            yield return null;
        }

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 0f;
            introCanvasGroup.interactable = false;
            introCanvasGroup.blocksRaycasts = false;
        }

        if (introCanvasObject != null)
            introCanvasObject.SetActive(false);

        if (panelInicioRompecabezas != null)
            StartCoroutine(MostrarPanelInicio());
    }

    private void MovePlayerToTable()
    {
        if (xrOrigin == null || xrCamera == null || playerTableSpawn == null)
            return;

        float cameraYaw = xrCamera.eulerAngles.y;
        float targetYaw = playerTableSpawn.eulerAngles.y;
        float yawDifference = targetYaw - cameraYaw;

        xrOrigin.Rotate(0f, yawDifference, 0f, Space.World);

        Vector3 cameraOffset = xrCamera.position - xrOrigin.position;
        xrOrigin.position = playerTableSpawn.position - cameraOffset;
    }

    IEnumerator MostrarPanelInicio()
    {
        panelInicioRompecabezas.SetActive(true);
        panelInicioCanvasGroup.alpha = 1f;

        if (locucionInicioRompecabezas != null)
        {
            AudioSource audio = GetComponent<AudioSource>();
            if (audio == null) audio = gameObject.AddComponent<AudioSource>();
            audio.PlayOneShot(locucionInicioRompecabezas);
        }

        yield return null;
    }

}

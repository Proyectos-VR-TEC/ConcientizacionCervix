using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    private Coroutine currentPanelCoroutine;
    private GameObject currentPanel;
    private CanvasGroup currentCanvasGroup;
    private AudioSource currentAudioSource;

    [Header("Transición")]
    public int totalPiezas = 6;
    public float esperaAntesDeTransicion = 3f;
    public Transform xrOrigin;
    public Transform xrCamera;
    public Transform spawnSala2;
    public CanvasGroup fadeCanvasGroup;
    public GameObject fadeCanvasObject;
    public float fadeTransicionDuration = 2f;

    [Header("Intro Sala 2 - Podios")]
    public GameObject panelPodio1;
    public GameObject panelPodio2;
    public AudioClip loc10;
    public AudioClip loc11;
    public AudioSource audioPodios;
    public float delayAudioDespuesFade = 0.7f;

    private bool secuenciaPodiosIniciada = false;

    [Header("Configuración visual del Fade")]
    public RectTransform fadePanelRect;
    public float fadeDistanceFromCamera = 0.45f;
    public Vector2 fadeCanvasSize = new Vector2(6000f, 3500f);
    public float fadeCanvasScale = 0.001f;

    [Header("Objetos que se ocultan durante el Fade")]
    public GameObject[] objetosOcultarDuranteFade;

    private bool[] estadosOriginalesObjetosFade;

    private int piezasColocadas = 0;
    private bool finalActivado = false;
    private bool transicionEnCurso = false;

    [Header("Panel Final")]
    public GameObject panelFinal;
    public CanvasGroup panelFinalCanvasGroup;
    public AudioClip locucionFinal;
    public float fadePanelFinal = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PrepararFadeCanvasInicial();

        if (panelFinal != null)
            panelFinal.SetActive(false);

        if (panelFinalCanvasGroup != null)
        {
            panelFinalCanvasGroup.alpha = 0f;
            panelFinalCanvasGroup.interactable = false;
            panelFinalCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowPanel(
        GameObject panel,
        CanvasGroup canvasGroup,
        AudioSource audioSource,
        AudioClip locucion,
        float locucionDelay,
        float duration,
        float fadeDuration
    )
    {
        if (currentPanelCoroutine != null)
            StopCoroutine(currentPanelCoroutine);

        if (currentPanel != null)
            currentPanel.SetActive(false);

        if (currentAudioSource != null)
            currentAudioSource.Stop();

        currentPanel = panel;
        currentCanvasGroup = canvasGroup;
        currentAudioSource = audioSource;

        currentPanelCoroutine = StartCoroutine(
            RunPanel(panel, canvasGroup, audioSource, locucion, locucionDelay, duration, fadeDuration)
        );
    }

    public void PiezaColocada()
    {
        if (finalActivado)
            return;

        piezasColocadas++;

        if (piezasColocadas >= totalPiezas)
        {
            finalActivado = true;
            StartCoroutine(MostrarPanelFinal());
        }
    }

    IEnumerator MostrarPanelFinal()
    {
        yield return new WaitForSeconds(esperaAntesDeTransicion);

        if (currentAudioSource != null)
            yield return new WaitWhile(() => currentAudioSource.isPlaying);

        yield return new WaitForSeconds(1.3f);

        if (panelFinal != null)
            panelFinal.SetActive(true);

        if (panelFinalCanvasGroup != null)
        {
            panelFinalCanvasGroup.alpha = 1f;
            panelFinalCanvasGroup.interactable = true;
            panelFinalCanvasGroup.blocksRaycasts = true;
        }

        if (locucionFinal != null)
        {
            AudioSource audio = GetComponent<AudioSource>();

            if (audio == null)
                audio = gameObject.AddComponent<AudioSource>();

            audio.PlayOneShot(locucionFinal);
        }
    }

    public void ContinuarTransicion()
    {
        if (transicionEnCurso)
            return;

        transicionEnCurso = true;
        StartCoroutine(FadeYTransicion());
    }

    IEnumerator FadeYTransicion()
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (audio != null && audio.isPlaying)
        {
            float startVolume = audio.volume;
            float elapsedAudio = 0f;

            while (elapsedAudio < 0.5f)
            {
                elapsedAudio += Time.deltaTime;
                audio.volume = Mathf.Lerp(startVolume, 0f, elapsedAudio / 0.5f);
                yield return null;
            }

            audio.Stop();
            audio.volume = startVolume;
        }

        if (panelFinalCanvasGroup != null)
        {
            float elapsed = 0f;

            while (elapsed < fadePanelFinal)
            {
                elapsed += Time.deltaTime;
                panelFinalCanvasGroup.alpha = 1f - (elapsed / fadePanelFinal);
                yield return null;
            }

            panelFinalCanvasGroup.alpha = 0f;
            panelFinalCanvasGroup.interactable = false;
            panelFinalCanvasGroup.blocksRaycasts = false;
        }

        if (panelFinal != null)
            panelFinal.SetActive(false);

        yield return StartCoroutine(TransicionSala2());
    }

    IEnumerator RunPanel(
        GameObject panel,
        CanvasGroup canvasGroup,
        AudioSource audioSource,
        AudioClip locucion,
        float locucionDelay,
        float duration,
        float fadeDuration
    )
    {
        if (panel == null || canvasGroup == null)
            yield break;

        panel.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (locucion != null && audioSource != null)
        {
            yield return new WaitForSeconds(locucionDelay);
            audioSource.PlayOneShot(locucion);
        }

        yield return new WaitForSeconds(duration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        panel.SetActive(false);
    }

    IEnumerator TransicionSala2()
    {
        yield return new WaitForSeconds(0.2f);

        PrepararFadeCanvasParaTransicion();

        // Ocultar mandos, manos y rayos para que no se vean encima del fade.
        OcultarObjetosDuranteFade();

        float elapsed = 0f;

        while (elapsed < fadeTransicionDuration)
        {
            elapsed += Time.deltaTime;

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeTransicionDuration);

            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        MoverJugadorASala2();

        yield return new WaitForSeconds(0.2f);

        elapsed = 0f;

        while (elapsed < fadeTransicionDuration)
        {
            elapsed += Time.deltaTime;

            if (fadeCanvasGroup != null)
                fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeTransicionDuration);

            yield return null;
        }

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (fadeCanvasObject != null)
            fadeCanvasObject.SetActive(false);

        // Volver a mostrar mandos, manos y rayos.
        RestaurarObjetosDuranteFade();

        // Iniciar paneles y locución de la Sala 2 después de que todo ya aclaró
        StartCoroutine(IniciarSecuenciaPodiosDespuesDelFade());
    }

    private void MoverJugadorASala2()
    {
        if (xrOrigin == null || xrCamera == null || spawnSala2 == null)
        {
            Debug.LogWarning("Faltan referencias para mover al jugador a Sala 2.");
            return;
        }

        float cameraYaw = xrCamera.eulerAngles.y;
        float targetYaw = spawnSala2.eulerAngles.y;
        float yawDiff = targetYaw - cameraYaw;

        xrOrigin.Rotate(0f, yawDiff, 0f, Space.World);

        Vector3 offset = xrCamera.position - xrOrigin.position;
        xrOrigin.position = spawnSala2.position - offset;
    }

    private void PrepararFadeCanvasInicial()
    {
        if (fadeCanvasObject == null || fadeCanvasGroup == null)
            return;

        ConfigurarFadeCanvas();

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;

        fadeCanvasObject.SetActive(false);
    }

    private void PrepararFadeCanvasParaTransicion()
    {
        if (fadeCanvasObject == null || fadeCanvasGroup == null)
        {
            Debug.LogWarning("No está asignado FadeCanvasObject o FadeCanvasGroup.");
            return;
        }

        fadeCanvasObject.SetActive(true);

        ConfigurarFadeCanvas();

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    private void ConfigurarFadeCanvas()
    {
        if (fadeCanvasObject == null || xrCamera == null)
            return;

        Transform fadeTransform = fadeCanvasObject.transform;

        fadeTransform.SetParent(xrCamera, false);
        fadeTransform.localPosition = new Vector3(0f, 0f, fadeDistanceFromCamera);
        fadeTransform.localRotation = Quaternion.identity;
        fadeTransform.localScale = Vector3.one * fadeCanvasScale;

        RectTransform fadeRect = fadeCanvasObject.GetComponent<RectTransform>();

        if (fadeRect != null)
        {
            fadeRect.anchorMin = new Vector2(0.5f, 0.5f);
            fadeRect.anchorMax = new Vector2(0.5f, 0.5f);
            fadeRect.pivot = new Vector2(0.5f, 0.5f);
            fadeRect.anchoredPosition = Vector2.zero;
            fadeRect.sizeDelta = fadeCanvasSize;
            fadeRect.localPosition = new Vector3(0f, 0f, fadeDistanceFromCamera);
            fadeRect.localRotation = Quaternion.identity;
            fadeRect.localScale = Vector3.one * fadeCanvasScale;
        }

        Canvas canvas = fadeCanvasObject.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = xrCamera.GetComponent<Camera>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 9999;
        }

        if (fadePanelRect == null)
        {
            Transform panel = fadeCanvasObject.transform.Find("Panel");

            if (panel != null)
                fadePanelRect = panel.GetComponent<RectTransform>();
        }

        if (fadePanelRect != null)
        {
            fadePanelRect.anchorMin = Vector2.zero;
            fadePanelRect.anchorMax = Vector2.one;
            fadePanelRect.pivot = new Vector2(0.5f, 0.5f);
            fadePanelRect.offsetMin = Vector2.zero;
            fadePanelRect.offsetMax = Vector2.zero;
            fadePanelRect.anchoredPosition = Vector2.zero;
            fadePanelRect.localPosition = Vector3.zero;
            fadePanelRect.localRotation = Quaternion.identity;
            fadePanelRect.localScale = Vector3.one;
        }

        Image fadeImage = null;

        if (fadePanelRect != null)
            fadeImage = fadePanelRect.GetComponent<Image>();

        if (fadeImage != null)
        {
            fadeImage.sprite = null;
            fadeImage.type = Image.Type.Simple;
            fadeImage.color = Color.black;
            fadeImage.raycastTarget = false;
        }
    }

    private void OcultarObjetosDuranteFade()
    {
        if (objetosOcultarDuranteFade == null || objetosOcultarDuranteFade.Length == 0)
            return;

        estadosOriginalesObjetosFade = new bool[objetosOcultarDuranteFade.Length];

        for (int i = 0; i < objetosOcultarDuranteFade.Length; i++)
        {
            GameObject obj = objetosOcultarDuranteFade[i];

            if (obj == null)
                continue;

            estadosOriginalesObjetosFade[i] = obj.activeSelf;
            obj.SetActive(false);
        }
    }

    private void RestaurarObjetosDuranteFade()
    {
        if (objetosOcultarDuranteFade == null || estadosOriginalesObjetosFade == null)
            return;

        for (int i = 0; i < objetosOcultarDuranteFade.Length; i++)
        {
            GameObject obj = objetosOcultarDuranteFade[i];

            if (obj == null)
                continue;

            obj.SetActive(estadosOriginalesObjetosFade[i]);
        }

        estadosOriginalesObjetosFade = null;
    }

    private IEnumerator IniciarSecuenciaPodiosDespuesDelFade()
    {
        if (secuenciaPodiosIniciada)
            yield break;

        secuenciaPodiosIniciada = true;

        yield return new WaitForSeconds(delayAudioDespuesFade);

        if (panelPodio1 != null)
            panelPodio1.SetActive(true);

        if (panelPodio2 != null)
            panelPodio2.SetActive(false);

        ReproducirAudioPodio(loc10);
    }

    public void ContinuarAPanelPodio2()
    {
        if (panelPodio1 != null)
            panelPodio1.SetActive(false);

        if (panelPodio2 != null)
            panelPodio2.SetActive(true);

        ReproducirAudioPodio(loc11);
    }

    private void ReproducirAudioPodio(AudioClip clip)
    {
        if (audioPodios == null)
        {
            audioPodios = GetComponent<AudioSource>();

            if (audioPodios == null)
                audioPodios = gameObject.AddComponent<AudioSource>();
        }

        audioPodios.Stop();

        if (clip != null)
        {
            audioPodios.clip = clip;
            audioPodios.Play();
        }
    }
}
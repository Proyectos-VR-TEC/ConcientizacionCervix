using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PanelProteccion : MonoBehaviour
{
    [Header("Componentes visuales")]
    public GameObject imagenObj;
    public GameObject pantallaVideo;
    public VideoPlayer videoPlayer;

    [Header("Videos")]
    public VideoClip videoInicial;
    public VideoClip videoConsecuenciaSi;
    public VideoClip videoConsecuenciaNo;

    [Header("Locuciones / Audios")]
    public AudioSource audioSource;
    public AudioClip audioProteccion;
    public AudioClip audioProteccionPositivo;
    public AudioClip audioProteccionNegativo;

    [Header("Cambio automático")]
    public float tiempoCambioPregunta = 5f;

    [Header("Cuadros / Imágenes")]
    public Sprite ProteccionInicial;
    public Sprite ProteccionPregunta;
    public Sprite ProteccionPositiva;
    public Sprite ProteccionNegativa;

    public static bool eligioSiCondon = false;

    private Image imagen;
    private bool esperandoCambio = false;
    private float timerCambio = 0f;

    void Start()
    {
        if (imagenObj != null)
        {
            imagen = imagenObj.GetComponent<Image>();
            imagenObj.SetActive(true);
        }

        if (pantallaVideo != null)
            pantallaVideo.SetActive(false);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (esperandoCambio)
        {
            timerCambio += Time.deltaTime;

            if (timerCambio >= tiempoCambioPregunta)
            {
                esperandoCambio = false;

                if (imagen != null && ProteccionPregunta != null)
                    imagen.sprite = ProteccionPregunta;
            }
        }
    }

    public void AlAgarrarCondon()
    {
        if (imagen != null && ProteccionInicial != null)
            imagen.sprite = ProteccionInicial;

        if (imagenObj != null)
            imagenObj.SetActive(true);

        if (pantallaVideo != null)
            pantallaVideo.SetActive(true);

        if (videoPlayer != null && videoInicial != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = videoInicial;
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        ReproducirAudio(audioProteccion);

        // Iniciar timer para cambiar automáticamente al panel de pregunta
        esperandoCambio = true;
        timerCambio = 0f;
    }

    public void MostrarConsecuenciaSi()
    {
        eligioSiCondon = true;

        esperandoCambio = false;

        if (imagen != null && ProteccionPositiva != null)
            imagen.sprite = ProteccionPositiva;

        if (imagenObj != null)
            imagenObj.SetActive(true);

        if (pantallaVideo != null)
            pantallaVideo.SetActive(true);

        if (videoPlayer != null && videoConsecuenciaSi != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = videoConsecuenciaSi;
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        ReproducirAudio(audioProteccionPositivo);
    }

    public void MostrarConsecuenciaNo()
    {
        eligioSiCondon = false;

        esperandoCambio = false;

        if (imagen != null && ProteccionNegativa != null)
            imagen.sprite = ProteccionNegativa;

        if (imagenObj != null)
            imagenObj.SetActive(true);

        if (pantallaVideo != null)
            pantallaVideo.SetActive(true);

        if (videoPlayer != null && videoConsecuenciaNo != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = videoConsecuenciaNo;
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        ReproducirAudio(audioProteccionNegativo);
    }

    private void ReproducirAudio(AudioClip clip)
    {
        if (audioSource == null)
            return;

        audioSource.Stop();

        if (clip == null)
            return;

        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }
}
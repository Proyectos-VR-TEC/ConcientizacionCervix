using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PanelCalendario : MonoBehaviour
{
    [Header("Componentes")]
    public GameObject imagenObj;
    public GameObject pantallaVideo;
    public VideoPlayer videoPlayer;

    [Header("Imágenes")]
    public Sprite citologiaInicial;
    public Sprite citologiaPregunta;
    public Sprite citologiaPositivo;
    public Sprite citologiaNegativo;

    [Header("Locuciones / Audios")]
    public AudioSource audioSource;
    public AudioClip audioCitologia;
    public AudioClip audioCitologiaPositivo;
    public AudioClip audioCitologiaNegativo;

    [Header("Videos")]
    public VideoClip videoInicial;
    public VideoClip videoConsecuenciaSi;
    public VideoClip videoConsecuenciaNo;

    [Header("Cambio automático a pregunta")]
    public float tiempoParaMostrarPregunta = 5f;

    private Image imagen;
    private bool esperandoCambio = false;
    private bool preguntaYaMostrada = false;

    public static bool eligioSiCalendario = false;

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
        if (!esperandoCambio)
            return;

        if (preguntaYaMostrada)
            return;

        if (audioSource == null)
            return;

        if (audioSource.clip != audioCitologia)
            return;

        if (!audioSource.isPlaying)
            return;

        if (audioSource.time >= tiempoParaMostrarPregunta)
        {
            MostrarPanelPregunta();
        }
    }

    public void AlAgarrarCalendario()
    {
        esperandoCambio = true;
        preguntaYaMostrada = false;

        if (imagen != null && citologiaInicial != null)
            imagen.sprite = citologiaInicial;

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

        ReproducirAudio(audioCitologia);
    }

    private void MostrarPanelPregunta()
    {
        preguntaYaMostrada = true;

        if (imagen != null && citologiaPregunta != null)
            imagen.sprite = citologiaPregunta;
    }

    public void MostrarConsecuenciaSi()
    {
        eligioSiCalendario = true;

        esperandoCambio = false;
        preguntaYaMostrada = true;

        if (imagen != null && citologiaPositivo != null)
            imagen.sprite = citologiaPositivo;

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

        ReproducirAudio(audioCitologiaPositivo);
    }

    public void MostrarConsecuenciaNegativa()
    {
        eligioSiCalendario = false;

        esperandoCambio = false;
        preguntaYaMostrada = true;

        if (imagen != null && citologiaNegativo != null)
            imagen.sprite = citologiaNegativo;

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

        ReproducirAudio(audioCitologiaNegativo);
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
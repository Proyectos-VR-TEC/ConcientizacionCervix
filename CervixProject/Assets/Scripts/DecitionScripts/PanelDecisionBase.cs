using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public abstract class PanelDecisionBase : MonoBehaviour
{
    [Header("Componentes visuales")]
    public GameObject imagenObj;
    public GameObject pantallaVideo;
    public VideoPlayer videoPlayer;

    [Header("Sprites")]
    public Sprite spriteInicial;
    public Sprite spritePregunta;
    public Sprite spritePositivo;
    public Sprite spriteNegativo;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip audioInicial;
    public AudioClip audioPositivo;
    public AudioClip audioNegativo;

    [Header("Videos")]
    public VideoClip videoInicial;
    public VideoClip videoConsecuenciaSi;
    public VideoClip videoConsecuenciaNo;

    [Header("Tiempo para mostrar pregunta")]
    public float tiempoParaPregunta = 5f;

    protected Image imagen;
    private bool preguntaMostrada = false;

    protected virtual void Awake()
    {
        if (imagenObj != null)
            imagen = imagenObj.GetComponent<Image>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    protected virtual void Start()
    {
        if (pantallaVideo != null)
            pantallaVideo.SetActive(false);
    }

    public virtual void AlAgarrar()
    {
        preguntaMostrada = false;

        SetSprite(spriteInicial);
        if (imagenObj != null) imagenObj.SetActive(true);
        if (pantallaVideo != null) pantallaVideo.SetActive(true);

        PlayVideo(videoInicial);
        PlayAudio(audioInicial);
    }

    public virtual void MostrarConsecuenciaSi()
    {
        preguntaMostrada = true;
        SetSprite(spritePositivo);
        PlayVideo(videoConsecuenciaSi);
        PlayAudio(audioPositivo);
    }

    public virtual void MostrarConsecuenciaNo()
    {
        preguntaMostrada = true;
        SetSprite(spriteNegativo);
        PlayVideo(videoConsecuenciaNo);
        PlayAudio(audioNegativo);
    }

    protected void SetSprite(Sprite s)
    {
        if (imagen != null && s != null)
            imagen.sprite = s;
    }

    protected void PlayVideo(VideoClip clip)
    {
        if (videoPlayer == null || clip == null) return;
        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    protected void PlayAudio(AudioClip clip)
    {
        if (audioSource == null) return;
        audioSource.Stop();
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }

    protected virtual void Update()
    {
        if (preguntaMostrada) return;
        if (audioSource == null || !audioSource.isPlaying) return;
        if (audioSource.clip != audioInicial) return;

        if (audioSource.time >= tiempoParaPregunta)
        {
            SetSprite(spritePregunta);
            preguntaMostrada = true;
        }
    }

    public void ResetPanel()
    {
        preguntaMostrada = false;
        if (audioSource != null) audioSource.Stop();
        if (videoPlayer != null) videoPlayer.Stop();
    }
}
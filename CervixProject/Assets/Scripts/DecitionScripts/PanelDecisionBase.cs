using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

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

    // Callback que el DecisionManager asigna al activar el panel
    public Action AlTerminarConsecuencia;

    // Para saber si estamos esperando a que termine la consecuencia
    private bool esperandoFinConsecuencia = false;
    private float timerConsecuencia = 0f;
    private float duracionConsecuencia = 0f;

    protected virtual void Awake()
    {
        if (imagenObj != null)
            imagen = imagenObj.GetComponent<Image>();

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
        IniciarEsperaConsecuencia(videoConsecuenciaSi, audioPositivo);
    }

    public virtual void MostrarConsecuenciaNo()
    {
        preguntaMostrada = true;
        SetSprite(spriteNegativo);
        PlayVideo(videoConsecuenciaNo);
        PlayAudio(audioNegativo);
        IniciarEsperaConsecuencia(videoConsecuenciaNo, audioNegativo);
    }

    // Calcula cuánto dura la consecuencia más larga (video o audio)
    private void IniciarEsperaConsecuencia(VideoClip video, AudioClip audio)
    {
        float durVideo = (video != null) ? (float)video.length : 0f;
        float durAudio = (audio != null) ? audio.length : 0f;
        duracionConsecuencia = Mathf.Max(durVideo, durAudio);

        // Fallback: si no hay ni video ni audio, esperamos 2s
        if (duracionConsecuencia <= 0f) duracionConsecuencia = 2f;

        timerConsecuencia = 0f;
        esperandoFinConsecuencia = true;
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
        // Cambio automático a la imagen de pregunta
        if (!preguntaMostrada &&
            audioSource != null &&
            audioSource.isPlaying &&
            audioSource.clip == audioInicial &&
            audioSource.time >= tiempoParaPregunta)
        {
            SetSprite(spritePregunta);
            preguntaMostrada = true;
        }

        // Fin de la consecuencia
        if (esperandoFinConsecuencia)
        {
            timerConsecuencia += Time.deltaTime;
            if (timerConsecuencia >= duracionConsecuencia)
            {
                esperandoFinConsecuencia = false;
                AlTerminarConsecuencia?.Invoke();
            }
        }
    }

    public void ResetPanel()
    {
        preguntaMostrada = false;
        esperandoFinConsecuencia = false;
        timerConsecuencia = 0f;
        if (audioSource != null) audioSource.Stop();
        if (videoPlayer != null) videoPlayer.Stop();
    }
}
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PanelFolder : MonoBehaviour
{
    [Header("Componentes")]
    public GameObject imagenObj;
    public GameObject pantallaVideo;
    public VideoPlayer videoPlayer;

    [Header("Cuadros según decisiones")]
    public Sprite casoSiSi;    // condón SI + calendario SI
    public Sprite casoNoSi;    // condón NO + calendario SI
    public Sprite casoSiNo;    // condón SI + calendario NO
    public Sprite casoNoNo;    // condón NO + calendario NO

    [Header("Videos")]
    public VideoClip videoInicial;
    public VideoClip videoConsecuenciaSi;
    public VideoClip videoConsecuenciaNo;

    [Header("Videos finales")]
    public VideoClip final1; // Sí-Sí-Sí
    public VideoClip final2; // Cualquier otra combinación + Sí en folder
    public VideoClip final3; // Cualquier otra combinación + No en folder

    private Image imagen;

    void Start()
    {
        imagen = imagenObj.GetComponent<Image>();
        imagenObj.SetActive(true);
        pantallaVideo.SetActive(false);
    }

    public void AlAgarrarFolder()
    {
        // Determinar qué cuadro mostrar según decisiones anteriores
        bool siCondon = PanelProteccion.eligioSiCondon;
        bool siCalendario = PanelCalendario.eligioSiCalendario;

        Debug.Log($"Folder agarrado - Condón: {siCondon}, Calendario: {siCalendario}");

        if (siCondon && siCalendario)
        {
            Debug.Log("Mostrando: CasoSiSi");
            imagen.sprite = casoSiSi;
        }
        else if (!siCondon && siCalendario)
        {
            Debug.Log("Mostrando: CasoNoSi");
            imagen.sprite = casoNoSi;
        }
        else if (siCondon && !siCalendario)
        {
            Debug.Log("Mostrando: CasoSiNo");
            imagen.sprite = casoSiNo;
        }
        else
        {
            Debug.Log("Mostrando: CasoNoNo");
            imagen.sprite = casoNoNo;
        }

        imagenObj.SetActive(true);
        pantallaVideo.SetActive(true);
        videoPlayer.clip = videoInicial;
        videoPlayer.Play();
    }

    public void MostrarConsecuenciaSi()
    {
        VideoClip videoFinal = DeterminarVideoFinal(true);
        pantallaVideo.SetActive(true);
        videoPlayer.clip = videoFinal;
        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    public void MostrarConsecuenciaNegativa()
    {
        VideoClip videoFinal = DeterminarVideoFinal(false);
        pantallaVideo.SetActive(true);
        videoPlayer.clip = videoFinal;
        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    private VideoClip DeterminarVideoFinal(bool eligioSiFolder)
    {
        bool ambosSi = PanelProteccion.eligioSiCondon && PanelCalendario.eligioSiCalendario;

        if (ambosSi && eligioSiFolder)
            return final1;
        else if (eligioSiFolder)
            return final2;
        else
            return final3;
    }
}
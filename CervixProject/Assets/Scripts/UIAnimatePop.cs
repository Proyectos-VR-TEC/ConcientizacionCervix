using System.Collections;
using UnityEngine;

public class UIAnimatePop : MonoBehaviour
{
    [Header("Configuración de Animación")]
    [Tooltip("Tiempo que dura la animación de aparecer")]
    [SerializeField] private float duracionAparecer = 0.25f;
    
    [Tooltip("Tiempo que dura la animación de desaparecer")]
    [SerializeField] private float duracionDesaparecer = 0.2f;
    
    [Tooltip("Factor de escalado para el efecto rebote (1.2 = 120% del tamaño original)")]
    [SerializeField] private float factorRebote = 1.2f;
    
    [Tooltip("Si es true, el objeto escala desde 0; si es false, solo hace el rebote")]
    [SerializeField] private bool animarDesdeCero = true;

    private Vector3 escalaOriginal;
    private bool estaAnimando = false;

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    void OnEnable()
    {
        // Si el objeto se activa mientras está animando, detener todo
        if (estaAnimando)
        {
            StopAllCoroutines();
            estaAnimando = false;
        }

        // Iniciar la animación de aparecer
        StartCoroutine(AnimarAparecer());
    }

    void OnDisable()
    {
        // Detener animaciones al desactivarse (para evitar errores)
        StopAllCoroutines();
        estaAnimando = false;
    }

    /// <summary>
    /// Animación de aparecer (pop in)
    /// </summary>
    [ContextMenu("POP IN")]
    private IEnumerator AnimarAparecer()
    {
        estaAnimando = true;

        // Si queremos animar desde cero, empezamos así
        if (animarDesdeCero)
        {
            transform.localScale = Vector3.zero;
        }
        else
        {
            // Si no, empezamos desde la escala original
            transform.localScale = escalaOriginal * 0.9f;
        }

        // Calculamos la escala máxima (para el rebote)
        Vector3 escalaMaxima = escalaOriginal * factorRebote;
        
        // 1. Fase 1: Crecer hasta el punto máximo (rebote)
        float tiempo = 0;
        while (tiempo < duracionAparecer)
        {
            float progreso = tiempo / duracionAparecer;
            // Usamos una curva de easing para que sea más suave
            float progresoSuave = 1 - Mathf.Pow(1 - progreso, 3); // Ease Out Cubic
            
            Vector3 escalaInicial = animarDesdeCero ? Vector3.zero : (escalaOriginal * 0.9f);
            transform.localScale = Vector3.Lerp(escalaInicial, escalaMaxima, progresoSuave);
            
            tiempo += Time.deltaTime;
            yield return null;
        }

        // 2. Fase 2: Regresar a la escala original (efecto rebote)
        tiempo = 0;
        float duracionRebote = duracionAparecer * 0.4f; // El rebote dura menos
        
        while (tiempo < duracionRebote)
        {
            float progreso = tiempo / duracionRebote;
            // Ease Out Back para un rebote más natural
            float progresoSuave = 1 - Mathf.Pow(1 - progreso, 2);
            
            transform.localScale = Vector3.Lerp(escalaMaxima, escalaOriginal, progresoSuave);
            
            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar que quede en la escala original
        transform.localScale = escalaOriginal;
        estaAnimando = false;
    }

    /// <summary>
    /// Animación de desaparecer (pop out) - Llamar desde otro script
    /// </summary>
    [ContextMenu("POP OUT")]
    public void PopOut()
    {
        if (estaAnimando)
        {
            StopAllCoroutines();
            estaAnimando = false;
        }
        
        StartCoroutine(AnimarDesaparecer());
    }

    /// <summary>
    /// Animación de desaparecer (pop out) con callback al finalizar
    /// </summary>
    public void PopOut(System.Action alTerminar)
    {
        if (estaAnimando)
        {
            StopAllCoroutines();
            estaAnimando = false;
        }
        
        StartCoroutine(AnimarDesaparecer(alTerminar));
    }

    /// <summary>
    /// Animación de desaparecer (pop out)
    /// </summary>
    private IEnumerator AnimarDesaparecer()
    {
        yield return AnimarDesaparecer(null);
    }

    private IEnumerator AnimarDesaparecer(System.Action alTerminar)
    {
        estaAnimando = true;

        // 1. Fase 1: Crecer un poco (efecto de anticipación)
        Vector3 escalaMaxima = escalaOriginal * factorRebote;
        float tiempo = 0;
        float duracionAnticipacion = duracionDesaparecer * 0.3f;
        
        while (tiempo < duracionAnticipacion)
        {
            float progreso = tiempo / duracionAnticipacion;
            float progresoSuave = progreso * progreso; // Ease In
            transform.localScale = Vector3.Lerp(escalaOriginal, escalaMaxima, progresoSuave);
            
            tiempo += Time.deltaTime;
            yield return null;
        }

        // 2. Fase 2: Encoger hasta desaparecer
        tiempo = 0;
        float duracionEncoger = duracionDesaparecer * 0.7f;
        
        while (tiempo < duracionEncoger)
        {
            float progreso = tiempo / duracionEncoger;
            float progresoSuave = 1 - Mathf.Pow(1 - progreso, 3); // Ease Out Cubic
            transform.localScale = Vector3.Lerp(escalaMaxima, Vector3.zero, progresoSuave);
            
            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar que quede en cero antes de desactivar
        transform.localScale = Vector3.zero;
        estaAnimando = false;
        
        // Desactivar el objeto
        gameObject.SetActive(false);
        
        // Ejecutar callback si existe
        alTerminar?.Invoke();
    }

    /// <summary>
    /// Método para animar manualmente (desde otro script)
    /// </summary>
    public void AnimarAparecerManual()
    {
        if (estaAnimando)
        {
            StopAllCoroutines();
            estaAnimando = false;
        }
        
        StartCoroutine(AnimarAparecer());
    }

    /// <summary>
    /// Cambiar la escala original en tiempo de ejecución
    /// </summary>
    public void ActualizarEscalaOriginal(Vector3 nuevaEscala)
    {
        escalaOriginal = nuevaEscala;
    }
}
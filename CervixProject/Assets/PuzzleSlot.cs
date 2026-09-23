using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PuzzleSlot : MonoBehaviour
{
    public GameObject targetPiece;
    public float snapDistance = 0.15f;
    public AudioClip snapSound;
    public AudioClip locucionClip;
    public AudioClip closingSound;
    public float locucionDelay = 0.5f;
    public GameObject slotFantasma;
    public int pieceNumber; 

    public TriviaManager triviaManager;

    public XRGrabInteractable[] grabInteractables;

    private AudioSource audioSource;
    private bool isCompleted = false;
    private Rigidbody pieceRb;
    private XRGrabInteractable pieceGrab;
    public GameObject infoPanel;
    public float infoPanelDuration = 4f;
    public float fadeDuration = 1f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public float maxDistance = 2f;
    public float rotationSnapDistance = 0.5f;
    public float rotationSpeed = 5f;
    private bool isHovering = true;

    private CanvasGroup infoPanelCanvasGroup;

    // ============================================
    // NUEVAS VARIABLES AÑADIDAS
    // ============================================
    [Header("Respuesta de Info Panel")]
    [Tooltip("Panel que se mostrará cuando showInfoPanelResponse sea true")]
    public GameObject InfoPanelResponse;

    void Start()
    {
        if (infoPanel != null)
            infoPanelCanvasGroup = infoPanel.GetComponent<CanvasGroup>();
        
        audioSource = gameObject.AddComponent<AudioSource>();
        pieceRb = targetPiece.GetComponent<Rigidbody>();
        pieceGrab = targetPiece.GetComponentInChildren<XRGrabInteractable>();

        if (slotFantasma != null)
            slotFantasma.GetComponentInChildren<MeshRenderer>().enabled = false;

        pieceGrab.selectEntered.AddListener(OnPieceGrabbed);
        pieceGrab.selectExited.AddListener(OnPieceReleased);

        initialPosition = targetPiece.transform.position;
        initialRotation = targetPiece.transform.rotation;
    }

    void Update()
    {
        // ============================================
        // VERIFICACIÓN DEL PANEL DE RESPUESTA
        // ============================================
        // Solo mostramos el panel si:
        // 2. El pieceNumber del slot coincide con el de la pieza (targetPiece)
        // 3. El panel aún no se ha mostrado (para no repetir)
        if (triviaManager.answered)
        {
            int pieceNumberDeLaPieza = targetPiece.GetComponent<PuzzlePiece>().numPiece;

            if (pieceNumberDeLaPieza == pieceNumber)
            {
                if (InfoPanelResponse != null)
                {
                    MostrarPanelRespuesta();

                    // Opcional: iniciar un temporizador para ocultarlo después de un tiempo
                    // StartCoroutine(OCultarPanelRespuestaDespuesDeTiempo());
                }
            }
            else
            {
                Debug.Log($"PuzzleSlot {pieceNumber}: El pieceNumber de la pieza ({pieceNumberDeLaPieza}) no coincide con el del slot ({pieceNumber}). No se muestra el panel.");
            }
        }

        // ============================================
        // LÓGICA ORIGINAL DEL UPDATE
        // ============================================
        if (isCompleted) return;

        if (pieceGrab.isSelected)
        {
            triviaManager.answered = false; // Resetear la respuesta cuando la pieza es agarrada
            float dist = Vector3.Distance(targetPiece.transform.position, transform.position);
            if (dist < rotationSnapDistance)
            {
                Debug.Log("Distancia: " + dist + " rotando...");
                targetPiece.transform.rotation = Quaternion.Slerp(
                    targetPiece.transform.rotation,
                    transform.rotation,
                    Time.deltaTime * rotationSpeed
                );
                if (isHovering && closingSound != null)
                {
                    audioSource.PlayOneShot(closingSound);
                    isHovering = false;
                }
            }
        }

        if (pieceGrab.isSelected) return;

        if (!pieceGrab.isSelected)
        {
            float distFromOrigin = Vector3.Distance(targetPiece.transform.position, initialPosition);
            if (distFromOrigin > maxDistance)
            {
                targetPiece.transform.position = initialPosition;
                targetPiece.transform.rotation = initialRotation;
                pieceRb.linearVelocity = Vector3.zero;
                pieceRb.angularVelocity = Vector3.zero;
            }
            isHovering = true;
        }

        Vector3 pieceCenter = targetPiece.GetComponentInChildren<MeshRenderer>().bounds.center;
        Vector3 slotCenter = GetComponentInChildren<MeshRenderer>().bounds.center;
        float distance = Vector3.Distance(pieceCenter, slotCenter);
        if (distance < snapDistance)
        {
            pieceRb.linearVelocity = Vector3.zero;
            pieceRb.angularVelocity = Vector3.zero;
            pieceRb.isKinematic = true;

            targetPiece.transform.position = transform.position;
            targetPiece.transform.rotation = transform.rotation;

            pieceGrab.interactionLayers = 0;
            pieceGrab.enabled = false;
            
            triviaManager.StartTrivia(
                targetPiece.GetComponent<PuzzlePiece>().correctAnswerValue, 
                targetPiece.GetComponent<PuzzlePiece>().correctClip, 
                targetPiece.GetComponent<PuzzlePiece>().incorrectClip,
                targetPiece.GetComponent<PuzzlePiece>().answerOptions,
                targetPiece.GetComponent<PuzzlePiece>().numPiece
            );

            foreach (var grabInteractable in grabInteractables)
            {
                grabInteractable.enabled = false;
            }

            if (snapSound != null)
                audioSource.PlayOneShot(snapSound);

            isCompleted = true;
            PuzzleManager.Instance.PiezaColocada();

            GetComponentInChildren<MeshRenderer>().enabled = false;

            if (infoPanel != null)
                PuzzleManager.Instance.ShowPanel(infoPanel, infoPanelCanvasGroup, audioSource, locucionClip, locucionDelay, infoPanelDuration, fadeDuration);

            Debug.Log("¡Pieza colocada!");
        }
    }

    /// <summary>
    /// Método alternativo que activa directamente el panel (por si lo prefieres)
    /// </summary>
    public void MostrarPanelRespuesta()
    {
        if (InfoPanelResponse != null)
            InfoPanelResponse.SetActive(true);
    }

    // ============================================

    void OnPieceGrabbed(SelectEnterEventArgs args)
    {
        if (slotFantasma != null)
            slotFantasma.GetComponentInChildren<MeshRenderer>().enabled = true;
    }

    void OnPieceReleased(SelectExitEventArgs args)
    {
        if (!isCompleted && slotFantasma != null)
            slotFantasma.GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
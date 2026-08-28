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

    public TriviaManager triviaManager; // Referencia al TriviaManager

    public XRGrabInteractable[] grabInteractables; // Array para almacenar los XRGrabInteractables de las piezas

    private AudioSource audioSource;
    private bool isCompleted = false;
    private Rigidbody pieceRb;
    private XRGrabInteractable pieceGrab;
    public GameObject infoPanel;
    public float infoPanelDuration = 4f;
    public float fadeDuration = 1f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public float maxDistance = 2f; // distancia m�xima antes de regresar
    public float rotationSnapDistance = 0.5f; // distancia para empezar a rotar
    public float rotationSpeed = 5f; // velocidad de rotaci�n
    private bool isHovering = true;


    private CanvasGroup infoPanelCanvasGroup;

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
        if (isCompleted) return;

        // Rotaci�n suave cuando est� cerca del slot
        if (pieceGrab.isSelected)
        {
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

        // No hacer snap si el usuario la est� agarrando
        if (pieceGrab.isSelected) return;

        // Regresar pieza si se fue muy lejos
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
            // Primero desactivar f�sica
            pieceRb.linearVelocity = Vector3.zero;
            pieceRb.angularVelocity = Vector3.zero;
            pieceRb.isKinematic = true;

            // Luego mover a posici�n exacta
            targetPiece.transform.position = transform.position;
            targetPiece.transform.rotation = transform.rotation;

            // Desactivar grab
            pieceGrab.interactionLayers = 0;
            pieceGrab.enabled = false;
            triviaManager.StartTrivia(
                targetPiece.GetComponent<PuzzlePiece>().correctAnswerValue, 
                targetPiece.GetComponent<PuzzlePiece>().correctClip, 
                targetPiece.GetComponent<PuzzlePiece>().incorrectClip);

            // Desactivar los XRGrabInteractables de las piezas
            foreach (var grabInteractable in grabInteractables)
            {
                grabInteractable.enabled = false;
            }

            // Sonido
            if (snapSound != null)
                audioSource.PlayOneShot(snapSound);

            isCompleted = true;
            PuzzleManager.Instance.PiezaColocada();


            // Ocultar el slot fantasma
            GetComponentInChildren<MeshRenderer>().enabled = false;

            if (infoPanel != null)
                PuzzleManager.Instance.ShowPanel(infoPanel, infoPanelCanvasGroup, audioSource, locucionClip, locucionDelay, infoPanelDuration, fadeDuration);

            Debug.Log("�Pieza colocada!");
        }
    }


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
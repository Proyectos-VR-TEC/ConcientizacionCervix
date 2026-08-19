using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ActivarZonas : MonoBehaviour
{
    public ZonaDecision zonaSi;
    public ZonaDecision zonaNo;
    public PanelProteccion panel;
    public PanelCalendario panelCalendario;
    public GameObject burbuja;
    public PanelFolder panelFolder;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grabInteractable.selectEntered.AddListener(AlAgarrar);
        grabInteractable.selectExited.AddListener(AlSoltar);

        // Guardar posición inicial
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
    }
    void AlAgarrar(SelectEnterEventArgs args)
    {
        zonaSi.ResetZona();
        zonaNo.ResetZona();
        if (rb != null) rb.isKinematic = false;
        if (panel != null) panel.AlAgarrarCondon();
        if (panelCalendario != null) panelCalendario.AlAgarrarCalendario();
        if (panelFolder != null) panelFolder.AlAgarrarFolder();  // ← agregá acá
        zonaSi.habilitado = true;
        zonaNo.habilitado = true;
        zonaSi.SetVisible(true);
        zonaNo.SetVisible(true);
        if (burbuja != null) burbuja.SetActive(false);
    }

    void AlSoltar(SelectExitEventArgs args)
    {
        float distSi = Vector3.Distance(zonaSi.transform.position, transform.position);
        float distNo = Vector3.Distance(zonaNo.transform.position, transform.position);
        float minDist = Mathf.Min(distSi, distNo);
        if (minDist < zonaSi.radioDeteccion)
        {
            if (distSi < distNo)
            {
                zonaSi.Colocar(transform);
                zonaNo.ResetZona();
                if (panel != null) panel.MostrarConsecuenciaSi();
                if (panelCalendario != null) panelCalendario.MostrarConsecuenciaSi();
                if (panelFolder != null) panelFolder.MostrarConsecuenciaSi();  // ← agregá acá
            }
            else
            {
                zonaNo.Colocar(transform);
                zonaSi.ResetZona();
                if (panel != null) panel.MostrarConsecuenciaNo();
                if (panelCalendario != null) panelCalendario.MostrarConsecuenciaNegativa();
                if (panelFolder != null) panelFolder.MostrarConsecuenciaNegativa();  // ← agregá acá
            }
        }
        else
        {
            zonaSi.ResetZona();
            zonaNo.ResetZona();
            if (rb != null) rb.isKinematic = true;
            transform.position = posicionOriginal;
            transform.rotation = rotacionOriginal;
            if (burbuja != null) burbuja.SetActive(true);
        }
    }
}
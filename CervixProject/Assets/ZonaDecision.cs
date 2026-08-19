using UnityEngine;

public class ZonaDecision : MonoBehaviour
{
    public string tagObjeto = "Condon";
    public bool esZonaSi = true;
    public float radioDeteccion = 1.2f;
    [HideInInspector] public bool condonAqui = false;
    [HideInInspector] public bool colocado = false;
    [HideInInspector] public bool habilitado = false;

    private Renderer rend;
    private Canvas canvas;
    public Color colorHover = new Color(0.72f, 0.53f, 0.96f);
    private Color colorOriginal;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        canvas = GetComponentInChildren<Canvas>();
        if (rend != null) colorOriginal = new Color(0.5f, 0.5f, 0.5f); 
        SetVisible(false);
    }

    void Update()
    {
        if (!habilitado || colocado) return;
        GameObject obj = GameObject.FindWithTag(tagObjeto);
        if (obj == null) return;

        float distancia = Vector3.Distance(transform.position, obj.transform.position);
        bool cerca = distancia < radioDeteccion;

        condonAqui = cerca;
        // Solo cambia color, no visibilidad
        if (rend != null)
            rend.material.SetColor("_BaseColor", cerca ? colorHover : colorOriginal);
    }

    public void SetVisible(bool visible) // ← ahora es público
    {
        if (rend != null) rend.enabled = visible;
        if (canvas != null) canvas.enabled = visible;
    }

    public void Colocar(Transform obj)
    {
        colocado = true;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.isKinematic = true; }
        obj.position = transform.position + Vector3.up * 0.15f;
        obj.rotation = Quaternion.identity;
        SetVisible(true);
        if (esZonaSi) Debug.Log("Eligió SÍ");
        else Debug.Log("Eligió NO");
    }

    public void ResetZona()
    {
        habilitado = false;
        colocado = false;
        condonAqui = false;
        SetVisible(false);
        if (rend != null) rend.material.SetColor("_BaseColor", colorOriginal);
    }

}
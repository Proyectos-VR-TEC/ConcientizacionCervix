using UnityEngine;
using UnityEngine.UI;
public class Diana : MonoBehaviour
{
    [Header("Configuración")]
    public bool esDianaSi = true;          // true = SÍ, false = NO
    public string tagBala = "Bala";

    [Header("Referencias")]
    public PanelDecisionBase panel;        // el panel que esta diana controla

    [Header("Feedback (opcional)")]
    public Renderer rend;
    public Color colorImpacto = Color.green;
    private Color colorOriginal;

    private bool yaDisparada = false;

    void Awake()
    {
        if (rend != null) colorOriginal = rend.material.color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (yaDisparada) return;
        if (!other.CompareTag(tagBala)) return;

        yaDisparada = true;

        // Feedback visual
        if (rend != null) rend.material.color = colorImpacto;

        // Destruir la bala
        Destroy(other.gameObject);

        // Delegar la decisión al panel
        if (panel != null)
        {
            if (esDianaSi) panel.MostrarConsecuenciaSi();
            else           panel.MostrarConsecuenciaNo();
        }
        else
        {
            Debug.LogWarning($"Diana {name} no tiene panel asignado");
        }
    }

    public void ResetDiana()
    {
        yaDisparada = false;
        if (rend != null) rend.material.color = colorOriginal;
    }
}
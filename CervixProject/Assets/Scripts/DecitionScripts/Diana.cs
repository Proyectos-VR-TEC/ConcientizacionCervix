using UnityEngine;
using UnityEngine.Events;

public class Diana : MonoBehaviour
{
    [Header("Configuración")]
    public bool esDianaSi = true;
    public string tagBala = "Bala";

    [Header("Referencias")]
    public PanelDecisionBase panel;

    [Header("Feedback (opcional)")]
    
    public UnityEvent onCollisionEvent; // Evento que se dispara al colisionar con la bala

    private bool yaDisparada = false;
    private bool bloqueada = false;   // 🆕

    void OnTriggerEnter(Collider other)
    {
        if (yaDisparada) return;
        if (bloqueada) return;                       // 🆕
        if (!other.CompareTag(tagBala)) return;

        yaDisparada = true;

        onCollisionEvent?.Invoke();

        Destroy(other.gameObject);

        // 🆕 Avisar al manager para que bloquee las otras dianas del grupo
        DecisionManager.Instance?.BloquearOtrasDianas(this);

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

    // 🆕 Método para bloquear sin desactivar el GameObject
    public void Bloquear()
    {
        bloqueada = true;
    }

    public void ResetDiana()
    {
        yaDisparada = false;
        bloqueada = false;             
    }


}
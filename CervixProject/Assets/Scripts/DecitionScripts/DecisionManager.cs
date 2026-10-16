using UnityEngine;
using UnityEngine.Events;

public class DecisionManager : MonoBehaviour
{
    public static DecisionManager Instance { get; private set; }

    [Header("Paneles en orden (3 decisiones)")]
    public PanelDecisionBase[] paneles;

    [Header("Dianas en orden (2 por panel: SÍ y NO)")]
    public Diana[] dianasPanel0;   // dianas del panel 0
    public Diana[] dianasPanel1;   // dianas del panel 1
    public Diana[] dianasPanel2;   // dianas del panel 2

    [Header("Eventos al completar las 3 decisiones")]
    public UnityEvent alCompletarTodo;

    private int indiceActual = 0;
    private bool esperandoDecision = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Desactivar todo
        foreach (var p in paneles) if (p != null) p.gameObject.SetActive(false);
        DesactivarTodasLasDianas();

        // Arrancar la primera decisión
        ActivarDecision(0);
    }

    void ActivarDecision(int index)
    {
        if (index >= paneles.Length)
        {
            alCompletarTodo?.Invoke();
            Debug.Log("DecisionManager: Todas las decisiones completadas.");
            return;
        }

        indiceActual = index;
        Debug.Log($"DecisionManager: Activando decisión {index}.");
        esperandoDecision = true;

        var panel = paneles[index];
        panel.gameObject.SetActive(true);

        // 🆕 Asignar el callback ANTES de arrancar
        panel.AlTerminarConsecuencia = () => NotificarDecisionCompletada();

        // 🆕 Reset por si se reutiliza
        panel.ResetPanel();

        panel.AlAgarrar();
        // ActivarDianas(index);
    }

    public void NotificarDecisionCompletada()
    {
        if (!esperandoDecision) return;
        esperandoDecision = false;

        paneles[indiceActual].gameObject.SetActive(false);
        DesactivarDianas(indiceActual);

        ActivarDecision(indiceActual + 1);
    }

    void ActivarDianas(int index)
    {
        Diana[] grupo = GetDianasDePanel(index);
        foreach (var d in grupo) if (d != null) d.gameObject.SetActive(true);
    }

    void DesactivarDianas(int index)
    {
        Diana[] grupo = GetDianasDePanel(index);
        foreach (var d in grupo) if (d != null) { d.ResetDiana(); d.gameObject.SetActive(false); }
    }

    void DesactivarTodasLasDianas()
    {
        for (int i = 0; i < paneles.Length; i++) DesactivarDianas(i);
    }

    Diana[] GetDianasDePanel(int index)
    {
        return index switch {
            0 => dianasPanel0,
            1 => dianasPanel1,
            2 => dianasPanel2,
            _ => new Diana[0]
        };
    }

    public void BloquearOtrasDianas(Diana origen)
{
    for (int i = 0; i < paneles.Length; i++)
    {
        Diana[] grupo = GetDianasDePanel(i);
        if (System.Array.IndexOf(grupo, origen) < 0) continue;

        foreach (var d in grupo)
        {
            if (d == null || d == origen) continue;
            d.Bloquear();
        }
        return; // ya encontramos su grupo
    }
}
}


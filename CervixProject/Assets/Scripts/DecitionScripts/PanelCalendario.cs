using UnityEngine;

public class PanelCalendario : PanelDecisionBase
{
    [Header("Estado global")]
    public GameState state;

    public override void MostrarConsecuenciaSi()
    {
        base.MostrarConsecuenciaSi();
        if (state != null) state.eligioSiCalendario = true;
        Debug.Log("Calendario: eligió SÍ");
    }

    public override void MostrarConsecuenciaNo()
    {
        base.MostrarConsecuenciaNo();
        if (state != null) state.eligioSiCalendario = false;
        Debug.Log("Calendario: eligió NO");
    }
}
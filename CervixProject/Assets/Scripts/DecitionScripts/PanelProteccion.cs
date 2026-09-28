using UnityEngine;

public class PanelProteccion : PanelDecisionBase
{
    [Header("Estado global")]
    public GameState state;

    public override void MostrarConsecuenciaSi()
    {
        base.MostrarConsecuenciaSi();
        if (state != null) state.eligioSiProteccion = true;
        Debug.Log("Protección: eligió SÍ");
    }

    public override void MostrarConsecuenciaNo()
    {
        base.MostrarConsecuenciaNo();
        if (state != null) state.eligioSiProteccion = false;
        Debug.Log("Protección: eligió NO");
    }
}
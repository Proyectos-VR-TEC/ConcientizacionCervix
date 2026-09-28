using UnityEngine;

public class PanelFolder : PanelDecisionBase
{
    [Header("Estado global")]
    public GameState state;

    public override void MostrarConsecuenciaSi()
    {
        base.MostrarConsecuenciaSi();
        if (state != null) state.eligioSiFolder = true;
        Debug.Log("Folder: eligió SÍ");
    }

    public override void MostrarConsecuenciaNo()
    {
        base.MostrarConsecuenciaNo();
        if (state != null) state.eligioSiFolder = false;
        Debug.Log("Folder: eligió NO");
    }
}
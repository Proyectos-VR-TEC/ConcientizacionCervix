using UnityEngine;

[CreateAssetMenu(menuName = "Juego/GameState", fileName = "GameState")]
public class GameState : ScriptableObject
{
    public bool eligioSiProteccion;
    public bool eligioSiCalendario;
    public bool eligioSiFolder;

    public void Reset()
    {
        eligioSiProteccion = false;
        eligioSiCalendario = false;
        eligioSiFolder = false;
    }
}
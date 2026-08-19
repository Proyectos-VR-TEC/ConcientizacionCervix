using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PieceHover : MonoBehaviour
{
    public Color hoverColor = Color.cyan;
    private Color originalColor;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        originalColor = meshRenderer.material.color;
    }

    public void OnHoverEnter()
    {
        meshRenderer.material.color = hoverColor;
    }

    public void OnHoverExit()
    {
        meshRenderer.material.color = originalColor;
    }
}
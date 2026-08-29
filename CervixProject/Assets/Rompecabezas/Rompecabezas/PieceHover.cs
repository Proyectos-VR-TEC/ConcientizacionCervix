using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PieceHover : MonoBehaviour
{
    public Color hoverColor = Color.cyan;
    public AudioSource hoverSound;
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
        hoverSound.Play();

    }

    public void OnHoverExit()
    {
        meshRenderer.material.color = originalColor;
    }
}
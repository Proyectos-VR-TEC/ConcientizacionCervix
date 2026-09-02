using System;
using System.Collections;
using UnityEngine;

public class ActivarComponente : MonoBehaviour
{
    public GameObject componente; // Arrastra cualquier componente
    public GameObject[] componentes; // Arrastra aquí el objeto "Button" de la jerarquía 
    public Animator TamiAnimator; // Arrastra el Animator del corazón
    public string firstAnimation; // Nombre de la animación a reproducir
 
    public bool activarAlIniciar; // Si quieres que se active al iniciar

    void Start()
    {
        if (activarAlIniciar)
        {
            AnimarCordi();
        }
    }

    public void AnimarCordi()
    {
        if (TamiAnimator != null)
        {
            TamiAnimator.Play(firstAnimation);
            Debug.Log("Activando animación de corazón");
        }
    }

    public void Activar()
    {
        componente.SetActive(true);
    }

    public void Desactivar()
    {
        componente.SetActive(false);
    }


    public void DesactivarConDelay(float delay)
    {
        StartCoroutine(DesactivarDelay(delay));
    }

    IEnumerator DesactivarDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (componente != null)
            componente.SetActive(false);
    }


    public void ActivarObjetoConDelay(float delay)
    {
        StartCoroutine(ActivarConDelay(delay));
    }

    IEnumerator ActivarConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (componente != null)
            componente.SetActive(true);
    }

    public void ActivarObjetosConDelay(float delay)
    {
        StartCoroutine(ActivarObsConDelay(delay));
    }

    IEnumerator ActivarObsConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (GameObject obj in componentes)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void DesactivarObjetosConDelay(float delay)
    {
        StartCoroutine(DesactivarObsConDelay(delay));
    }

    IEnumerator DesactivarObsConDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (GameObject obj in componentes)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
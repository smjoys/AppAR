using UnityEngine;
using System.Collections;
public class CajaController : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animator;
    public GameObject boton3D;
    public GameObject modeloOculto;

    private bool abierta = false;
    private float duracionAnimacionAbrir = 1.15f;

    void Start()
    {

        if (modeloOculto != null) modeloOculto.SetActive(false);
        if (boton3D != null) boton3D.SetActive(true);

        animator.Play("Reposo", 0, 0f);
    }

    public void AbrirCaja()
    {
        if (abierta) return;

        abierta = true;

        animator.SetTrigger("Abrir");

        if (boton3D != null) boton3D.SetActive(false);
        StartCoroutine(EsperarYMostrarModelo());

    }

    private IEnumerator EsperarYMostrarModelo()
    {
        yield return new WaitForSeconds(duracionAnimacionAbrir);
        if (modeloOculto != null)
        {
            modeloOculto.SetActive(true);
        }
    }

}

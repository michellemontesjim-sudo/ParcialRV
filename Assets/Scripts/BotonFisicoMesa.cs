using UnityEngine;
using UnityEngine.Events;

public class BotonFisicoMesa : MonoBehaviour
{
    [Tooltip("Conecta aquí MesaQuimica -> AvanzarDialogo")]
    public UnityEvent alPresionarBoton;

    private float ultimoToque = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // Evita que se presione dos veces seguidas por accidente (cooldown de 0.5s)
        if (Time.time - ultimoToque < 0.5f) return;

        // Detecta si lo tocó la mano/control del jugador
        if (other.CompareTag("Player") || other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>() != null)
        {
            ultimoToque = Time.time;
            alPresionarBoton.Invoke();
        }
    }
}
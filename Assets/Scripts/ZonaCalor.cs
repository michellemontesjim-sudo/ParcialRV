using UnityEngine;

public class ZonaCalor : MonoBehaviour
{
    [Tooltip("Cuántos grados sube por segundo")]
    public float velocidadCalentamiento = 25f;
    public ParticleSystem fuegoOChispas;
    public AudioSource sonidoFuego;

    private void OnTriggerStay(Collider other)
    {
        // Detecta si lo que está encima es un ingrediente
        IngredienteQuimico ingrediente = other.GetComponent<IngredienteQuimico>();

        if (ingrediente != null)
        {
            ingrediente.Calentar(velocidadCalentamiento);
        }
    }
}
using UnityEngine;

public class ZonaCalor : MonoBehaviour
{
    [Header("Estado de la Estufa")]
    public bool estaEncendida = false;
    public float velocidadCalentamiento = 25f;

    [Header("Efectos Visuales y Sonoros (Opcional)")]
    public ParticleSystem fuegoOChispas;
    public AudioSource sonidoFuego;

    void Start()
    {
        if (estaEncendida) Encender();
        else Apagar();
    }

    public void Encender()
    {
        estaEncendida = true;
        
        if (fuegoOChispas != null && !fuegoOChispas.isPlaying) fuegoOChispas.Play();
        if (sonidoFuego != null && !sonidoFuego.isPlaying) sonidoFuego.Play();
    }

    public void Apagar()
    {
        estaEncendida = false;
        
        if (fuegoOChispas != null) fuegoOChispas.Stop();
        if (sonidoFuego != null) sonidoFuego.Stop();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!estaEncendida) return;

        // Busca el script tanto en el collider tocado como en su objeto padre
        IngredienteQuimico ingrediente = other.GetComponent<IngredienteQuimico>()
                                      ?? other.GetComponentInParent<IngredienteQuimico>();

        if (ingrediente != null)
        {
            ingrediente.Calentar(velocidadCalentamiento);
        }
    }
}
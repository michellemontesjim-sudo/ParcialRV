using UnityEngine;

public class IngredienteQuimico : MonoBehaviour
{
    [Tooltip("Nombre base del ingrediente. Ej: 'Agua', 'Sodio'")]
    public string nombreIngrediente;

    [Header("Temperatura")]
    public float temperaturaActual = 20f;
    public float temperaturaObjetivo = 80f;
    public bool estaCaliente = false;

    [Header("Feedback Visual y Sonoro")]
    public Renderer meshRenderer;
    public Color colorCaliente = Color.red;
    public AudioSource audioSource;
    public AudioClip sonidoListo;

    private Color colorOriginal;

    void Start()
    {
        if (meshRenderer != null)
        {
            colorOriginal = meshRenderer.material.color;
        }

        
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1.0f;
            audioSource.playOnAwake = false;
        }
    }

    public void Calentar(float gradosPorSegundo)
    {
        
        if (estaCaliente) return;

        temperaturaActual += gradosPorSegundo * Time.deltaTime;

        if (meshRenderer != null)
        {
            float progreso = Mathf.InverseLerp(20f, temperaturaObjetivo, temperaturaActual);
            meshRenderer.material.color = Color.Lerp(colorOriginal, colorCaliente, progreso);
        }

        if (temperaturaActual >= temperaturaObjetivo)
        {
            temperaturaActual = temperaturaObjetivo;
            estaCaliente = true;

            // Reproduce el sonido una sola vez al alcanzar la temperatura
            if (audioSource != null && sonidoListo != null)
            {
                audioSource.PlayOneShot(sonidoListo);
            }

            Debug.Log($"{nombreIngrediente} ¡está listo y caliente!");
        }
    }
}
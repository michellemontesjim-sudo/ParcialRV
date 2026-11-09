using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DispensadorIngredientes : MonoBehaviour
{
    [Header("Configuración del Lote")]
    [Tooltip("El prefab del ingrediente (debe tener XR Grab Interactable y tu script IngredienteQuimico)")]
    public GameObject prefabIngrediente;

    [Tooltip("El punto exacto donde aparecerán (puedes arrastrar este mismo objeto)")]
    public Transform puntoSpawn;

    void Start()
    {
        // Generamos el primer ingrediente al iniciar el nivel
        if (prefabIngrediente != null && puntoSpawn != null)
        {
            GenerarNuevo();
        }
        else
        {
            Debug.LogWarning("Falta asignar el prefab o el punto de spawn en el dispensador.");
        }
    }

    void GenerarNuevo()
    {
        // 1. Creamos una copia del prefab en la posición del dispensador
        GameObject nuevoIngrediente = Instantiate(prefabIngrediente, puntoSpawn.position, puntoSpawn.rotation);

        // 2. Buscamos su componente de agarre de Realidad Virtual
        XRGrabInteractable agarre = nuevoIngrediente.GetComponent<XRGrabInteractable>();

        if (agarre != null)
        {
            // 3. Vinculamos el evento por código: "Cuando te agarren, avísame"
            agarre.selectEntered.AddListener(AlSerAgarrado);
        }
    }

    void AlSerAgarrado(SelectEnterEventArgs args)
    {
        // 1. Buscamos el frasco específico que el jugador acaba de agarrar
        XRGrabInteractable agarre = args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        // 2. Le quitamos el evento. 
        // ¿Por qué? Para que si el jugador lo suelta en el piso y lo vuelve a agarrar, no genere OTRO clon extra en la mesa.
        agarre.selectEntered.RemoveListener(AlSerAgarrado);

        // 3. Generamos el siguiente frasco de repuesto en el punto de inicio
        GenerarNuevo();
    }
}
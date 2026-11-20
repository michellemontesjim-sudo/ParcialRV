using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DispensadorIngredientes : MonoBehaviour
{
    [Header("Configuración del Lote")]
    [Tooltip("El prefab del ingrediente (debe tener XR Grab Interactable y tu script IngredienteQuimico)")]
    public GameObject prefabIngrediente;

    [Tooltip("El punto exacto donde aparecerán (puedes arrastrar este mismo objeto)")]
    public Transform puntoSpawn;

    [Tooltip("Segundos que tardará en reaparecer un nuevo ingrediente tras agarrar el anterior")]
    public float tiempoDeEspera = 2.0f;

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
        XRGrabInteractable agarre = args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (agarre != null)
        {
            agarre.selectEntered.RemoveListener(AlSerAgarrado);
        }

        // En lugar de instanciar de inmediato, iniciamos la cuenta regresiva
        StartCoroutine(EsperarYGenerar());

    }

    IEnumerator EsperarYGenerar()
    {
        // Pausa la ejecución durante los segundos configurados en el Inspector
        yield return new WaitForSeconds(tiempoDeEspera);

        GenerarNuevo();
    }
}
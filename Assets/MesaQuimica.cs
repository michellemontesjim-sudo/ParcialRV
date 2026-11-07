using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

[System.Serializable]
public class LineaDialogo
{
    [TextArea(2, 4)]
    public string texto;
    public AudioClip audioVoz;
}

[System.Serializable]
public class RecetaQuimica
{
    public string nombreReceta;
    public List<string> ingredientes;
}

[System.Serializable]
public class NivelQuimica
{
    [Tooltip("Los diálogos que el personaje dirá ANTES de empezar las recetas de este nivel")]
    public List<LineaDialogo> dialogosCinematica;
    [Tooltip("Las recetas que se deben completar en este nivel")]
    public List<RecetaQuimica> recetasDelNivel;
}

public class MesaQuimica : MonoBehaviour
{
    [Header("UI - Paneles")]
    public GameObject panelCinematica;
    public GameObject panelReceta;

    [Header("UI - Textos")]
    public TextMeshProUGUI textoDialogoCinematica;
    public TextMeshProUGUI textoInstruccionesReceta;
    public TextMeshProUGUI textoPuntaje;

    [Header("Configuración de Cinemática")]
    public float velocidadEscritura = 0.05f;
    public Animator animMascota;
    public AudioSource audioSourceMascota;

    [Header("Progresión del Juego")]
    public List<NivelQuimica> niveles = new List<NivelQuimica>();
    private int indiceNivelActual = 0;
    private int indiceRecetaActual = 0;
    private List<string> ingredientesFaltantes = new List<string>();

    [Header("Efectos de Juego")]
    public int puntajeJugador = 0;
    public AudioSource sonidoExito;
    public ParticleSystem humoQuimico;

    // Variables internas de control
    private bool enCinematica = false;
    private bool escribiendoDialogo = false;
    private int indiceDialogoActual = 0;
    private Coroutine rutinaEscribir;

    void Start()
    {
        ActualizarUIPuntaje();
        if (niveles.Count > 0)
        {
            IniciarCinematicaNivel();
        }
    }

    // --- SISTEMA DE CINEMÁTICA ---

    void IniciarCinematicaNivel()
    {
        enCinematica = true;

        // Encendemos la interfaz de novela visual y apagamos la de recetas
        panelCinematica.SetActive(true);
        panelReceta.SetActive(false);

        indiceDialogoActual = 0;
        MostrarLineaDialogo();
    }

    // Conecta tu botón (físico o UI) a esta función para pasar el diálogo
    public void AvanzarDialogo()
    {
        if (!enCinematica) return;

        if (escribiendoDialogo)
        {
            // Si el jugador presiona mientras escribe, se autocompleta de golpe
            if (rutinaEscribir != null) StopCoroutine(rutinaEscribir);

            textoDialogoCinematica.text = niveles[indiceNivelActual].dialogosCinematica[indiceDialogoActual].texto;
            escribiendoDialogo = false;

            if (animMascota != null) animMascota.SetBool("Hablando", false);
            if (audioSourceMascota != null) audioSourceMascota.Stop();
        }
        else
        {
            // Pasamos a la siguiente línea de diálogo
            indiceDialogoActual++;

            if (indiceDialogoActual < niveles[indiceNivelActual].dialogosCinematica.Count)
            {
                MostrarLineaDialogo();
            }
            else
            {
                // Si ya no hay más diálogos, termina la cinemática
                TerminarCinematica();
            }
        }
    }

    void MostrarLineaDialogo()
    {
        if (rutinaEscribir != null) StopCoroutine(rutinaEscribir);
        rutinaEscribir = StartCoroutine(EscribirEfecto(niveles[indiceNivelActual].dialogosCinematica[indiceDialogoActual]));
    }

    IEnumerator EscribirEfecto(LineaDialogo linea)
    {
        escribiendoDialogo = true;
        textoDialogoCinematica.text = "";

        if (audioSourceMascota != null && linea.audioVoz != null)
        {
            audioSourceMascota.clip = linea.audioVoz;
            audioSourceMascota.Play();
        }
        if (animMascota != null) animMascota.SetBool("Hablando", true);

        foreach (char letra in linea.texto.ToCharArray())
        {
            textoDialogoCinematica.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        if (animMascota != null) animMascota.SetBool("Hablando", false);
        escribiendoDialogo = false;
    }

    void TerminarCinematica()
    {
        enCinematica = false;

        // Apagamos la cinemática y mostramos la receta
        panelCinematica.SetActive(false);
        panelReceta.SetActive(true);

        indiceRecetaActual = 0;
        CargarReceta(indiceRecetaActual);
    }

    // --- SISTEMA DE QUÍMICA ---

    public void EvaluarIngrediente(SelectEnterEventArgs args)
    {
        // Si estamos en medio de un diálogo, el caldero no acepta ingredientes
        if (enCinematica || indiceNivelActual >= niveles.Count) return;

        GameObject objetoPuesto = args.interactableObject.transform.gameObject;
        IngredienteQuimico ingrediente = objetoPuesto.GetComponent<IngredienteQuimico>();

        if (ingrediente != null)
        {
            if (ingredientesFaltantes.Contains(ingrediente.nombreIngrediente))
            {
                ingredientesFaltantes.Remove(ingrediente.nombreIngrediente);
                Destroy(objetoPuesto, 0.1f);

                if (ingredientesFaltantes.Count == 0) RecetaCompletada();
                else ActualizarUIInstrucciones();
            }
        }
    }

    void RecetaCompletada()
    {
        puntajeJugador += 20;
        ActualizarUIPuntaje();

        if (sonidoExito != null) sonidoExito.Play();
        if (humoQuimico != null) humoQuimico.Play();

        indiceRecetaActual++;

        if (indiceRecetaActual < niveles[indiceNivelActual].recetasDelNivel.Count)
        {
            CargarReceta(indiceRecetaActual);
        }
        else
        {
            NivelCompletado();
        }
    }

    void NivelCompletado()
    {
        puntajeJugador += 50;
        ActualizarUIPuntaje();

        indiceNivelActual++;

        if (indiceNivelActual < niveles.Count)
        {
            // AL TERMINAR EL NIVEL: Se inicia automáticamente la cinemática del siguiente nivel
            IniciarCinematicaNivel();
        }
        else
        {
            panelReceta.SetActive(true);
            panelCinematica.SetActive(false);
            textoInstruccionesReceta.text = "<color=green>¡Felicidades!</color>\nHas completado todas las fórmulas del juego.";
        }
    }

    void CargarReceta(int indiceReceta)
    {
        RecetaQuimica receta = niveles[indiceNivelActual].recetasDelNivel[indiceReceta];
        ingredientesFaltantes = new List<string>(receta.ingredientes);
        ActualizarUIInstrucciones();
    }

    void ActualizarUIInstrucciones()
    {
        if (textoInstruccionesReceta == null) return;
        RecetaQuimica receta = niveles[indiceNivelActual].recetasDelNivel[indiceRecetaActual];

        string textoFinal = $"<b><color=#00FFFF>Fórmula {indiceRecetaActual + 1}: {receta.nombreReceta}</color></b>\n\n";
        textoFinal += "<color=yellow>Falta añadir:</color>\n";

        foreach (string ing in ingredientesFaltantes)
        {
            textoFinal += "- " + ing + "\n";
        }
        textoInstruccionesReceta.text = textoFinal;
    }

    void ActualizarUIPuntaje()
    {
        if (textoPuntaje != null) textoPuntaje.text = "Puntos: " + puntajeJugador;
    }
}
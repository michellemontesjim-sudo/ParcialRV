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
    [Header("Cinemática")]
    public List<LineaDialogo> dialogosCinematica;

    [Header("Tiempo (Opcional)")]
    [Tooltip("¿Este nivel tiene tiempo límite?")]
    public bool tieneTiempoLimite;
    [Tooltip("Tiempo en segundos para completar TODO el nivel")]
    public float tiempoEnSegundos;

    [Header("Recetas")]
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
    public TextMeshProUGUI textoTemporizador; // NUEVO: Texto para el reloj

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

    [Header("Penalizaciones")]
    public int puntosPorError = 5;
    public AudioSource sonidoError;
    private Coroutine rutinaError;

    // Variables internas de control
    private bool enCinematica = false;
    private bool escribiendoDialogo = false;
    private int indiceDialogoActual = 0;
    private Coroutine rutinaEscribir;

    // Control del temporizador
    private bool temporizadorActivo = false;
    private float tiempoRestante = 0f;

    void Start()
    {
        ActualizarUIPuntaje();
        if (textoTemporizador != null) textoTemporizador.gameObject.SetActive(false);

        if (niveles.Count > 0)
        {
            IniciarCinematicaNivel();
        }
    }

    // --- EL RELOJ DEL JUEGO ---
    void Update()
    {
        // El tiempo solo corre si está activo y no estamos en cinemática
        if (temporizadorActivo && !enCinematica)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarUITemporizador();

            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                temporizadorActivo = false;
                NivelFalladoPorTiempo(); // ¡Se acabó el tiempo!
            }
        }
    }

    void ActualizarUITemporizador()
    {
        if (textoTemporizador != null)
        {
            // Convertimos los segundos a formato Minutos:Segundos (Ej: 01:30)
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoTemporizador.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            // Si quedan 10 segundos o menos, el texto se pone rojo para dar tensión
            if (tiempoRestante <= 10f) textoTemporizador.color = Color.red;
            else textoTemporizador.color = Color.white;
        }
    }

    void NivelFalladoPorTiempo()
    {
        if (sonidoError != null) sonidoError.Play();

        puntajeJugador -= 15; // Penalización fuerte por fallar el nivel
        if (puntajeJugador < 0) puntajeJugador = 0;
        ActualizarUIPuntaje();

        if (rutinaError != null) StopCoroutine(rutinaError);
        StartCoroutine(MensajeFallaTiempo());
    }

    IEnumerator MensajeFallaTiempo()
    {
        textoInstruccionesReceta.text = "<color=red><b>¡TIEMPO AGOTADO!</b></color>\nLos químicos se han arruinado. Reiniciando el nivel...";
        yield return new WaitForSeconds(3f);

        // Volvemos a empezar el nivel actual
        IniciarCinematicaNivel();
    }


    // --- SISTEMA DE CINEMÁTICA ---
    void IniciarCinematicaNivel()
    {
        enCinematica = true;
        temporizadorActivo = false; // Pausamos el reloj durante la cinemática
        if (textoTemporizador != null) textoTemporizador.gameObject.SetActive(false);

        panelCinematica.SetActive(true);
        panelReceta.SetActive(false);
        indiceDialogoActual = 0;
        MostrarLineaDialogo();
    }

    public void AvanzarDialogo()
    {
        if (!enCinematica) return;

        if (escribiendoDialogo)
        {
            if (rutinaEscribir != null) StopCoroutine(rutinaEscribir);
            textoDialogoCinematica.text = niveles[indiceNivelActual].dialogosCinematica[indiceDialogoActual].texto;
            escribiendoDialogo = false;

            if (animMascota != null) animMascota.SetBool("Hablando", false);
            if (audioSourceMascota != null) audioSourceMascota.Stop();
        }
        else
        {
            indiceDialogoActual++;
            if (indiceDialogoActual < niveles[indiceNivelActual].dialogosCinematica.Count) MostrarLineaDialogo();
            else TerminarCinematica();
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
        panelCinematica.SetActive(false);
        panelReceta.SetActive(true);

        // Al terminar la cinemática, activamos el reloj si el nivel lo requiere
        NivelQuimica nivelAct = niveles[indiceNivelActual];
        if (nivelAct.tieneTiempoLimite)
        {
            tiempoRestante = nivelAct.tiempoEnSegundos;
            temporizadorActivo = true;
            if (textoTemporizador != null) textoTemporizador.gameObject.SetActive(true);
        }

        indiceRecetaActual = 0;
        CargarReceta(indiceRecetaActual);
    }

    // --- SISTEMA DE QUÍMICA ---
    public void EvaluarIngrediente(SelectEnterEventArgs args)
    {
        if (enCinematica || indiceNivelActual >= niveles.Count) return;

        GameObject objetoPuesto = args.interactableObject.transform.gameObject;
        IngredienteQuimico ingrediente = objetoPuesto.GetComponent<IngredienteQuimico>();

        if (ingrediente != null)
        {
            if (ingredientesFaltantes.Contains(ingrediente.nombreIngrediente))
            {
                if (rutinaError != null) StopCoroutine(rutinaError);

                ingredientesFaltantes.Remove(ingrediente.nombreIngrediente);
                Destroy(objetoPuesto, 0.1f);

                if (ingredientesFaltantes.Count == 0) RecetaCompletada();
                else ActualizarUIInstrucciones();
            }
            else
            {
                Destroy(objetoPuesto, 0.1f);
                PenalizarJugador(ingrediente.nombreIngrediente);
            }
        }
    }

    void PenalizarJugador(string nombreIncorrecto)
    {
        puntajeJugador -= puntosPorError;
        if (puntajeJugador < 0) puntajeJugador = 0;

        ActualizarUIPuntaje();
        if (sonidoError != null) sonidoError.Play();

        if (rutinaError != null) StopCoroutine(rutinaError);
        rutinaError = StartCoroutine(MostrarMensajeErrorTemporal(nombreIncorrecto));
    }

    IEnumerator MostrarMensajeErrorTemporal(string nombreIncorrecto)
    {
        ActualizarUIInstrucciones();
        string textoBase = textoInstruccionesReceta.text;
        textoInstruccionesReceta.text = $"<color=red><b>¡ERROR!</b></color>\nEl ingrediente <color=orange>{nombreIncorrecto}</color> no va aquí.\n\n" + textoBase;

        yield return new WaitForSeconds(2.5f);

        if (!enCinematica && ingredientesFaltantes.Count > 0) ActualizarUIInstrucciones();
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
        temporizadorActivo = false; // Detenemos el reloj si logró pasar
        if (textoTemporizador != null) textoTemporizador.gameObject.SetActive(false);

        puntajeJugador += 50;
        ActualizarUIPuntaje();

        indiceNivelActual++;

        if (indiceNivelActual < niveles.Count)
        {
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
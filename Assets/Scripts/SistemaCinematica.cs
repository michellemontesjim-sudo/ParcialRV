using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*// Estructura para cada línea de diálogo
[System.Serializable]
public class LineaDialogo
{
    [TextArea(2, 4)]
    public string texto;
    [Tooltip("Audio opcional que suena mientras dice esta línea")]
    public AudioClip audioVoz;
}

public class SistemaCinematica : MonoBehaviour
{
    [Header("Interfaz Visual")]
    [Tooltip("El panel que contiene el fondo blanco, el avatar y el texto")]
    public GameObject panelCinematica;
    public TextMeshProUGUI textoDialogo;

    [Header("Configuración de Diálogos")]
    public List<LineaDialogo> secuenciaDialogos = new List<LineaDialogo>();
    public float velocidadEscritura = 0.05f;

    [Header("Referencias 3D (Opcional)")]
    public Animator animMascota;
    public AudioSource audioSourceMascota;

    private int indiceActual = 0;
    private bool escribiendo = false;
    private Coroutine rutinaEscribir;

    void Start()
    {
        // Si hay diálogos en la lista, encendemos la cinemática al iniciar
        if (secuenciaDialogos.Count > 0)
        {
            IniciarCinematica();
        }
    }

    public void IniciarCinematica()
    {
        panelCinematica.SetActive(true);
        indiceActual = 0;
        MostrarLineaActual();
    }

    // Llama a esta función desde un botón para continuar leyendo
    public void AvanzarDialogo()
    {
        if (escribiendo)
        {
            // Si presionas el botón mientras escribe, se autocompleta el texto de golpe
            if (rutinaEscribir != null) StopCoroutine(rutinaEscribir);

            textoDialogo.text = secuenciaDialogos[indiceActual].texto;
            escribiendo = false;

            if (animMascota != null) animMascota.SetBool("Hablando", false);
            if (audioSourceMascota != null) audioSourceMascota.Stop();
        }
        else
        {
            // Si ya terminó de escribir, pasamos a la siguiente línea
            indiceActual++;
            if (indiceActual < secuenciaDialogos.Count)
            {
                MostrarLineaActual();
            }
            else
            {
                TerminarCinematica();
            }
        }
    }

    void MostrarLineaActual()
    {
        if (rutinaEscribir != null) StopCoroutine(rutinaEscribir);
        rutinaEscribir = StartCoroutine(EscribirEfecto(secuenciaDialogos[indiceActual]));
    }

    IEnumerator EscribirEfecto(LineaDialogo linea)
    {
        escribiendo = true;
        textoDialogo.text = "";

        // Reproducimos el audio y la animación
        if (audioSourceMascota != null && linea.audioVoz != null)
        {
            audioSourceMascota.clip = linea.audioVoz;
            audioSourceMascota.Play();
        }
        if (animMascota != null) animMascota.SetBool("Hablando", true);

        // Efecto máquina de escribir
        foreach (char letra in linea.texto.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        // Terminó de escribir
        if (animMascota != null) animMascota.SetBool("Hablando", false);
        escribiendo = false;
    }

    public void TerminarCinematica()
    {
        panelCinematica.SetActive(false);
        Debug.Log("Cinemática terminada. El juego normal puede comenzar.");
        // Aquí puedes llamar a una función para activar tus otras mecánicas de juego
    }
}*/
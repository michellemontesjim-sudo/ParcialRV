using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalVR : MonoBehaviour
{
    [Header("Escena Siguiente")]
    [Tooltip("Nombre exacto de la escena de la cinemática inicial")]
    public string escenaCinematica = "CinematicaIntro";

    [Header("Paneles UI")]
    public GameObject panelInstrucciones;

    [Header("Sonido de Botones")]
    public AudioSource audioUI;
    public AudioClip sonidoClick;

    void Start()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    private void ReproducirClick()
    {
        if (audioUI != null && sonidoClick != null)
        {
            audioUI.PlayOneShot(sonidoClick);
        }
    }

    public void BotonJugar()
    {
        ReproducirClick();
        StartCoroutine(CargarEscenaConDelay(escenaCinematica));
    }

    public void BotonAbrirInstrucciones()
    {
        ReproducirClick();
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);
    }

    public void BotonCerrarInstrucciones()
    {
        ReproducirClick();
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    public void BotonSalir()
    {
        ReproducirClick();
        StartCoroutine(SalirConDelay());
    }

    private IEnumerator CargarEscenaConDelay(string nombreEscena)
    {
        yield return new WaitForSeconds(0.15f);
        SceneManager.LoadScene(nombreEscena);
    }

    private IEnumerator SalirConDelay()
    {
        yield return new WaitForSeconds(0.15f);
        Debug.Log("Saliendo de The Chemical Table...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
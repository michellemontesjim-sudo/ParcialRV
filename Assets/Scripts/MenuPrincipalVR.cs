using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalVR : MonoBehaviour
{
    [Header("Nombres de Escenas")]
    [Tooltip("Nombre exacto de la escena de la cinemática inicial")]
    public string escenaCinematica = "CinematicaIntro";

    [Tooltip("Nombre exacto de la escena del tutorial")]
    public string escenaTutorial = "TutorialScene";

    [Header("Paneles UI")]
    public GameObject panelInstrucciones;

    void Start()
    {
        // Asegura que el panel de instrucciones empiece cerrado
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    public void BotonJugar()
    {
        SceneManager.LoadScene(escenaCinematica);
    }

    public void BotonTutorial()
    {
        SceneManager.LoadScene(escenaTutorial);
    }

    public void BotonAbrirInstrucciones()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(true);
    }

    public void BotonCerrarInstrucciones()
    {
        if (panelInstrucciones != null)
            panelInstrucciones.SetActive(false);
    }

    public void BotonSalir()
    {
        Debug.Log("Saliendo de The Chemical Table...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

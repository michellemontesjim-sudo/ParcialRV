using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicaAlquimia : MonoBehaviour
{
    [Header("Manos y Animadores")]
    public Transform manoDerecha;
    public Animator animatorDerecha;
    public Transform manoIzquierda;
    public Animator animatorIzquierda;
    public float velocidadMano = 1.2f;

    [Header("3 Ingredientes y Tarro")]
    public GameObject ingrediente1;
    public GameObject ingrediente2;
    public GameObject ingrediente3;
    public Transform puntoVertidoTarro;

    [Header("Partículas")]
    public ParticleSystem particulasIngrediente;
    public ParticleSystem particulasPocionFinal;

    [Header("Sonidos del Tarro")]
    public AudioSource audioTarro;
    public AudioClip sonidoCaerIngrediente;
    public AudioClip sonidoPocionCompleta;

    [Header("Interfaz y Escena Final")]
    public GameObject textoBienvenida;
    public GameObject textoExito;
    public string nombreEscenaSiguiente = "SampleScene";

    private Vector3 posInicialDerecha;
    private Quaternion rotInicialDerecha;
    private Vector3 posInicialIzquierda;
    private Quaternion rotInicialIzquierda;

    void Start()
    {
        posInicialDerecha = manoDerecha.localPosition;
        rotInicialDerecha = manoDerecha.localRotation;
        posInicialIzquierda = manoIzquierda.localPosition;
        rotInicialIzquierda = manoIzquierda.localRotation;

        if (textoExito != null) textoExito.SetActive(false);
    }

    public void TomarIngrediente1() => StartCoroutine(SecuenciaIngrediente(ingrediente1, manoDerecha, animatorDerecha, posInicialDerecha, rotInicialDerecha, 0.95f));
    public void TomarIngrediente2() => StartCoroutine(SecuenciaIngrediente(ingrediente2, manoIzquierda, animatorIzquierda, posInicialIzquierda, rotInicialIzquierda, 1.05f));
    public void TomarIngrediente3() => StartCoroutine(SecuenciaIngrediente(ingrediente3, manoDerecha, animatorDerecha, posInicialDerecha, rotInicialDerecha, 1.15f));

    public void CompletarPocionYCambiarEscena() => StartCoroutine(SecuenciaFinal());

    private IEnumerator SecuenciaIngrediente(GameObject frasco, Transform mano, Animator anim, Vector3 posReposo, Quaternion rotReposo, float pitchTarro)
    {
        Collider col = frasco.GetComponent<Collider>();
        Vector3 destinoFrasco = (col != null) ? col.bounds.center : frasco.transform.position;

        while (Vector3.Distance(mano.position, destinoFrasco) > 0.05f)
        {
            mano.position = Vector3.MoveTowards(mano.position, destinoFrasco, velocidadMano * Time.deltaTime);
            yield return null;
        }

        if (anim != null) anim.SetFloat("Grip", 1f);

        Rigidbody rb = frasco.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        Vector3 desfasePivote = frasco.transform.position - destinoFrasco;
        frasco.transform.SetParent(mano);
        frasco.transform.position = mano.position + desfasePivote + (mano.forward * 0.05f);

        while (Vector3.Distance(mano.position, puntoVertidoTarro.position) > 0.05f)
        {
            mano.position = Vector3.MoveTowards(mano.position, puntoVertidoTarro.position, velocidadMano * Time.deltaTime);
            yield return null;
        }

        Quaternion rotOriginal = mano.rotation;
        mano.Rotate(0f, 0f, 75f);
        if (particulasIngrediente != null) particulasIngrediente.Play();

        if (audioTarro != null) audioTarro.pitch = pitchTarro;
        ReproducirSonidoConDebug(audioTarro, sonidoCaerIngrediente, "Caer Ingrediente");

        yield return new WaitForSeconds(0.9f);

        frasco.SetActive(false);
        if (anim != null) anim.SetFloat("Grip", 0f);
        mano.rotation = rotOriginal;

        while (Vector3.Distance(mano.localPosition, posReposo) > 0.02f)
        {
            mano.localPosition = Vector3.MoveTowards(mano.localPosition, posReposo, velocidadMano * Time.deltaTime);
            mano.localRotation = Quaternion.RotateTowards(mano.localRotation, rotReposo, 250f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SecuenciaFinal()
    {
        if (particulasPocionFinal != null) particulasPocionFinal.Play();

        if (audioTarro != null) audioTarro.pitch = 1f;
        ReproducirSonidoConDebug(audioTarro, sonidoPocionCompleta, "Pocion Completa");

        if (textoBienvenida != null) textoBienvenida.SetActive(false);
        if (textoExito != null) textoExito.SetActive(true);

        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }

    private void ReproducirSonidoConDebug(AudioSource fuente, AudioClip clip, string nombreEvento)
    {
        if (fuente == null)
        {
            Debug.LogWarning($"[Audio Debug] {nombreEvento}: El AudioSource es NULL (no está asignado en el Inspector).");
            return;
        }
        if (clip == null)
        {
            Debug.LogWarning($"[Audio Debug] {nombreEvento}: El AudioClip es NULL (no hay archivo de audio asignado).");
            return;
        }
        if (!fuente.enabled || !fuente.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"[Audio Debug] {nombreEvento}: El AudioSource o el objeto '{fuente.gameObject.name}' está desactivado.");
            return;
        }

        Debug.Log($"[Audio Debug] {nombreEvento}: Reproduciendo '{clip.name}' en '{fuente.gameObject.name}' | Volume: {fuente.volume} | SpatialBlend: {fuente.spatialBlend}");
        fuente.PlayOneShot(clip);
    }
}
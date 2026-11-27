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
    public GameObject ingrediente1; // Lo toma la mano derecha
    public GameObject ingrediente2; // Lo toma la mano izquierda
    public GameObject ingrediente3; // Lo toma la mano derecha
    public Transform puntoVertidoTarro; // Un Empty justo encima de la boca del tarro

    [Header("Partículas")]
    public ParticleSystem particulasIngrediente;
    public ParticleSystem particulasPocionFinal;

    [Header("Sonidos 3D")]
    public AudioSource audioManoDerecha;
    public AudioSource audioManoIzquierda;
    public AudioClip sonidoAgarrar;
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

    // Estos 4 métodos se llaman desde el Timeline con Signal Emitters
    public void TomarIngrediente1() => StartCoroutine(SecuenciaIngrediente(ingrediente1, manoDerecha, animatorDerecha, audioManoDerecha, posInicialDerecha, rotInicialDerecha, 0.95f));
    public void TomarIngrediente2() => StartCoroutine(SecuenciaIngrediente(ingrediente2, manoIzquierda, animatorIzquierda, audioManoIzquierda, posInicialIzquierda, rotInicialIzquierda, 1.05f));
    public void TomarIngrediente3() => StartCoroutine(SecuenciaIngrediente(ingrediente3, manoDerecha, animatorDerecha, audioManoDerecha, posInicialDerecha, rotInicialDerecha, 1.15f));

    public void CompletarPocionYCambiarEscena() => StartCoroutine(SecuenciaFinal());

    private IEnumerator SecuenciaIngrediente(GameObject frasco, Transform mano, Animator anim, AudioSource audioMano, Vector3 posReposo, Quaternion rotReposo, float pitchTarro)
    {
        // 1. Obtener el centro visual del frasco (usando su Collider si tiene) y mover la mano hacia allí
        Collider col = frasco.GetComponent<Collider>();
        Vector3 destinoFrasco = (col != null) ? col.bounds.center : frasco.transform.position;

        while (Vector3.Distance(mano.position, destinoFrasco) > 0.05f)
        {
            mano.position = Vector3.MoveTowards(mano.position, destinoFrasco, velocidadMano * Time.deltaTime);
            yield return null;
        }

        // 2. Cerrar dedos, sonar agarre y pegar frasco centrado en la mano
        if (anim != null) anim.SetFloat("Grip", 1f);
        if (audioMano != null && sonidoAgarrar != null) audioMano.PlayOneShot(sonidoAgarrar);

        Rigidbody rb = frasco.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        Vector3 desfasePivote = frasco.transform.position - destinoFrasco;
        frasco.transform.SetParent(mano);
        frasco.transform.position = mano.position + desfasePivote + (mano.forward * 0.05f);

        // 3. Llevar frasco encima del tarro
        while (Vector3.Distance(mano.position, puntoVertidoTarro.position) > 0.05f)
        {
            mano.position = Vector3.MoveTowards(mano.position, puntoVertidoTarro.position, velocidadMano * Time.deltaTime);
            yield return null;
        }

        // 4. Inclinar mano para verter, soltar partículas y sonido del tarro
        Quaternion rotOriginal = mano.rotation;
        mano.Rotate(0f, 0f, 75f);
        if (particulasIngrediente != null) particulasIngrediente.Play();
        if (audioTarro != null && sonidoCaerIngrediente != null)
        {
            audioTarro.pitch = pitchTarro;
            audioTarro.PlayOneShot(sonidoCaerIngrediente);
        }

        yield return new WaitForSeconds(0.9f);

        // 5. Consumir frasco, abrir dedos y regresar mano a reposo
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
        if (audioTarro != null && sonidoPocionCompleta != null)
        {
            audioTarro.pitch = 1f;
            audioTarro.PlayOneShot(sonidoPocionCompleta);
        }

        if (textoBienvenida != null) textoBienvenida.SetActive(false);
        if (textoExito != null) textoExito.SetActive(true);

        yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}
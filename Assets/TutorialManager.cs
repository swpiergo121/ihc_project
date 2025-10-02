using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // Elimina todas las variables públicas relacionadas con el carrusel (pasos, botonAnterior, botonSiguiente, etc.)

    // Función para ir al siguiente paso lógico (Permisos)
    public void Continuar()
    {
        // Esta función será conectada al botón "Comenzar" o "Saltar"
        SceneManager.LoadScene("Pasodos");
    }

    // Función para volver al menú
    public void VolverAMenu()
    {
        // Esta función será conectada al botón "Anterior" si lo mantienes
        SceneManager.LoadScene("MenuPrincipal");
    }

    // Si quieres mantener el botón "Saltar Tutorial" separado:
    public void SaltarTutorial()
    {
        // Simplemente llama a la misma función Continuar()
        SceneManager.LoadScene("Permisos");
    }
}
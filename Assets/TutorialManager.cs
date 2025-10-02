using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // Elimina todas las variables p�blicas relacionadas con el carrusel (pasos, botonAnterior, botonSiguiente, etc.)

    // Funci�n para ir al siguiente paso l�gico (Permisos)
    public void Continuar()
    {
        // Esta funci�n ser� conectada al bot�n "Comenzar" o "Saltar"
        SceneManager.LoadScene("Pasodos");
    }

    // Funci�n para volver al men�
    public void VolverAMenu()
    {
        // Esta funci�n ser� conectada al bot�n "Anterior" si lo mantienes
        SceneManager.LoadScene("MenuPrincipal");
    }

    // Si quieres mantener el bot�n "Saltar Tutorial" separado:
    public void SaltarTutorial()
    {
        // Simplemente llama a la misma funci�n Continuar()
        SceneManager.LoadScene("Permisos");
    }
}
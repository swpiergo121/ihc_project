using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void IrATutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void IrAPermisos()
    {
        SceneManager.LoadScene("Permisos");
    }

    public void IrAEscenaAR()
    {
        SceneManager.LoadScene("text");
    }

    public void MostrarProximamente()
    {
        Debug.Log("Pr�ximamente");
        // O crear un popup simple
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PermisosManager : MonoBehaviour
{
    public void PermitirAcceso()
    {
        // Aquí iría la solicitud real de permisos
        SceneManager.LoadScene("text");
    }

    public void Cancelar()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}
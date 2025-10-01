// AbrirMenuMasa.cs
using UnityEngine;

public class AbrirMenuMasa : MonoBehaviour
{
    public GameObject menuSlider;
    public GameObject pelota;

    public void AbrirMenu()
    {
        Slingshot slingshot = pelota.GetComponent<Slingshot>();
        if (slingshot != null)
        {
            slingshot.enabled = false;
        }

        menuSlider.SetActive(true);
    }
}
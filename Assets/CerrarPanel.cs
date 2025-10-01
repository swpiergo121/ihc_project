using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerrarPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject panelACerrar;
    public GameObject pelota;
    public void CerrarMenu()
    {
        panelACerrar.SetActive(false);

        if (pelota != null)
        {
            Slingshot slinshot = pelota.GetComponent<Slingshot>();
            if (slinshot != null)
            {
                slinshot.enabled = true;
            }
        }
    }
}

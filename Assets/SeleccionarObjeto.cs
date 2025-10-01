using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeleccionarObjeto : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject menuSlider;
    private Slingshot slingshot;
    void Start()
    {
        slingshot = GetComponent<Slingshot>();
        
    }
    void OnMouseDown()
    {
        // descativvamos mientras el menu esta abierto
        slingshot.enabled = false;

        menuSlider.SetActive(true);
        menuSlider.transform.position = transform.position + new Vector3(0.15f, 0.1f, 0);

    }
}
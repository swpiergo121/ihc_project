using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

    rb = GetComponent<Rigidbody>();
    rb.useGravity = false;
    rb.isKinematic = true; // Prevent physics simulation until launched
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

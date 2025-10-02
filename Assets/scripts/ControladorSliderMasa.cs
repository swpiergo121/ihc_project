using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorSliderMasa : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;
    public TextMeshProUGUI textoMasa;
    public GameObject objetoObjetivo;

    private Rigidbody rb;
    private Vector3 escalaInicial;

    void Start()
    {
        slider.onValueChanged.AddListener(CambiarMasa);

        if (objetoObjetivo != null)
        {
            rb = objetoObjetivo.GetComponent<Rigidbody>();
            escalaInicial = objetoObjetivo.transform.localScale;

        }
    }
    void CambiarMasa(float valor)
    {
        Debug.Log($"Slider movido a: {valor}"); // Ver en Console

        if (objetoObjetivo == null)
        {
            Debug.LogError("Objeto Objetivo es NULL!");
            return;
        }

        float nuevaMasa = Mathf.Lerp(0.5f, 5f, valor);
        if (rb != null) rb.mass = nuevaMasa;

        float factorEscala = Mathf.Lerp(0.5f, 1.5f, valor);
        objetoObjetivo.transform.localScale = escalaInicial * factorEscala;

        if (textoMasa != null)
        {
            textoMasa.text = $"Masa: {nuevaMasa:F1} kg";
        }
        else
        {
            Debug.LogError("Texto Masa es NULL!");
        }
    }



}

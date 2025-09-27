using UnityEngine;

public class Slingshot : MonoBehaviour
{
    private Vector3 startPos;
    private bool isDragging = false;
    private Rigidbody rb;
    private Vector3 initialWorldPos; // Agregamos una variable para guardar la posición inicial MUNDIAL de la pelota

    [Header("Slingshot Settings")]
    public float forceMultiplier = 150f; // Ajuste la potencia base a un valor más típico, puedes probar con 15f
    public float maxStretch = 159f;     // Máximo estiramiento permitido (metros)
    public float dragPlaneDistance = 50f; // **Ajuste:** Usar una distancia mayor, como 5m, es más común para arrastrar objetos con la cámara

    // **Añadir:** Una variable para el punto de anclaje (si es diferente del centro)
    // En este caso, el anclaje será la posición inicial de la pelota.

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Guardamos la posición local original de la pelota para el reseteo
        startPos = transform.localPosition;
        // Guardamos la posición mundial original de la pelota para el cálculo de la dirección/estiramiento
        initialWorldPos = transform.position;
        rb.useGravity = false;
        // **CORRECCIÓN/MEJORA:** Aseguramos que la pelota esté en cinemática desde el inicio si es necesario, 
        // o al menos que no tenga velocidad.
        rb.isKinematic = false;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 1. Detección de toque inicial en la pelota
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        isDragging = true;
                        rb.isKinematic = true; // Hacemos cinemático para arrastrarlo sin física
                        rb.velocity = Vector3.zero; // Limpiamos cualquier velocidad residual
                        rb.angularVelocity = Vector3.zero;

                        // Opcional: Podrías querer resetear la posición a 'initialWorldPos' aquí por si se movió.
                        // transform.position = initialWorldPos; 
                    }
                }
            }

            // 2. Arrastre (solo si estamos arrastrando)
            if (isDragging && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
            {
                // Calcula la posición mundial donde se debe colocar la pelota
                Vector3 screenPoint = new Vector3(touch.position.x, touch.position.y, dragPlaneDistance);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);

                // Calculamos el vector de "estiramiento" (offset) desde la posición inicial (anclaje)
                Vector3 stretchVector = worldPos - initialWorldPos;

                // Limitamos la magnitud del estiramiento al máximo permitido
                // ¡IMPORTANTE! Aquí limitamos el vector de estiramiento
                stretchVector = Vector3.ClampMagnitude(stretchVector, maxStretch);

                // Establecemos la nueva posición MUNDIAL de la pelota
                // Nueva Posición = Posición Inicial + Vector de Estiramiento Limitado
                transform.position = initialWorldPos + stretchVector;
            }

            // 3. Soltar y lanzar
            if (touch.phase == TouchPhase.Ended && isDragging)
            {
                isDragging = false;
                rb.isKinematic = false;
                rb.useGravity = true;

                // Dirección: desde la posición actual hacia la posición inicial (el anclaje)
                // Esto es lo que da la sensación de lanzar en dirección opuesta al arrastre
                Vector3 direction = initialWorldPos - transform.position;

                // Distancia de estiramiento (magnitud del vector 'direction')
                float stretchDistance = direction.magnitude;

                // Fuerza: es directamente proporcional a la distancia (o distancia^2) y el multiplicador
                // Usar stretchDistance * forceMultiplier ya es un buen inicio. 
                // Si quieres más explosividad, puedes mantener Mathf.Pow(stretchDistance, 2)
                float launchForce = stretchDistance * forceMultiplier;

                // Aplicamos la fuerza como un impulso
                rb.AddForce(direction.normalized * launchForce, ForceMode.Impulse);
            }
        }
    }

    // ---

    // 4. Funciones de Reseteo (sin cambios significativos, solo aclaraciones)
    private void OnCollisionEnter(Collision collision)
    {
        // Se resetea 2 segundos después de chocar
        Invoke("ResetBall", 2f);
    }

    void ResetBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // La posición debe ser la 'startPos' (local) para volver a su punto original,
        // asumiendo que el padre no se ha movido.
        transform.localPosition = startPos;
        // Actualizamos la posición mundial inicial para el próximo lanzamiento
        initialWorldPos = transform.position;
        rb.useGravity = false;
        rb.isKinematic = false; // La dejamos en modo físico pero sin gravedad hasta el siguiente arrastre
    }
}
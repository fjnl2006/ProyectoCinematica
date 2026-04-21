using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour
{
    Vector2 lookInput;
    float currentAngleX = 0f;
    float currentAngleY = 0f;
    public float rotationSpeed = 2f; // Ajusta la sensibilidad de rotación

    [SerializeField] private float min;
    [SerializeField] private float max;
    [SerializeField] private Transform suelo;
    [SerializeField] private float velocidadBala = 30f;
    [SerializeField] private float gravedad = 9.8f;
    [SerializeField] private int pasosTrayectoria = 50; // Cantidad de puntos en la trayectoria
    [SerializeField] private GameObject direccion; 
    
    private LineRenderer lineRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Obtener o crear LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        // Configurar LineRenderer
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Si no hay input, no rotamos
        if (lookInput == Vector2.zero)
            return;
        
        // Usar el vector lookInput del New Input System
        // lookInput.x = movimiento horizontal (eje Y)
        // lookInput.y = movimiento vertical (eje X)
        
        // Acumular rotación en eje Y (izquierda-derecha)
        currentAngleY += lookInput.x * rotationSpeed;
        currentAngleY = Mathf.Clamp(currentAngleY, min, max);
        
        // Acumular rotación en eje X (arriba-abajo)
        currentAngleX -= lookInput.y * rotationSpeed;
        currentAngleX = Mathf.Clamp(currentAngleX, -50f, 70f); // Limitar rotación vertical
        
        // Aplicar rotación en X, Y, Z desde el pivote
        transform.rotation = Quaternion.Euler(currentAngleX, currentAngleY, 0);
        
        // Calcular y dibujar la trayectoria de la bala
        DibujarTrayectoria(direccion.transform.position, direccion.transform.forward, velocidadBala, gravedad);
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    
    /// <summary>
    /// Calcula y dibuja la trayectoria de la bala usando LineRenderer
    /// </summary>
    void DibujarTrayectoria(Vector3 origen, Vector3 direccion, float velocidad, float gravedad)
    {
        Vector3[] posiciones = new Vector3[pasosTrayectoria];
        float alturaSuelo = suelo != null ? suelo.position.y : 0f;
        
        for (int i = 0; i < pasosTrayectoria; i++)
        {
            float tiempo = (i / (float)(pasosTrayectoria - 1)) * CalcularTiempoImpacto(origen, direccion, velocidad, gravedad, alturaSuelo);
            
            // Calcular posición en el tiempo t
            Vector3 posicion = origen + direccion * velocidad * tiempo;
            posicion.y += -0.5f * gravedad * tiempo * tiempo;
            
            // Si la bala cae por debajo del suelo, detener
            if (posicion.y <= alturaSuelo)
            {
                posicion.y = alturaSuelo;
                // Redimensionar el array para que termine aquí
                System.Array.Resize(ref posiciones, i + 1);
                break;
            }
            
            posiciones[i] = posicion;
        }
        
        // Asignar posiciones al LineRenderer
        lineRenderer.positionCount = posiciones.Length;
        lineRenderer.SetPositions(posiciones);
    }
    
    /// <summary>
    /// Calcula el tiempo hasta que la bala impacta el suelo
    /// </summary>
    float CalcularTiempoImpacto(Vector3 origen, Vector3 direccion, float velocidad, float gravedad, float alturaSuelo)
    {
        // Usar ecuación cuadrática: y = y0 + (v*sin(θ))*t - (g*t²)/2
        // Cuando y = alturaSuelo, resolvemos para t
        
        float velocidadY = direccion.y * velocidad;
        float alturaInicial = origen.y - alturaSuelo;
        
        // at² + bt + c = 0
        float a = -0.5f * gravedad;
        float b = velocidadY;
        float c = alturaInicial;
        
        float discriminante = b * b - 4 * a * c;
        
        if (discriminante < 0)
            return 1f; // No hay solución real
        
        float t1 = (-b + Mathf.Sqrt(discriminante)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminante)) / (2 * a);
        
        // Retornar el tiempo positivo mayor
        if (t1 > 0 && t2 > 0)
            return Mathf.Max(t1, t2);
        else if (t1 > 0)
            return t1;
        else
            return t2;
    }
}

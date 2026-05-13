using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour
{
    Vector2 lookInput;
    float currentAngleX = 0f;
    float currentAngleY = 0f;
    public float rotationSpeed = 2f;

    [SerializeField] private float min;
    [SerializeField] private float max;
    [SerializeField] private Transform suelo;
    [SerializeField] private float velocidadBala = 30f;
    [SerializeField] private float gravedad = 9.8f;
    [SerializeField] private int pasosTrayectoria = 60;
    [SerializeField] private Transform direccion;
    private float bulletTime = 6f;
    [Header("Marcador de impacto")]
    [SerializeField] private float radioMarcador = 0.5f;

    [SerializeField] private GameObject prefabBala;

    private GameObject trajectoriaGO;
    private LineRenderer lineRenderer;
    private GameObject marcadorGO;
    private LineRenderer marcadorLR;
    public GameObject canvas;

    [SerializeField] private float shootTime;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        CrearTrayectoriaGO();
        CrearMarcadorGO();
        //canvas.SetActive(false);
    }


    void FixedUpdate()
    {
        if (lookInput != Vector2.zero)
        {
            currentAngleY += lookInput.x * rotationSpeed;
            currentAngleY = Mathf.Clamp(currentAngleY, min, max);

            currentAngleX -= lookInput.y * rotationSpeed;
            currentAngleX = Mathf.Clamp(currentAngleX, -50f, 70f);

            transform.rotation = Quaternion.Euler(currentAngleX, currentAngleY, 0);
        }

        DibujarTrayectoria(direccion.position, direccion.forward, velocidadBala, gravedad);
        shootTime += Time.deltaTime;

        // NOTA: El Canvas ahora se gestiona desde BridgeManager.SetActiveActionMap()
        // No manipular aquí para evitar conflictos
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (shootTime > 2f)
        {
            Vector3 velocidadInicial = direccion.forward * velocidadBala;

            GameObject bala = Instantiate(prefabBala, direccion.position, Quaternion.identity);
            Shoot scriptBala = bala.GetComponent<Shoot>();
            //scriptBala.dañoExplosion = WaveSpawner.instance.currentBulletDamage;
            //scriptBala.radius = WaveSpawner.instance.currentRadius;
            scriptBala.Inicializar(velocidadInicial, gravedad);
            Destroy(bala, bulletTime);
            shootTime = 0f;
            //canvas.SetActive(false);
        }

    }

    void CrearTrayectoriaGO()
    {
        trajectoriaGO = new GameObject("Trayectoria_Cannon");


        lineRenderer = trajectoriaGO.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.04f;


        Material mat = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = mat;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = new Color(1f, 0.3f, 0f);


        lineRenderer.sortingOrder = 10;
    }

    void CrearMarcadorGO()
    {
        marcadorGO = new GameObject("Marcador_Impacto");


        marcadorLR = marcadorGO.AddComponent<LineRenderer>();
        marcadorLR.useWorldSpace = true;
        marcadorLR.loop = true;
        marcadorLR.startWidth = 0.08f;
        marcadorLR.endWidth = 0.08f;
        marcadorLR.sortingOrder = 10;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        marcadorLR.material = mat;
        marcadorLR.startColor = new Color(1f, 0.15f, 0f, 0.95f);
        marcadorLR.endColor = new Color(1f, 0.85f, 0f, 0.95f);

        int seg = 32;
        marcadorLR.positionCount = seg;
        Vector3[] pts = new Vector3[seg];
        for (int i = 0; i < seg; i++)
        {
            float a = (i / (float)seg) * Mathf.PI * 2f;
            pts[i] = new Vector3(Mathf.Cos(a) * radioMarcador, 0f, Mathf.Sin(a) * radioMarcador);
        }
        marcadorLR.SetPositions(pts);
        marcadorGO.SetActive(false);
    }



    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }


    void DibujarTrayectoria(Vector3 origen, Vector3 dirUnit, float v, float g)
    {
        float ysuelo = suelo != null ? suelo.position.y : 0f;

        float vx = dirUnit.x * v;
        float vy = dirUnit.y * v;
        float vz = dirUnit.z * v;

        float tVuelo = CalcularTiempoVuelo(origen.y, vy, g, ysuelo);

        List<Vector3> puntos = new List<Vector3>();
        Vector3 impacto = origen;

        for (int i = 0; i < pasosTrayectoria; i++)
        {
            float t = (i / (float)(pasosTrayectoria - 1)) * tVuelo;

            float x = origen.x + vx * t;
            float y = origen.y + vy * t - 0.5f * g * t * t;
            float z = origen.z + vz * t;

            if (y <= ysuelo)
            {
                impacto = new Vector3(x, ysuelo, z);
                puntos.Add(impacto);
                break;
            }

            Vector3 p = new Vector3(x, y, z);
            puntos.Add(p);
            impacto = p;
        }

        lineRenderer.positionCount = puntos.Count;
        lineRenderer.SetPositions(puntos.ToArray());


        marcadorGO.SetActive(true);

        int segM = marcadorLR.positionCount;
        Vector3[] ptsM = new Vector3[segM];
        for (int i = 0; i < segM; i++)
        {
            float a = (i / (float)segM) * Mathf.PI * 2f;
            ptsM[i] = impacto + new Vector3(
                Mathf.Cos(a) * radioMarcador,
                0.02f,
                Mathf.Sin(a) * radioMarcador
            );
        }
        marcadorLR.SetPositions(ptsM);
    }


    float CalcularTiempoVuelo(float y0, float vy, float g, float ysuelo)
    {
        float a = 0.5f * g;
        float b = -vy;
        float c = -(y0 - ysuelo);
        float disc = b * b - 4f * a * c;

        if (disc < 0f) return 1f;

        float sqrtD = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtD) / (2f * a);
        float t2 = (-b - sqrtD) / (2f * a);

        if (t1 > 0f && t2 > 0f) return Mathf.Max(t1, t2);
        if (t1 > 0f) return t1;
        if (t2 > 0f) return t2;
        return 0.01f;
    }

    void OnDestroy()
    {

        if (trajectoriaGO != null) Destroy(trajectoriaGO);
        if (marcadorGO != null) Destroy(marcadorGO);
    }

    public void ResetInput()
    {
        lookInput = Vector2.zero;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class BridgeManager : MonoBehaviour
{
    [Header("Componentes ")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject topDownCamera;

    [Header("Listas de Puentes (Mismo Orden)")]
    [SerializeField] private ChangeCamera[] cameraChangers; //aqui va el camrechchanger que tiene cada cañon para rotar la camara de cada puente
    [SerializeField] private Cannon[] cannons;
    [SerializeField] private Door[] doors;

    private int activeBridgeIndex = 0;

    void Start()
    {
        SetTopDownView();
    }

    // INPUTS: MAPA TOPDOWN
    public void OnGoToBridge1(InputAction.CallbackContext context) { if (context.performed) SwitchToBridge(0); }
    public void OnGoToBridge2(InputAction.CallbackContext context) { if (context.performed) SwitchToBridge(1); }
    public void OnGoToBridge3(InputAction.CallbackContext context) { if (context.performed) SwitchToBridge(2); }
    public void OnGoToBridge4(InputAction.CallbackContext context) { if (context.performed) SwitchToBridge(3); }

    // INPUTS: MAPA CAÑON
    public void OnReturnToTopDown(InputAction.CallbackContext context) { if (context.performed) SetTopDownView(); }

    // para ciclar la cámara del puente activo, se llama desde el input del mapa de cañón
    public void OnCycleCamera(InputAction.CallbackContext context)
    {
        if (context.performed && cameraChangers.Length > activeBridgeIndex && cameraChangers[activeBridgeIndex] != null)
        {
            cameraChangers[activeBridgeIndex].CiclarCamara();
        }
    }

    public void OnLook(InputAction.CallbackContext context) { cannons[activeBridgeIndex].Look(context); }
    public void OnShoot(InputAction.CallbackContext context) { cannons[activeBridgeIndex].OnShoot(context); }

    public void OnActivateDoor(InputAction.CallbackContext context)
    {
        if (context.performed && doors.Length > activeBridgeIndex && doors[activeBridgeIndex] != null)
        {
            StartCoroutine(doors[activeBridgeIndex].BajarYSubir());
        }
    }

    // logica para el cmabvio de puente y inputs maps
    private void SwitchToBridge(int index)
    {
        activeBridgeIndex = index;
        playerInput.SwitchCurrentActionMap("BridgeMap");
        topDownCamera.SetActive(false);

        for (int i = 0; i < cameraChangers.Length; i++)
        {
            if (i == index)
            {
                cameraChangers[i].ActivarSistema(); // Enciende la cámara actual de este puente
            }
            else
            {
                cameraChangers[i].DesactivarSistema(); // Apaga las cámaras de los demás
                cannons[i].ResetInput();
            }
        }
    }

    private void SetTopDownView()
    {
        playerInput.SwitchCurrentActionMap("TopDownMap");
        topDownCamera.SetActive(true);

        for (int i = 0; i < cameraChangers.Length; i++)
        {
            cameraChangers[i].DesactivarSistema(); // Apaga todas las cámaras de los puentes
            cannons[i].ResetInput();
        }
    }
}
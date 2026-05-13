using UnityEngine;
using UnityEngine.InputSystem;

public class BridgeManager : MonoBehaviour
{
    [Header("Componentes ")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject topDownCamera;

    [Header("Listas de Puentes (Mismo Orden)")]
    [SerializeField] private ChangeCamera[] cameraChangers;
    [SerializeField] private Cannon[] cannons;
    [SerializeField] private Door[] doors;
    [SerializeField] private ControlPuerta[] drawbridges;

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

    public void OnMoverPuerta(InputAction.CallbackContext context)
    {
        Debug.Log($"[BridgeManager] Input MoverPuerta detectado. Reenviando a puente {activeBridgeIndex}");

        if (drawbridges.Length > activeBridgeIndex && drawbridges[activeBridgeIndex] != null)
        {
            drawbridges[activeBridgeIndex].OnMoverPuerta(context);
        }
        else
        {
            Debug.LogWarning($"[BridgeManager] No hay puerta levadiza asignada en el índice {activeBridgeIndex} del array Drawbridges");
        }
    }

    // LÓGICA DE TRANSICIÓN
    private void SwitchToBridge(int index)
    {
        activeBridgeIndex = index;
        playerInput.SwitchCurrentActionMap("BridgeMap");
        topDownCamera.SetActive(false);

        for (int i = 0; i < cameraChangers.Length; i++)
        {
            if (i == index)
            {
                cameraChangers[i].ActivarSistema();
            }
            else
            {
                cameraChangers[i].DesactivarSistema();
                cannons[i].ResetInput();

                if (drawbridges.Length > i && drawbridges[i] != null)
                {
                    drawbridges[i].OnMoverPuerta(new InputAction.CallbackContext());
                }
            }
        }
    }

    private void SetTopDownView()
    {
        playerInput.SwitchCurrentActionMap("TopDownMap");
        topDownCamera.SetActive(true);

        for (int i = 0; i < cameraChangers.Length; i++)
        {
            cameraChangers[i].DesactivarSistema();
            cannons[i].ResetInput();

            if (drawbridges.Length > i && drawbridges[i] != null)
            {
                drawbridges[i].OnMoverPuerta(new InputAction.CallbackContext());
            }
        }
    }
}
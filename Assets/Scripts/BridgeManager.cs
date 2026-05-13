using UnityEngine;
using UnityEngine.InputSystem;

public class BridgeManager : MonoBehaviour
{
    [Header("Componentes ")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject topDownCamera;

    [Header("Canvas por ActionMap")]
    [SerializeField] private GameObject topDownCanvas;
    [SerializeField] private GameObject bridgeMapCanvas;

    [Header("Listas de Puentes (Mismo Orden)")]
    [SerializeField] private ChangeCamera[] cameraChangers;
    [SerializeField] private Cannon[] cannons;
    [SerializeField] private Door[] doors;
    [SerializeField] private ControlPuerta[] drawbridges;

    private int activeBridgeIndex = 0;
    private string currentActiveActionMap = ""; // Inicializar vacío para forzar la inicialización

    void Start()
    {
        // Inicializar: desactivar todos los canvas
        if (topDownCanvas != null)
            topDownCanvas.SetActive(false);
        if (bridgeMapCanvas != null)
            bridgeMapCanvas.SetActive(false);

        // Ahora establecer la vista inicial (forzará SetActiveActionMap porque currentActiveActionMap está vacío)
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
        Debug.Log($"[BridgeManager] SwitchToBridge({index}) llamado");

        // PASO 1: Cambiar ActionMap (esto desactiva todos los ActionMaps y los Canvas)
        SetActiveActionMap("BridgeMap");

        // PASO 2: Actualizar índice
        activeBridgeIndex = index;
        topDownCamera.SetActive(false);

        // PASO 3: Actualizar sistemas de puente
        for (int i = 0; i < cameraChangers.Length; i++)
        {
            if (i == index)
            {
                cameraChangers[i].ActivarSistema();
                // Activar Canvas del cañón actual
                if (cannons[i] != null && cannons[i].canvas != null)
                {
                    cannons[i].canvas.SetActive(true);
                    Debug.Log($"[BridgeManager] Canvas del cañón {i} activado");
                }
            }
            else
            {
                cameraChangers[i].DesactivarSistema();
                cannons[i].ResetInput();

                // Desactivar Canvas de otros cañones
                if (cannons[i] != null && cannons[i].canvas != null)
                {
                    cannons[i].canvas.SetActive(false);
                }

                if (drawbridges.Length > i && drawbridges[i] != null)
                {
                    drawbridges[i].OnMoverPuerta(new InputAction.CallbackContext());
                }
            }
        }
    }

    private void SetTopDownView()
    {
        Debug.Log("[BridgeManager] SetTopDownView() llamado");

        // PASO 1: Cambiar ActionMap (esto desactiva todos los ActionMaps y los Canvas)
        SetActiveActionMap("TopDownMap");

        // PASO 2: Actualizar cámara
        topDownCamera.SetActive(true);

        // PASO 3: Desactivar sistemas de puente
        for (int i = 0; i < cameraChangers.Length; i++)
        {
            cameraChangers[i].DesactivarSistema();
            cannons[i].ResetInput();

            // Desactivar Canvas de todos los cañones
            if (cannons[i] != null && cannons[i].canvas != null)
            {
                cannons[i].canvas.SetActive(false);
            }

            if (drawbridges.Length > i && drawbridges[i] != null)
            {
                drawbridges[i].OnMoverPuerta(new InputAction.CallbackContext());
            }
        }
    }

    /// <summary>
    /// Establece el ActionMap activo de forma exclusiva.
    /// Garantiza que solo un ActionMap esté activo a la vez.
    /// Gestiona los Canvas según el ActionMap activo.
    /// </summary>
    private void SetActiveActionMap(string actionMapName)
    {
        if (currentActiveActionMap == actionMapName)
        {
            return; // Ya está activo, no hacer nada
        }

        // Desactivar TODOS los ActionMaps primero
        var inputActions = playerInput.actions;
        foreach (var actionMap in inputActions.actionMaps)
        {
            if (actionMap.enabled)
            {
                actionMap.Disable();
                Debug.Log($"[BridgeManager] ActionMap '{actionMap.name}' desactivado");
            }
        }

        // Luego activar solo el ActionMap deseado
        playerInput.SwitchCurrentActionMap(actionMapName);
        currentActiveActionMap = actionMapName;
        Debug.Log($"[BridgeManager] ActionMap '{actionMapName}' activado");

        // Gestionar Canvas según el ActionMap
        if (actionMapName == "TopDownMap")
        {
            if (topDownCanvas != null)
            {
                topDownCanvas.SetActive(true);
                Debug.Log("[BridgeManager] TopDownCanvas activado");
            }
            if (bridgeMapCanvas != null)
            {
                bridgeMapCanvas.SetActive(false);
                Debug.Log("[BridgeManager] BridgeMapCanvas desactivado");
            }
        }
        else if (actionMapName == "BridgeMap")
        {
            if (topDownCanvas != null)
            {
                topDownCanvas.SetActive(false);
                Debug.Log("[BridgeManager] TopDownCanvas desactivado");
            }
            if (bridgeMapCanvas != null)
            {
                bridgeMapCanvas.SetActive(true);
                Debug.Log("[BridgeManager] BridgeMapCanvas activado");
            }
        }
    }
}
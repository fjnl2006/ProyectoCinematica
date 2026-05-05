using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCamera : MonoBehaviour
{
    public Door Door;
    public List<Camera> cameras;
    public int index = 0;
    public Camera currentCamera;

    void Awake()
    {
        SetCamera(0);
    }

    // Este método será llamado por el sistema de eventos del PlayerInput
    public void OnCameraAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            NextCamera();
        }
    }


    void NextCamera()
    {
        index++;
        if (index >= cameras.Count)
            index = 0;

        SetCamera(index);
    }

    void SetCamera(int i)
    {
        foreach (Camera cam in cameras)
            cam.gameObject.SetActive(false);

        currentCamera = cameras[i];
        currentCamera.gameObject.SetActive(true);
        Debug.Log("Cámara cambiada a: " + currentCamera.name);
    }
    public void OnDoorAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnDoorAction triggered");
            StartCoroutine(Door.BajarYSubir());
        }
    }
}

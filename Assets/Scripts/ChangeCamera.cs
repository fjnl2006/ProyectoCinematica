using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeCamera : MonoBehaviour
{
    public List<Camera> cameras;
    public int index = 0;
    public Camera currentCamera;

    PlayerInput input;
    InputAction cameraAction;

    void Start()
    {
        input = GetComponent<PlayerInput>();


        input.actions.Enable();

        cameraAction = input.actions["Camera"];

        SetCamera(0);
    }


    void Update()
    {
        if (cameraAction.WasPressedThisFrame())
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
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Settings : MonoBehaviour
{
    
    [SerializeField] private GameObject menuPausa;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEscPressed(InputAction.CallbackContext context)
    {
        Time.timeScale = 0f;
        menuPausa.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    public void OnBack()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuPausa.SetActive(false);
    }
}

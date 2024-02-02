using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Controls playerControls;

    private InputAction settingsAction;


    private void Start()
    {
        SetWindowActive( false );
    }
    
    private void Awake()
    {
        playerControls = new Controls();
    }
    
    
    private void OnEnable()
    {
        settingsAction = playerControls.UI.Settings;

        settingsAction.Enable();

        settingsAction.performed += ToggleSettings;
    }

    private void OnDisable()
    {
        settingsAction.Disable();
    }

    private void ToggleSettings( InputAction.CallbackContext callbackContext )
    {
        SetWindowActive( !GetComponent<Canvas>().enabled );
    }
    
    //Enables/disables the canvas and interactability to show/hide the window
    public void SetWindowActive(bool value)
    {
        GetComponentInParent<Settings>().IsPaused = value;
        GetComponent<Canvas>().enabled = value;
        GetComponent<CanvasGroup>().interactable = value;
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class TaskWindow : MonoBehaviour
{
    private GameManager game;
    public GameObject taskContainer;
    //public List<Task> taskList;
    
    private TextMeshProUGUI[] textSlots;
    private Controls playerControls;

    private InputAction openTaskbarAction;
    private bool isTransitioning;
    
    private void Awake()
    {
        playerControls = new Controls();
        GetComponent<Canvas>().enabled = false;
        GetComponent<CanvasGroup>().interactable = false;
    }
    
    
    private void OnEnable()
    {
        openTaskbarAction = playerControls.UI.OpenTaskbar;

        openTaskbarAction.Enable();

        openTaskbarAction.performed += ToggleTaskbar;
    }

    private void OnDisable()
    {
        openTaskbarAction.Disable();
    }


    private void ToggleTaskbar(InputAction.CallbackContext context)
    {

        Canvas canvas = GetComponent<Canvas>();
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        //this check prevents spamming the task bar button from causing visual glitches
        if ( isTransitioning )
            return;
        
        
        if ( !canvas.enabled )
        {
            StartCoroutine( FadeIn( .5f, canvasGroup, canvas ) );
        }
        else
        {
            StartCoroutine( FadeOut(.5f, canvasGroup, canvas) );
        }

    }


    private IEnumerator FadeIn(float time, CanvasGroup cg, Canvas canvas)
    {
        time = Mathf.Max( time, .1f );
        float timer = 0;
        isTransitioning = true;
        canvas.enabled = true;
        cg.interactable = true;
        
        while ( timer < 1 )
        {
            cg.alpha = Mathf.SmoothStep( 0, 1, timer );
            timer += Time.deltaTime* (1/time);
            yield return null;
        }
        isTransitioning = false;
        cg.alpha = 1;
    }
    
    private IEnumerator FadeOut(float time, CanvasGroup cg, Canvas canvas)
    {
        time = Mathf.Max( time, .1f );
        float timer = 0;
        isTransitioning = true;
        
        while ( timer < 1 )
        {
            cg.alpha = Mathf.SmoothStep( 1, 0, timer );
            timer += Time.deltaTime * (1/time);
            yield return null;
        }
        isTransitioning = false;
        cg.alpha = 0;
        cg.interactable = false;
        canvas.enabled = false;
    }

    public void InitializeTasks()
    {
        textSlots = taskContainer.GetComponentsInChildren<TextMeshProUGUI>();
        game = GameObject.FindWithTag( "Game Manager" ).GetComponent<GameManager>();

        for ( int i = 0; i < textSlots.Length; i++ )
        {
            if ( i < game.Tasks.Count )
            {
                textSlots[i].text = game.Tasks[i].actionDescription;
            }
            else
            {
                textSlots[i].text = "";
            }
        }

        UpdateTasks();
        
    }

    private void ResetText()
    {
        foreach ( TextMeshProUGUI tmp in textSlots )
        {
            tmp.text = "";
            tmp.alpha = 1;
            tmp.fontStyle = FontStyles.Normal;
        }
    }

    public void UpdateTasks()
    {
        for ( int i = 0; i < game.Tasks.Count; i++ )
        {
            if ( !game.Tasks[i].IsAvailable )
            {
                textSlots[i].fontStyle = FontStyles.Strikethrough;
                textSlots[i].alpha = .5f;
            }
            else
            {
                textSlots[i].fontStyle = FontStyles.Normal;
                textSlots[i].alpha = 1;
            }
        }
        
        //TODO: play scribbling-out noise
    }
    
    
    
    
    
}

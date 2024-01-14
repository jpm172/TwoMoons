using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoonTrackerGame : MonoBehaviour
{

    private GameManager game;
    private Sprite newMoon, waxingCrescent, firstQuarter, waxingGibbous, fullMoon, waningGibbous, thirdQuarter, waningCrescent;
    private Sprite[] phases;

    public GameObject gamePiece, heldPiece, playArea, phasePanel;
    private Vector3 offset;


    private Controls playerControls;
    private InputAction mouseAction;
    
    
    // Start is called before the first frame update
    void Start()
    {
        game = GetComponentInParent<GameManager>();
        loadResources();
    }
    
    private void Awake()
    {
        playerControls = new Controls();
    }

    private void OnEnable()
    {
        mouseAction = playerControls.Player.Mouse;
        mouseAction.Enable();
    }
    
    private void OnDisable()
    {
        mouseAction = playerControls.Player.Mouse;
    }

    // Update is called once per frame
    void Update()
    {
        //whenever the mouse button is released, try and release any game pieces held
        if ( mouseAction.WasReleasedThisFrame() )
        {
            ReleaseMoon();
        }
        
        //Have the held game piece follow the mouse
        if ( heldPiece != null )
        {
            heldPiece.transform.position = Input.mousePosition + offset;
        }

    }

    private void loadResources()
    {
        phases = new Sprite[8];
        
        phases[0] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/New Moon" );
        phases[1] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Waxing Crescent" );
        phases[2] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/First Quarter" );
        phases[3] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Waxing Gibbous" );
        phases[4] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Full Moon" );
        phases[5] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Waning Gibbous" );
        phases[6] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Third Quarter" );
        phases[7] = Resources.Load<Sprite>( "Sprites/Moon Phases/Default/Waning Crescent" );
    }
    
    public void StartGame()
    {
        
    }

    public void PickUpMoon(GameObject obj)
    {
        if ( heldPiece == null )
        {
            heldPiece = obj;
            //offset is used to make the selected object move relative to the point you first clicked, rather than snapping it to the center of the mouse
            offset = heldPiece.transform.position - Input.mousePosition;
        }
    }
    

    public void ReleaseMoon()
    {
        //if there is a held game piece and it is being released in the delete area (the moon phase panel) outside the defined play area, delete the game piece
        if ( heldPiece != null && (isInDeleteArea() || !isInPlayArea() ) )
        {
            Destroy( heldPiece );
        }
        heldPiece = null;
    }

    //returns true if the game piece is within the bounds of the moon phase panel, false otherwise
    private bool isInDeleteArea()
    {
        //make sure both rects are in the same relative space so that Overlaps() works correctly
        Rect gamePieceRect = heldPiece.GetComponent<RectTransform>().GetWorldRect();
        Rect phasePanelRect = phasePanel.GetComponent<RectTransform>().GetWorldRect();

        return phasePanelRect.Overlaps( gamePieceRect  );
    }
    
    //returns true if the game piece is within the bounds of the defined play area, false otherwise
    private bool isInPlayArea()
    {
        Rect gamePieceRect = heldPiece.GetComponent<RectTransform>().GetWorldRect();
        Rect playAreaRect = playArea.GetComponent<RectTransform>().GetWorldRect();

        return playAreaRect.Overlaps( gamePieceRect  );
    }

    //spawns a game piece and assigns its sprite and events necessary to funciton
    //Phase: integer representing what moon phase was selected
    public void SelectedMoon(int phase)
    {
        Debug.Log( "selected moon phase " + phase );
        //spawn the game piece parented to the play area and set its sprite
        GameObject newGamePiece = Instantiate( gamePiece, Input.mousePosition, Quaternion.identity, playArea.transform );
        newGamePiece.GetComponent<Image>().sprite = phases[phase];
        
        //add the trigger event for picking up the piece on player click
        EventTrigger trigger = newGamePiece.GetComponent<EventTrigger>();

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerDown;
        entry2.callback.AddListener((data) => { PickUpMoon( newGamePiece ); });
        trigger.triggers.Add(entry2);

        //then call PickUpMoon so everything is updated properly
        PickUpMoon( newGamePiece );
    }
}


//helper function to get a RectTransform in worldspace
public static class RectTransformExtensions
{
    public static Rect GetWorldRect(this RectTransform rectTransform)
    {
        var localRect = rectTransform.rect;

        return new Rect
        {
            min = rectTransform.TransformPoint(localRect.min),
            max = rectTransform.TransformPoint(localRect.max)
        };
    }
}
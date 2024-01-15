using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoonTrackerGame : MonoBehaviour
{
    public TextMeshProUGUI playerFeedback;
    public GameObject gamePiece, playArea, phasePanel, displayArea;
    public float answerRelief = 25f;
    public bool debug;
    
    
    private GameManager game;
    private Sprite newMoon, waxingCrescent, firstQuarter, waxingGibbous, fullMoon, waningGibbous, thirdQuarter, waningCrescent;
    private Sprite[] phases;
    
    private GameObject heldPiece;
    private List<GameObject> placedPieces, debugPieces;
    private Vector3 offset;
    
    private Controls playerControls;
    private InputAction mouseAction;
    
    
    // Start is called before the first frame update
    void Start()
    {
        game = GetComponentInParent<GameManager>();
        placedPieces = new List<GameObject>();
        debugPieces = new List<GameObject>();
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
        mouseAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
        DebugMode();
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

    //Will enable/disable the debug objects for this game
    //For this game, that means hiding the game pieces that show the correct answer radius and phase on the play area
    private void DebugMode()
    {
        if ( debug )
        {
            foreach ( GameObject g in debugPieces )
            {
                g.SetActive( true );
                g.GetComponent<RectTransform>().sizeDelta = new Vector2( answerRelief * 2, answerRelief * 2 );
            }
        }
        else
        {
            foreach ( GameObject g in debugPieces )
            {
                g.SetActive( false );
            }
        }
    }

    //loads all of the moon phases into the phase list, sorted by their chronological order in the real world
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
        //destroy any remaining moons to make sure screen is clear
        foreach ( Transform answerMoon in displayArea.transform )
        {
            Destroy( answerMoon.gameObject );
        }

        foreach ( GameObject g in debugPieces )
        {
            Destroy( g );
        }
        debugPieces.Clear();
        
        //create moonCount game pieces that the player will try to match
        for ( int i = 0; i < game.moonCount; i++ )
        {
            GameObject moon = Instantiate( gamePiece, displayArea.transform.position + game.moonPositions[i], Quaternion.identity, displayArea.transform );
            moon.GetComponent<Image>().sprite = phases[ game.moonPhases[i] ];
            moon.GetComponent<GamePiece>().Info = game.moonPhases[i];
            
            //create debug moons in the play area that represent the answer radius
            GameObject debugMoon = Instantiate( gamePiece, playArea.transform.position + game.moonPositions[i], Quaternion.identity, playArea.transform );
            debugMoon.GetComponent<Image>().sprite = phases[ game.moonPhases[i] ];
            debugMoon.GetComponent<Image>().color = new Color(1,1,1, .33f);
            debugMoon.GetComponent<GraphicRaycaster>().enabled = false;
            debugMoon.GetComponent<RectTransform>().sizeDelta = new Vector2(answerRelief*2, answerRelief*2);
            
            debugPieces.Add( debugMoon );
        }
        
    }

    //Called by the submit button, checks if the answer is correct and tells the player what they did wrong
    public void SubmitGame()
    {
        string feedback = "";
        bool success = CheckAnswer( out feedback );

        if ( !success )
        {
            feedback = "Wrong!: " +feedback;
        }

        playerFeedback.text = feedback;
    }
    
    //takes the list of moons placed and checks if the player matched the phases/locations
    public bool CheckAnswer(out string message)
    {
        //do some basic checks to see if the player placed too few/many moons down
        if ( placedPieces.Count < game.moonCount )
        {
            message = "Not enough moons!";
            return false;
        }
        if ( placedPieces.Count > game.moonCount )
        {
            message = "Too many moons!";
            return false;
        }
        
        List<GameObject> correctAnswers = new List<GameObject>();
        //check to see if the player correctly matched the position/phases of each moon, making sure not to double count 
        foreach ( GameObject placed in placedPieces )
        {
            RectTransform playerMoonRect = placed.GetComponent<RectTransform>();
            foreach ( Transform answerMoon in displayArea.transform )
            {
                RectTransform answerMoonRect = answerMoon.GetComponent<RectTransform>();
                float dist = Vector2.Distance( playerMoonRect.anchoredPosition, answerMoonRect.anchoredPosition );
                
                if ( !correctAnswers.Contains( answerMoon.gameObject ) && dist <= answerRelief && answerMoon.GetComponent<GamePiece>().Info == placed.GetComponent<GamePiece>().Info )
                {
                    correctAnswers.Add( answerMoon.gameObject );
                    break;
                }
            }
        }

        //if not all were matched correctly, return false
        if ( correctAnswers.Count != game.moonCount )
        {
            message = "Some moons are in the wrong position/phase!";
            return false;
        }

        message = "Correct!";
        return true;
    }
    

    //Will have a game piece follow the mouse
    public void PickUpMoon(GameObject obj)
    {
        //TODO play drawing sound when picking up moon
        //only pick up piece if one is not already held
        if ( heldPiece == null )
        {
            heldPiece = obj;
            //offset is used to make the selected object move relative to the point you first clicked, rather than snapping it to the center of the mouse
            offset = heldPiece.transform.position - Input.mousePosition;
        }
    }
    
    //Will release the moon, and will check to see if the moon was dropped in a valid location or needs to be deleted
    public void ReleaseMoon()
    {
        //TODO play erasing sound when deleting moon / thunk sound when dropping moon
        //if there is a held game piece and it is being released in the delete area (the moon phase panel) outside the defined play area, delete the game piece
        if ( heldPiece != null && (isInDeleteArea() || !isInPlayArea() ) )
        {
            placedPieces.Remove( heldPiece );
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
        Rect playAreaRect = playArea.GetComponent<RectTransform>().GetWorldRect();
        
        return playAreaRect.Contains( heldPiece.transform.position );
    }

    //spawns a game piece and assigns its sprite and events necessary to funciton
    //Phase: integer representing what moon phase was selected
    public void SelectedMoon(int phase)
    {
        //spawn the game piece parented to the play area and set its sprite
        GameObject newGamePiece = Instantiate( gamePiece, Input.mousePosition, Quaternion.identity, playArea.transform );
        newGamePiece.GetComponent<Image>().sprite = phases[phase];
        newGamePiece.GetComponent<GamePiece>().Info = phase;
        placedPieces.Add( newGamePiece );
        
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





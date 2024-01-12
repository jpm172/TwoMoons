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
    public Sprite[] phases;

    public GameObject gamePiece, heldPiece;
    private Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        game = GetComponentInParent<GameManager>();
        loadResources();
    }
    

    // Update is called once per frame
    void Update()
    {
        /*
        if ( heldPiece != null )
        {
            heldPiece.transform.position = Input.mousePosition + offset;
            Debug.Log( heldPiece.GetComponent<RectTransform>().anchoredPosition.magnitude );
        }
        */
        
        
        if ( heldPiece != null )
        {
            heldPiece.transform.position = Input.mousePosition + offset;
            //Debug.Log( heldPiece.GetComponent<RectTransform>().anchoredPosition.magnitude );
        }
            
        Debug.Log( Input.mousePosition/GetComponentInParent<Canvas>().scaleFactor );
        //Debug.Log( GetComponent<Canvas>().scaleFactor );
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
        if ( heldPiece.GetComponent<RectTransform>().anchoredPosition.y > -110 )
        {
            Destroy( heldPiece );
        }
        heldPiece = null;
    }


    public void SelectedMoon(int phase)
    {
        Debug.Log( "selected moon phase " + phase );
        GameObject newGamePiece = Instantiate( gamePiece, Input.mousePosition, Quaternion.identity, transform );
        newGamePiece.GetComponent<Image>().sprite = phases[phase];
        
        EventTrigger trigger = newGamePiece.GetComponent<EventTrigger>();
        
        //add the trigger event for picking up the piece on player click
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerDown;
        entry2.callback.AddListener((data) => { PickUpMoon( newGamePiece ); });
        trigger.triggers.Add(entry2);
        
        
        //add trigger event for release the piece when letting go of click
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { ReleaseMoon(); });
        trigger.triggers.Add(entry);


        PickUpMoon( newGamePiece );
    }
}

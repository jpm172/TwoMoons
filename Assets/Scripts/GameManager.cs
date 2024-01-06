using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] LQ_Sprites, OBS_Sprites;
    public Image gameImage;
    
    
    public List< LevelNode > level;
    [SerializeField]private LevelNode curNode;
    
    
    private Controls playerControls;

    private InputAction moveForwardAction, moveBackAction, moveLeftAction, moveRightAction;
    
    
    void Start()
    {
        gameImage.enabled = true;
        initiateLevel();
    }

    
    private void Awake()
    {
        playerControls = new Controls();
    }
    
    //assign all the controls
    private void OnEnable()
    {
        moveForwardAction = playerControls.Player.MoveForward;
        moveBackAction = playerControls.Player.MoveBackward;
        moveLeftAction = playerControls.Player.MoveLeft;
        moveRightAction = playerControls.Player.MoveRight;
        
        moveForwardAction.Enable();
        moveBackAction.Enable();
        moveLeftAction.Enable();
        moveRightAction.Enable();
        
        
        moveForwardAction.performed += moveForward;
        moveBackAction.performed += moveBackward;
        moveLeftAction.performed += moveLeft;
        moveRightAction.performed += moveRight;
    }

    private void OnDisable()
    {
        moveForwardAction.Disable();
        moveBackAction.Disable();
        moveLeftAction.Disable();
        moveRightAction.Disable();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void initiateLevel()
    {
        level = new List<LevelNode>();
        
        LevelNode livingQuarters = new LevelNode( "Living Quarters", true, LQ_Sprites[0]  );
        LevelNode observatory = new LevelNode( "Observatory", true, OBS_Sprites[0] );

        
        
        connectLocations( livingQuarters, observatory, LQ_Sprites.Skip( 1 ).ToArray() );
        
        
        level.Add( livingQuarters );
        level.Add( observatory );

        curNode = livingQuarters;
        gameImage.sprite = curNode.sprite;
    }

    private void connectLocations( LevelNode startNode, LevelNode endNode, Sprite[] pathSprites )
    {
        LevelNode[] pathNodes = new LevelNode[pathSprites.Length];
        
        for ( int i = 0; i < pathSprites.Length; i++ )
        {
            pathNodes[i] = new LevelNode( i.ToString(), false, pathSprites[i] );
            if ( i > 0 )
            {
                pathNodes[i - 1].next = pathNodes[i];
                pathNodes[i].previous = pathNodes[i - 1];
            }
        }

        if ( pathNodes.Length == 0 )
        {
            startNode.next = endNode;
            endNode.previous = startNode;
        }
        else
        {
            startNode.next = pathNodes[0];
            pathNodes[0].previous = startNode;

            endNode.previous = pathNodes[pathNodes.Length - 1];
            pathNodes[pathNodes.Length - 1].next = endNode;
        }
        
    }

    public void moveForward( InputAction.CallbackContext context )
    {
        Debug.Log( "Move Forward" );
        curNode = curNode.next;
        gameImage.sprite = curNode.sprite;
    }

    public void moveBackward( InputAction.CallbackContext context )
    {
        Debug.Log( "Move Back" );
        curNode = curNode.previous;
        gameImage.sprite = curNode.sprite;
    }

    public void moveLeft(InputAction.CallbackContext context) { Debug.Log( "Move Left" );}

    public void moveRight( InputAction.CallbackContext context )
    {
        Debug.Log( "Move Right" );
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] LQ_Sprites, OBS_Sprites, OL_Sprites, Woods_Sprites, SNTY_Sprites, pathToSNTY_Sprites;
    public Image gameImage, forwardArrow, backwardArrow, leftArrow, rightArrow;
    
    
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
        LevelNode overlook = new LevelNode( "Overlook", true, OL_Sprites[0] );
        LevelNode woods = new LevelNode( "Woods", true, Woods_Sprites[0] );
        LevelNode sanctuary = new LevelNode( "Sanctuary", true, SNTY_Sprites[0] );

        createPath( livingQuarters, observatory, LQ_Sprites.Skip( 1 ).ToArray(), OBS_Sprites.Skip( 1 ).ToArray() );
        //createPath( overlook, woods, OL_Sprites.Skip( 1 ).ToArray() );
        //createPath( woods, overlook, Woods_Sprites.Skip( 1 ).ToArray() );
        //createPath( sanctuary, null, SNTY_Sprites.Skip( 1 ).ToArray() );

        //createBranch( livingQuarters, 4,true, sanctuary, pathToSNTY_Sprites );
        //createIntersection( livingQuarters, 2, overlook, 1, woods, 3 );
        //createIntersection( observatory, 4, woods, overlook );
        
        level.Add( livingQuarters );
        level.Add( observatory );
        level.Add( overlook );
        level.Add( woods );
        level.Add( sanctuary );

        curNode = livingQuarters;
        gameImage.sprite = curNode.sprite;
        updateMovementIndicators();
    }

    /*
    private void createBranch(Path originPath, int originIndex, bool isRight, LevelNode branchLocation, Sprite[] pathSprites )
    {
        LevelNode originNode = originLocation.getNodeAt( originIndex );
        originNode.isIntersection = true;

        LevelNode[] forwardPath = new LevelNode[pathSprites.Length];
        
        
        for ( int i = 0; i < pathSprites.Length; i++ )
        {
            forwardPath[i] = new LevelNode( i.ToString(), false, pathSprites[i] );
            if ( i > 0 )
            {
                forwardPath[i - 1].forward = forwardPath[i];
                forwardPath[i].backward = forwardPath[i - 1];
            }
        }


        forwardPath[0].backward = originLocation;
        forwardPath[forwardPath.Length - 1].forward = branchLocation;
        

        if ( isRight )
            originNode.right = forwardPath[0];
        else
            originNode.left = forwardPath[0];



    }
    */
    private void createIntersection(LevelNode mainLocation, int mainIndex, LevelNode leftLocation, int leftIndex, LevelNode rightLocation, int rightIndex)
    {
        LevelNode mainNode = mainLocation.forward;
        for ( int i = 1; i < mainIndex; i++ )
        {
            if ( mainNode.isLocation )
            {
                Debug.LogError( "Intersecting node out of bounds" );
                break;
            }
            mainNode = mainNode.forward;
        }


        LevelNode leftNode = leftLocation.forward;
        for ( int i = 1; i < leftIndex; i++ )
        {
            if ( leftNode.isLocation )
            {
                Debug.LogError( "Intersecting node out of bounds" );
                break;
            }
            leftNode = leftNode.forward;
        }
        
        LevelNode rightNode = rightLocation.forward;
        for ( int i = 1; i < rightIndex; i++ )
        {
            if ( rightNode.isLocation )
            {
                Debug.LogError( "Intersecting node out of bounds" );
                break;
            }
            rightNode = rightNode.forward;
        }
        

        mainNode.isIntersection = true;
        leftNode.isIntersection = true;
        rightNode.isIntersection = true;
    }
    
    //Will create the forward path between a start and end location by creating a linked list between the 2 locations
    //startNode: the starting location
    //endNode: the ending location
    //pathSprites: the ordered list of sprites that will be shown while moving down the path
    private void createPath( LevelNode startNode, LevelNode endNode, Sprite[] forwardSprites, Sprite[] backwardSprites )
    {
        LevelNode[] forwardPath = new LevelNode[forwardSprites.Length];
        LevelNode[] backwardPath = new LevelNode[backwardSprites.Length];
        
        for ( int i = 0; i < forwardSprites.Length; i++ )
        {
            forwardPath[i] = new LevelNode( i.ToString(), false, forwardSprites[i] );
            if ( i > 0 )
            {
                forwardPath[i - 1].forward = forwardPath[i];
                forwardPath[i].backward = forwardPath[i - 1];
            }
        }
        
        
        for ( int i = 0; i < backwardSprites.Length; i++ )
        {
            backwardPath[i] = new LevelNode( i.ToString(), false, backwardSprites[i] );
            if ( i > 0 )
            {
                backwardPath[i - 1].forward = backwardPath[i];
                backwardPath[i].backward = backwardPath[i - 1];
            }
        }
        
        if ( forwardPath.Length == 0 )
        {
            startNode.forward = endNode;
        }
        else
        {
            startNode.forward = forwardPath[0];
            forwardPath[0].backward = startNode;
            forwardPath[forwardPath.Length - 1].forward = endNode;
        }
        
        if ( backwardPath.Length == 0 )
        {
            endNode.forward = startNode;
        }
        else
        {
            endNode.forward = backwardPath[0];
            backwardPath[0].backward = endNode;
            backwardPath[backwardPath.Length - 1].forward = startNode;
        }
        
    }


    //Will set the opacity of the movement indicators to represent whether the player can move in a certain direction
    private void updateMovementIndicators()
    {
        Color imgColor = forwardArrow.color;
        
        imgColor.a = curNode.forward != null ? 1 : .3f;
        forwardArrow.color = imgColor;

        imgColor.a = curNode.backward != null ? 1 : .3f;
        backwardArrow.color = imgColor;

        imgColor.a = curNode.left != null ? 1 : .3f;
        leftArrow.color = imgColor;
        
        imgColor.a = curNode.right != null ? 1 : .3f;
        rightArrow.color = imgColor;
    }
    
    public void moveForward( InputAction.CallbackContext context )
    {
        if ( curNode.forward != null )
        {
            //Debug.Log( "Move Forward" );
            curNode = curNode.forward;
            gameImage.sprite = curNode.sprite;
            updateMovementIndicators();
        }
    }

    public void moveBackward( InputAction.CallbackContext context )
    {
        if ( curNode.backward != null )
        {
            //Debug.Log( "Move Back" );
            curNode = curNode.backward;
            gameImage.sprite = curNode.sprite;
            updateMovementIndicators();
        }
    }

    public void moveLeft( InputAction.CallbackContext context )
    {
        if ( curNode.left != null )
        {
            Debug.Log( "Move Left" );
            curNode = curNode.left;
            gameImage.sprite = curNode.sprite;
            updateMovementIndicators();
        }
        
        
    }

    public void moveRight( InputAction.CallbackContext context )
    {
        if ( curNode.right != null )
        {
            Debug.Log( "Move Right" );
            curNode = curNode.right;
            gameImage.sprite = curNode.sprite;
            updateMovementIndicators();
        }
    }


}

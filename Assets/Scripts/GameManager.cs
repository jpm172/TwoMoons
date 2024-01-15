using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] LQ_to_OBS, OBS_to_LQ, OL_to_path, path_to_OL, woods_to_path, path_to_woods, SNTY_to_path, path_to_SNTY;
    public Sprite LQ_Main, OBS_Main, Woods_Main, OL_Main, SNTY_Main;
    public Image gameImage, forwardArrow, backwardArrow, leftArrow, rightArrow;

    public int moonCount;
    public Vector3[] moonPositions;
    public int[] moonPhases;
    
    public List< LevelNode > level;
    [SerializeField]private LevelNode playerPosition;
    private TaskWindow task_window;
    
    private Controls playerControls;

    private InputAction moveForwardAction, moveBackAction, moveLeftAction, moveRightAction;
    
    
    void Start()
    {
        gameImage.enabled = true;
        task_window = GetComponentInChildren<TaskWindow>();
        moonCount = 2;
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
    
    
    void Update()
    {
        
    }

    public void initiateLevel()
    {
        level = new List<LevelNode>();

        loadResources();
        initializeMoons();
        
        //Create the key locations
        LevelNode livingQuarters = new LevelNode( "Living Quarters", true, LQ_Main  );
        LevelNode observatory = new LevelNode( "Observatory", true, OBS_Main );
        LevelNode overlook = new LevelNode( "Overlook", true, OL_Main );
        LevelNode woods = new LevelNode( "Woods", true, Woods_Main );
        LevelNode sanctuary = new LevelNode( "Sanctuary", true, SNTY_Main );
        
        
        //Create the paths between the locations
        //createPath( livingQuarters, LQ_to_OBS );
        //createPath( observatory, OBS_to_LQ );
        //createPath( woods, woods );
        
        //connectPath( livingQuarters, observatory, livingQuarters.getTail(), observatory.getTail(),  'F' );
        //connectPath( livingQuarters );
        createPath( livingQuarters, observatory, LQ_to_OBS, OBS_to_LQ );

        createBranch( livingQuarters, 4, observatory, 2, sanctuary, SNTY_to_path, path_to_SNTY );
        createBranch( livingQuarters, 2, observatory, 4, woods, woods_to_path, path_to_woods );
        createBranch(  observatory, 4, livingQuarters, 2,overlook, OL_to_path, path_to_OL );
        
        //store the locations in the level list
        level.Add( livingQuarters );
        level.Add( observatory );
        level.Add( overlook );
        level.Add( woods );
        level.Add( sanctuary );

        //set the player's starting position to be the living quarters
        playerPosition = livingQuarters;
        gameImage.sprite = playerPosition.sprite;
        updateMovementIndicators();
        //task_window.initializeTasks();
    }

    private void initializeMoons()
    {
        moonPhases = new int[moonCount];
        moonPositions = new Vector3[moonCount];
        
        for ( int i = 0; i < moonCount; i++ )
        {
            moonPhases[i] = Random.Range( 0, 7 );
            moonPositions[i] = new Vector2(Random.Range( -150, 150 ), Random.Range( -150, 150 ) );
        }
    }
    
    private void loadResources()
    {
        //loadedSprites =  Resources.LoadAll<Sprite>( "Sprites/Living Quarters/Main Sprites" );

        //load all the main sprites
        LQ_Main = Resources.Load<Sprite>( "Sprites/Living Quarters/Main/Living Quarters-Main" );
        OBS_Main = Resources.Load<Sprite>( "Sprites/Observatory/Main/Observatory-Main" );
        OL_Main = Resources.Load<Sprite>( "Sprites/Overlook/Main/Overlook-Main" );
        Woods_Main = Resources.Load<Sprite>( "Sprites/Woods/Main/Woods-Main" );
        SNTY_Main = Resources.Load<Sprite>( "Sprites/Sanctuary/Main/Sanctuary-Main" );
        
        //load all of the path sprites
        LQ_to_OBS =  Resources.LoadAll<Sprite>( "Sprites/Living Quarters/LQ to OBS" );
        OBS_to_LQ = Resources.LoadAll<Sprite>( "Sprites/Observatory/OBS to LQ" );
        OL_to_path = Resources.LoadAll<Sprite>( "Sprites/Overlook/OL to Path" );
        path_to_OL = Resources.LoadAll<Sprite>( "Sprites/Overlook/Path to OL" );
        woods_to_path = Resources.LoadAll<Sprite>( "Sprites/Woods/Woods to Path" );
        path_to_woods = Resources.LoadAll<Sprite>( "Sprites/Woods/Path to Woods" );
        SNTY_to_path = Resources.LoadAll<Sprite>( "Sprites/Sanctuary/SNTY to Path" );
        path_to_SNTY = Resources.LoadAll<Sprite>( "Sprites/Sanctuary/Path to SNTY" );
    }



    /*
    private void createPath( LevelNode main_location, Sprite[] forward_sprites )
    {
        if ( forward_sprites != null )
        {
            LevelNode[] forward_path = new LevelNode[forward_sprites.Length];

            for ( int i = 0; i < forward_sprites.Length; i++ )
            {
                forward_path[i] = new LevelNode( i.ToString(), false, forward_sprites[i] );
                if ( i > 0 )
                {
                    forward_path[i - 1].forward = forward_path[i];
                    forward_path[i].backward = forward_path[i - 1];
                }
            }

            main_location.forward = forward_path[0];
            forward_path[0].backward = main_location;
        }

    }
    */

    //F == forward, L == left, R == right
    private void connectPath(LevelNode locatioforward_ref, LevelNode locatiobackward_ref, LevelNode port1, LevelNode port2, char direction)
    {
        //LevelNode locatioforward_ref_tail = locatioforward_ref.getTail();
       // LevelNode locatiobackward_ref_tail = locatiobackward_ref.getTail();

        switch ( direction )
        {
            case 'F':
                port1.forward = locatiobackward_ref;
                port2.forward = locatioforward_ref;
                break;
            case 'L':
                port1.left = locatiobackward_ref;
                port2.right = locatioforward_ref;
                break;
            case 'R':
                break;
            default:
                Debug.LogError( direction +" is an Invalid Path Direction" );
                break;
        }


        
        
    }
    
    
    private void createBranch( LevelNode leftLocation, int leftNodeIndex, LevelNode rightLocation, int rightNodeIndex, LevelNode branchLocation, Sprite[] forwardSprites, Sprite[] backwardSprites )
    {
        LevelNode leftNode = leftLocation.getNodeAt( leftNodeIndex );
        LevelNode rightNode = rightLocation.getNodeAt( rightNodeIndex );

        leftNode.isIntersection = true;
        rightNode.isIntersection = true;

        //Only create the path if given sprites
        if ( forwardSprites != null )
        {
            LevelNode[] forwardPath = new LevelNode[forwardSprites.Length];
            for ( int i = 0; i < forwardSprites.Length; i++ )
            {
                forwardPath[i] = new LevelNode( i.ToString(), false, forwardSprites[i] );
                if ( i > 0 )
                {
                    forwardPath[i - 1].forward = forwardPath[i];
                    forwardPath[i].backward = forwardPath[i - 1];
                }
            }
            
            //connect the start of the branch location to its forward path
            branchLocation.forward = forwardPath[0];
            forwardPath[0].backward = branchLocation;
            
            //connect the end of the forward path such that it turns into the correct paths
            forwardPath[forwardPath.Length - 1].left = rightNode;
            forwardPath[forwardPath.Length - 1].right = leftNode;
            
            //store the path node in the root path so that intersections can be set up if needed
            rightNode.forward_ref = forwardPath[forwardPath.Length - 1];

            //since we store the intersect node in the right node, if there is something in the left node then we know we have a connection
            if ( leftNode.forward_ref != null )
            {
                //forward intersect refernce
                leftNode.backward_ref.backward = forwardPath[forwardPath.Length - 1];
                forwardPath[forwardPath.Length - 1].forward = leftNode.backward_ref;
            }

        }
        else//no forwards path == skip from path to branch location
        {
            rightNode.left = branchLocation;
            leftNode.right = branchLocation;

            rightNode.forward_ref = branchLocation;
            if ( leftNode.forward_ref != null )
            {
                leftNode.forward_ref.backward = branchLocation;

            }
        }
        
        //Only create the path if given sprites
        if ( backwardSprites != null )
        {
            LevelNode[] backwardPath = new LevelNode[backwardSprites.Length];
            for ( int i = 0; i < backwardSprites.Length; i++ )
            {
                backwardPath[i] = new LevelNode( i.ToString(), false, backwardSprites[i] );
                if ( i > 0 )
                {
                    backwardPath[i - 1].forward = backwardPath[i];
                    backwardPath[i].backward = backwardPath[i - 1];
                }
            }
            
            //connect the backward path to the correct positions
            backwardPath[0].right = rightNode;
            backwardPath[0].left = leftNode;
            backwardPath[backwardPath.Length - 1].forward = branchLocation;

            //set up the root branches to turn into the backward node
            rightNode.left = backwardPath[0];
            leftNode.right = backwardPath[0];
            
            //store the path node in the root path so that intersections can be set up if needed
            rightNode.backward_ref = backwardPath[0];

            if ( leftNode.forward_ref != null )
            {
                //backward intersence refernce
                leftNode.forward_ref.forward = backwardPath[0];
                backwardPath[0].backward = leftNode.forward_ref;
                //leftNode.backward_ref.backward = backwardPath[0];
            }

        }
        else//no backwards path == skip from branch location to path
        {
            branchLocation.right = rightNode;
            branchLocation.left = leftNode;

            rightNode.backward_ref = branchLocation;

            if ( leftNode.forward_ref != null )
            {
                branchLocation.backward = leftNode.backward_ref;
                leftNode.backward_ref.backward = branchLocation;
            }
        }


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
        
        imgColor.a = playerPosition.forward != null ? 1 : .3f;
        forwardArrow.color = imgColor;

        imgColor.a = playerPosition.backward != null ? 1 : .3f;
        backwardArrow.color = imgColor;

        imgColor.a = playerPosition.left != null ? 1 : .3f;
        leftArrow.color = imgColor;
        
        imgColor.a = playerPosition.right != null ? 1 : .3f;
        rightArrow.color = imgColor;
    }
    
    public void moveForward( InputAction.CallbackContext context )
    {
        if ( playerPosition.forward != null )
        {
            //Debug.Log( "Move Forward" );
            playerPosition = playerPosition.forward;
            gameImage.sprite = playerPosition.sprite;
            updateMovementIndicators();
        }
    }

    public void moveBackward( InputAction.CallbackContext context )
    {
        if ( playerPosition.backward != null )
        {
            //Debug.Log( "Move Back" );
            playerPosition = playerPosition.backward;
            gameImage.sprite = playerPosition.sprite;
            updateMovementIndicators();
        }
    }

    public void moveLeft( InputAction.CallbackContext context )
    {
        if ( playerPosition.left != null )
        {
            //Debug.Log( "Move Left" );
            playerPosition = playerPosition.left;
            gameImage.sprite = playerPosition.sprite;
            updateMovementIndicators();
        }
        
        
    }

    public void moveRight( InputAction.CallbackContext context )
    {
        if ( playerPosition.right != null )
        {
            //Debug.Log( "Move Right" );
            playerPosition = playerPosition.right;
            gameImage.sprite = playerPosition.sprite;
            updateMovementIndicators();
        }
    }


}

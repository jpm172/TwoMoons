using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite[] LQ_Sprites, OBS_Sprites, OL_Sprites, WoodsToPath_Sprites, pathToWoods_Sprites, SNTY_Sprites, pathToSNTY_Sprites;
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
    
    
    void Update()
    {
        
    }

    public void initiateLevel()
    {
        level = new List<LevelNode>();
        
        LevelNode livingQuarters = new LevelNode( "Living Quarters", true, LQ_Sprites[0]  );
        LevelNode observatory = new LevelNode( "Observatory", true, OBS_Sprites[0] );
        LevelNode overlook = new LevelNode( "Overlook", true, OL_Sprites[0] );
        LevelNode woods = new LevelNode( "Woods", true, WoodsToPath_Sprites[0] );
        LevelNode sanctuary = new LevelNode( "Sanctuary", true, SNTY_Sprites[0] );

        createPath( livingQuarters, observatory, LQ_Sprites.Skip( 1 ).ToArray(), OBS_Sprites.Skip( 1 ).ToArray() );

        createBranch( livingQuarters, 4, observatory, 2, sanctuary, SNTY_Sprites.Skip( 1 ).ToArray(), pathToSNTY_Sprites );
        createBranch( livingQuarters, 2, observatory, 4, woods, WoodsToPath_Sprites.Skip( 1 ).ToArray(), pathToWoods_Sprites.Skip( 1 ).ToArray() );
        createBranch(  observatory, 4, livingQuarters, 2,overlook, null, null );
        
        level.Add( livingQuarters );
        level.Add( observatory );
        level.Add( overlook );
        level.Add( woods );
        level.Add( sanctuary );

        curNode = livingQuarters;
        gameImage.sprite = curNode.sprite;
        updateMovementIndicators();
    }

    //Overloaded version of createBranch for when there is only one step from node to branch
    private void createBranch( LevelNode leftLocation, int leftNodeIndex, LevelNode rightLocation, int rightNodeIndex, LevelNode branchLocation )
    {
        LevelNode leftNode = leftLocation.getNodeAt( leftNodeIndex );
        LevelNode rightNode = rightLocation.getNodeAt( rightNodeIndex );
        
        Debug.Log( rightNode.isIntersection );
        
        leftNode.isIntersection = true;
        rightNode.isIntersection = true;
        
        branchLocation.left = rightNode;
        branchLocation.right = leftNode;

        rightNode.left = branchLocation;
        leftNode.right = branchLocation;

        if ( leftNode.n1 != null )
        {
            branchLocation.backward = leftNode.n2;
            leftNode.n2.backward = branchLocation;

            leftNode.n1.forward = branchLocation;

        }
        
    }

    
    private void createBranch( LevelNode leftLocation, int leftNodeIndex, LevelNode rightLocation, int rightNodeIndex, LevelNode branchLocation, Sprite[] forwardSprites, Sprite[] backwardSprites )
    {
        LevelNode leftNode = leftLocation.getNodeAt( leftNodeIndex );
        LevelNode rightNode = rightLocation.getNodeAt( rightNodeIndex );

        leftNode.isIntersection = true;
        rightNode.isIntersection = true;

        
        


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
            
            branchLocation.forward = forwardPath[0];
            forwardPath[0].backward = branchLocation;
            
            forwardPath[forwardPath.Length - 1].left = rightNode;
            forwardPath[forwardPath.Length - 1].right = leftNode;
            
            rightNode.n1 = forwardPath[forwardPath.Length - 1];

            if ( leftNode.n1 != null )
            {
                leftNode.n1.forward = forwardPath[forwardPath.Length - 1];
            }

        }
        else//no forwards path == skip from path to branch location
        {
            rightNode.left = branchLocation;
            leftNode.right = branchLocation;

            rightNode.n1 = branchLocation;
            if ( leftNode.n1 != null )
            {
                leftNode.n1.forward = branchLocation;

            }
        }


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
            
            backwardPath[0].right = rightNode;
            backwardPath[0].left = leftNode;
            backwardPath[backwardPath.Length - 1].forward = branchLocation;

            rightNode.left = backwardPath[0];
            leftNode.right = backwardPath[0];
            
            rightNode.n2 = backwardPath[0];

            if ( leftNode.n1 != null )
            {
                backwardPath[backwardPath.Length - 1] = leftNode.n2;
                leftNode.n2.backward = backwardPath[backwardPath.Length - 1];
            }

        }
        else//no backwards path == skip from branch location to path
        {
            branchLocation.right = rightNode;
            branchLocation.left = leftNode;

            rightNode.n2 = branchLocation;


            if ( leftNode.n1 != null )
            {
                branchLocation.backward = leftNode.n2;
                leftNode.n2.backward = branchLocation;
            }
        }


        if ( leftNode.n1 != null )
        {
            //branchLocation.backward = leftNode.n2;
            //leftNode.n2.backward = branchLocation;

            //leftNode.n1.forward = branchLocation;

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

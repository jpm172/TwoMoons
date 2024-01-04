using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


public class TextBoxHandler : MonoBehaviour
{
    
    public RectTransform textBoxBG;
    public string[] dialogueLines;
    public int fontSize = 14;
    public float scrollSpeed = .1f;
    public float timeToSize;
    public bool skipScroll, isWaitingForPage;
    public Controls playerControls;

    private InputAction interactAction;
    
    private RectTransform textRect;
    private TextMeshProUGUI tmp;
    
    private Coroutine currentTextCoroutine;
    private int lineCount = 0;
    



    private void Awake()
    {
        playerControls = new Controls();
    }
    
    private void OnEnable()
    {
        interactAction = playerControls.Player.interact;
        interactAction.Enable();
        interactAction.performed += interact;
    }

    private void OnDisable()
    {
        interactAction.Disable();;
    }

    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        textRect = GetComponent<RectTransform>();
        tmp.text = "";
    }


    private void Update()
    {

    }


    private void interact( InputAction.CallbackContext context )
    {
        if ( currentTextCoroutine != null )
        {
            skipScroll = true;
        }
        else
        {
            nextLine();
        }
    }

    //makes a deep copy of the pageInfo list by copying the first/last character indices of each page into a 2d array
    private int[,] GetPageBoundaries( TextMeshProUGUI tmp )
    {
        int[,] result = new int[tmp.textInfo.pageCount,2];
        for ( int i = 0; i < tmp.textInfo.pageCount; i++ )
        {
            result[i, 0] = tmp.textInfo.pageInfo[i].firstCharacterIndex;
            result[i, 1] = tmp.textInfo.pageInfo[i].lastCharacterIndex;

            if ( i == tmp.textInfo.pageCount - 1 )
            {
                result[i, 1] = tmp.text.Length + 1;
            }
        }
        return result;
    }
    
    IEnumerator TextScroll(string dialogue)
    {
        tmp.fontSize = fontSize;
        //Fill the text box and get the page boundaries
        tmp.text = dialogue;
        tmp.ForceMeshUpdate();

        StartCoroutine( resizeBackground(timeToSize) );
        
        int[,] pageBoundaries = GetPageBoundaries( tmp );
        //set the characters to be invisible make sure were on page 1
        tmp.maxVisibleCharacters = 0;
        tmp.pageToDisplay = 1;

        //scroll through each page
        for ( int n = 0; n < tmp.textInfo.pageCount; n++ )
        {
            //Scroll through the text on the nth page
            for ( int i = pageBoundaries[n,0]; i <= pageBoundaries[n,1]; i++ )
            {
                if ( skipScroll )
                {
                    tmp.maxVisibleCharacters = pageBoundaries[n,1];
                    break;
                }
                
                tmp.maxVisibleCharacters = i;
                yield return new WaitForSeconds( scrollSpeed );
            }

            //wait for the player to switch page only if we aren't on the last page
            if ( n < tmp.textInfo.pageCount - 1 )
            {
                isWaitingForPage = true;
                yield return new WaitUntil( () => !isWaitingForPage );
                tmp.pageToDisplay++;
                skipScroll = false;
            }
        }
        
        //set maxVisibleCharacters to the entire length so all characters are displayed
        tmp.maxVisibleCharacters = dialogue.Length;
        currentTextCoroutine = null;
        skipScroll = false;
    }


    //Smoothly Resizes the backround image to fit the text box
    //resizeTime: Time it takes in seconds to complete the transition (minimum of 0.01 seconds)
    private IEnumerator resizeBackground(float resizeTime)
    {
        resizeTime = Mathf.Max( .01f, resizeTime );//prevent time values < 0

        Vector2 spacing = new Vector2(20, 10);
        
        Vector2 startSize = textBoxBG.sizeDelta;
        Vector2 pref = tmp.GetPreferredValues();
        
        Vector2 targetSize = new Vector2( Mathf.Min( pref.x, textRect.sizeDelta.x ), 
                                        Mathf.Min( pref.y, textRect.sizeDelta.y ) );

        targetSize += spacing;//add the spacing vector to make sure the words are bumping up with the edges of the background
        
        float timer = 0;
        float progress = 0;
        
        while ( timer < 1 && !skipScroll )
        {
            //use SmoothStep to ease from start to target size
            progress = Mathf.SmoothStep( 0, 1, timer );
            timer += Time.deltaTime*(1/resizeTime);
            
            textBoxBG.sizeDelta = Vector2.Lerp( startSize, targetSize, progress );
            
            yield return new WaitForEndOfFrame();
        }

        textBoxBG.sizeDelta = targetSize;

    }
    
    public void nextLine()
    {
        currentTextCoroutine = StartCoroutine( TextScroll( dialogueLines[lineCount] ) );
        lineCount = ( lineCount + 1 ) % dialogueLines.Length;
    }


    
}

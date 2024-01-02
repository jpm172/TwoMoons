using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextBoxHandler : MonoBehaviour
{

    private TextMeshProUGUI tmp;

    public string[] dialogueLines;
    private int lineCount = 0;
    
    [SerializeField] private int fontSize = 14;
    [SerializeField] private float scrollSpeed = .1f;
    [SerializeField] private bool skipScroll, isWaitingForPage;

    private void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = "";
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
    }


    public void nextLine()
    {
        StartCoroutine( TextScroll( dialogueLines[lineCount] ) );
        lineCount = (lineCount + 1)%dialogueLines.Length;
    }
    
}

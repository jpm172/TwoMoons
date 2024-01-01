using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxHandler : MonoBehaviour
{
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
        // Scrolling will begin
        inTextScrolling = true;

        // Disable user input(click to next line)
        GameManager.Instance.userInputEnabled = false;

        // save current text speed
        float textSpeedSaved = GameManager.Instance.textSpeed;
        
        TextMeshProUGUI tmp = dialogBox.transform.GetChild( 0 ).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = GameManager.Instance.fontSize;
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
            //Debug.Log( "Page " +n );
            //Scroll through the text on the nth page
            for ( int i = pageBoundaries[n,0]; i <= pageBoundaries[n,1]; i++ )
            {
                //Debug.Log( "i " +i );
                //if (scrollingSkip) GameManager.Instance.textSpeed = 1;      
               
                if ( scrollingSkip )
                {
                    tmp.maxVisibleCharacters = pageBoundaries[n,1];
                    break;
                }
                
                tmp.maxVisibleCharacters = i;
                yield return new WaitForSeconds( 1 - GameManager.Instance.textSpeed );
            }
            
            // if user clicked to skip scrolling effect; reset text speed for future lines
            //not sure if its necessary to have this line in the page loop, but definitely need it at the very end
            if (scrollingSkip == true)
            {
                // go through each sprite set duration to 1
                foreach(Transform child in GameObject.Find("loadScene").gameObject.transform)
                {
                    if (child.name.Contains("charObject"))
                    {
                        child.GetComponent<Animated>()._totalDuration = 0;
                    }
                }
                scrollingSkip = false;
                GameManager.Instance.textSpeed = textSpeedSaved;
            }
            
            //wait for the player to switch page only if we aren't on the last page
            if ( n < tmp.textInfo.pageCount - 1 )
            {
                isWaitingForPage = true;
                yield return new WaitUntil( () => !isWaitingForPage );
                tmp.pageToDisplay++;
                scrollingSkip = false;
            }
        }
        
        //set maxVisibleCharacters to the entire length so all characters are displayed
        tmp.maxVisibleCharacters = dialogue.Length;

        // enable user input; scrolling has ended
        GameManager.Instance.userInputEnabled = true;
        // if user clicked to skip scrolling effect; reset text speed for future lines
        if (scrollingSkip == true)
        {
            // go through each sprite set duration to 1
            foreach(Transform child in GameObject.Find("loadScene").gameObject.transform)
            {
                if (child.name.Contains("charObject"))
                {
                    child.GetComponent<Animated>()._totalDuration = 0;
                }
            }
            scrollingSkip = false;
            GameManager.Instance.textSpeed = textSpeedSaved;
        }
        
        
        inTextScrolling = false; // text scrolling ended
    }
}

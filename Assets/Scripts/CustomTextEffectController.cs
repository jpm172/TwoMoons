using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
//The format to apply an effect is this:
//<link=effect_tag> All of this text will have some effect applied</link> while this text will not

//NOTE: You CANNOT nest the tags, if you want to combine tags you need to create a new effect from
//scratch that incorporates both effects under one effect tag
public class CustomTextEffectController : MonoBehaviour
{
    public TMP_Text textComponent;
    
    private List<CustomTextEffect> effectList = new List<CustomTextEffect>();
    
    public string curText;
    

    private void Start()
    {
        curText = "";
    }

    private void LateUpdate() 
    {
        textComponent.ForceMeshUpdate();

        //if curText is not equal to what is in the textbox, then update the effects list
        if ( !curText.Equals( textComponent.text ) )
        {
            getEffectList();
            curText = textComponent.text;
        }
        

        //go through each effect and apply it to the text
        foreach ( CustomTextEffect effect in effectList )
        {
            effect.applyEffect();
        }
        
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All); // IMPORTANT! applies all vertex and color changes.
    }

    //takes each link from the text and places any matching tags into the list
    private void getEffectList()
    {
        effectList.Clear();//clear the list so we don't have any effects left over
        
        TMP_LinkInfo[] linkInfoArr = textComponent.textInfo.linkInfo;
        for ( int i = 0; i < textComponent.textInfo.linkCount; i++ )
        {
            TMP_LinkInfo link = linkInfoArr[i];
            string linkID = link.GetLinkID();
            
            if ( linkID.Equals( "rainbow" ) )
            {
                effectList.Add( new RainbowEffect( link.linkTextfirstCharacterIndex,  link.linkTextLength, linkID, textComponent.textInfo ) );
            }
            else if ( linkID.Equals( "jitter" ) )
            {
                effectList.Add( new JitterEffect( link.linkTextfirstCharacterIndex,  link.linkTextLength, linkID, textComponent.textInfo ) );
            }
            else if ( linkID.Equals( "shake" ) )
            {
                effectList.Add( new ShakeEffect( link.linkTextfirstCharacterIndex,  link.linkTextLength, linkID, textComponent.textInfo ) );
            }
        }
        
    }
}
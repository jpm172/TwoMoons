using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

//This is the structure that each effect follows
public class CustomTextEffect 
{
    protected TMP_TextInfo textInfo;
    protected string effectTag;
    protected List<int> indices;
    protected int start, length;

    public CustomTextEffect( int newStart, int newLength, string newTag, TMP_TextInfo newTextInfo )
    {
        start = newStart;
        length = newLength;
        indices = new List<int>();
        indices.AddRange( Enumerable.Range( start, length ) );

        effectTag = newTag;

        textInfo = newTextInfo;
    }
    
    //this is the method called by CustomTextEffectController that you override with your own function
    public virtual void applyEffect() { }

    public override string ToString()
    {
        string str = "";
        foreach ( int i in indices )
        {
            str += i + ", ";
        }

        return effectTag + " on indices: " + str;
    }
}

//Shake Effect: Will constantly randomly shift each character to give a shaky effect to the text
//Good for showing fear/shock/suprise
public class ShakeEffect : CustomTextEffect
{
    private Vector3[] shakeOffset;
    private float magnitude = 1;
    public ShakeEffect( int newStart, int newLength, string newTag, TMP_TextInfo newTextInfo ) : base( newStart, newLength, newTag, newTextInfo )
    {}

    //Creates an array of random vectors to be used as the offset on the characters
    private Vector3[] getNewShakeOffset()
    {
        Vector3[] result = new Vector3[length];
        for ( int i = 0; i < result.Length; i++ )
        {
            result[i] = new Vector2( Random.Range( -magnitude, magnitude ),Random.Range( -magnitude, magnitude ) );
        }

        return result;
    }
    
    public override void applyEffect()
    {

        shakeOffset = getNewShakeOffset();

        for( int n = 0; n < indices.Count; n++ )
        {
            int index = indices[n];
            
            TMP_CharacterInfo charInfo = textInfo.characterInfo[index]; // Gets info on the current character
            int materialIndex = charInfo.materialReferenceIndex; // Gets the index of the current character material
            
            Vector3[] newVertices = textInfo.meshInfo[materialIndex].vertices;
 
            if(!charInfo.isVisible)//skip invisible characters (ie space)
                continue;
            
            // Loop all vertexes of the current characters
            for (int j = 0; j < 4; j++)
            {
                int vertexIndex = charInfo.vertexIndex + j;
                // apply the jitter offset
                newVertices[vertexIndex] += shakeOffset[n];
            }
        }
    }
}


//Jitter Effect: Will randomly shift each character every 0.25 seconds to give a chattering/jittery effect to the text
//Serves as a demo on how to create an effect that only updates on an internal timer
public class JitterEffect : CustomTextEffect
{
    private Vector3[] jitterOffset;
    private float timer = .25f;
    private float magnitude = 5;
    public JitterEffect( int newStart, int newLength, string newTag, TMP_TextInfo newTextInfo ) : base( newStart, newLength, newTag, newTextInfo )
    {
        jitterOffset = getNewjitterOffset();
    }

    //Creates an array of random vectors to be used as the offset on the characters
    private Vector3[] getNewjitterOffset()
    {
        Vector3[] result = new Vector3[length];
        for ( int i = 0; i < result.Length; i++ )
        {
            result[i] = new Vector2( Random.Range( -magnitude, magnitude ),Random.Range( -magnitude, magnitude ) );
        }

        return result;
    }
    
    public override void applyEffect()
    {
        timer -= Time.deltaTime;
        if ( timer <= 0 )//only change the offset every .25 seconds to give it a stepped jitter rather than a continuous shake
        {
            timer = .25f;
            jitterOffset = getNewjitterOffset();
        }
        
        for( int n = 0; n < indices.Count; n++ )
        {
            int index = indices[n];
            
            TMP_CharacterInfo charInfo = textInfo.characterInfo[index]; // Gets info on the current character
            int materialIndex = charInfo.materialReferenceIndex; // Gets the index of the current character material
            
            Vector3[] newVertices = textInfo.meshInfo[materialIndex].vertices;
 
            if(!charInfo.isVisible)//skip invisible characters (ie space)
                continue;
            
            // Loop all vertexes of the current characters
            for (int j = 0; j < 4; j++)
            {
                int vertexIndex = charInfo.vertexIndex + j;
                // apply the jitter offset
                newVertices[vertexIndex] += jitterOffset[n];
            }
        }
    }
}



//Rainbow Effect: Causes teh text to follow a wave as well as applying a rainbow colored gradient to the text
//Serves as a demo on how to change position and color of text
public class RainbowEffect : CustomTextEffect
{
    private Vector2 movementStrength = new Vector2(0.1f, 0.1f);
    private float movementSpeed = 1f;
    private float rainbowStrength = 10f;
    public RainbowEffect( int newStart, int newLength, string newTag, TMP_TextInfo newTextInfo ) : base( newStart, newLength, newTag, newTextInfo )
    {}

    public override void applyEffect()
    {
        foreach ( int index in indices )
        {

            TMP_CharacterInfo charInfo = textInfo.characterInfo[index]; // Gets info on the current character
            int materialIndex = charInfo.materialReferenceIndex; // Gets the index of the current character material
 
            Color32[] newColors = textInfo.meshInfo[materialIndex].colors32;
            Vector3[] newVertices = textInfo.meshInfo[materialIndex].vertices;
 
            if(!charInfo.isVisible)//skip invisible characters (ie space)
                    continue;

            // Loop all vertexes of the current characters
            for (int j = 0; j < 4; j++)
            {
                if (charInfo.character == ' ') continue; // Skips spaces
                int vertexIndex = charInfo.vertexIndex + j;
                   
                // Offset and Rainbow effects, replace it with any other effect you want.
                Vector3 offset = new Vector2(Mathf.Sin((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * movementStrength.x)), Mathf.Cos((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * movementStrength.y))) * 10f;
                Color32 rainbow = Color.HSVToRGB(((Time.realtimeSinceStartup * movementSpeed) + (vertexIndex * (0.001f * rainbowStrength))) % 1f, 1f, 1f);
                   
                // Sets the new effects
                newColors[vertexIndex] = rainbow;
                newVertices[vertexIndex] += offset;
            }
        }
    }
}

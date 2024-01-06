using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelNode
{
    [SerializeField] public string name;
    public bool isLocation, isIntersection;

    public Sprite sprite;

    //going back when on a location should always EXIT a location, and cause player to turn around
    //or have an exit direction assigned here for location nodes only?
    public LevelNode forward, backward, left, right, lastNodeArrivedFrom;
    
    
    public LevelNode( string newName, bool newIsLocation, Sprite newSprite )
    {
        name = newName;
        isLocation = newIsLocation;
        sprite = newSprite;
    }

    public LevelNode getNodeAt( int index )
    {
        LevelNode node = this;

        for ( int i = 0; i < index; i++ )
        {
            node = node.forward;
        }

        return node;
    }

    
}


public class Path
{
    
    public LevelNode forwardPath, returnPath ;


}
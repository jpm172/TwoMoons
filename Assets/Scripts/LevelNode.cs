using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelNode
{
    [SerializeField] public string name;
    public bool isLocation, isIntersection;
    private List<Action> locationActions;

    public Sprite sprite;

    //going back when on a location should always EXIT a location, and cause player to turn around
    //or have an exit direction assigned here for location nodes only?
    public LevelNode forward, backward, left, right, lastNodeArrivedFrom, forward_ref, backward_ref;


    public LevelNode( string newName, bool newIsLocation, Sprite newSprite )
    {
        name = newName;
        isLocation = newIsLocation;
        sprite = newSprite;
        locationActions = new List<Action>();
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

    public LevelNode getTail()
    {
        LevelNode curNode = this;

        while ( curNode.forward != null )
        {
            curNode = curNode.forward;
        }

        return curNode;
    }

    public LevelNode Forward
    {
        get { return forward; }
        set => forward = value;
    }

    public void AddAction( Action newAction )
    {
        locationActions.Add( newAction );
    }

    public List<Action> GetActions()
    {
        return locationActions;
    }
}
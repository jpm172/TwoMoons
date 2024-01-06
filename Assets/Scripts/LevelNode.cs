using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelNode
{
    [SerializeField] public string name;
    public bool isLocation;

    public Sprite sprite;

    public LevelNode next, previous;
    
    public LevelNode( string newName, bool newIsLocation, Sprite newSprite )
    {
        name = newName;
        isLocation = newIsLocation;
        sprite = newSprite;
    }
    
    
    public bool isPathNode()
    {
        return !isLocation;
    }
}

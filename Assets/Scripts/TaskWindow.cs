using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskWindow : MonoBehaviour
{
    private GameManager game;
    public GameObject taskContainer;
    //public List<Task> taskList;
    
    private TextMeshProUGUI[] textSlots;
    // Start is called before the first frame update
    void Start()
    {
        textSlots = taskContainer.GetComponentsInChildren<TextMeshProUGUI>();
        game = GameObject.FindWithTag( "Game Manager" ).GetComponent<GameManager>();
    }

    private void ResetText()
    {
        foreach ( TextMeshProUGUI tmp in textSlots )
        {
            tmp.text = "";
            tmp.alpha = 1;
            tmp.fontStyle = FontStyles.Normal;
        }
    }

    private void UpdateText()
    {
        for ( int i = 0; i < game.Tasks.Count; i++ )
        {
            textSlots[i].text = game.Tasks[i].actionDescription;
        }
    }

    public void UpdateTasks()
    {
        for ( int i = 0; i < game.Tasks.Count; i++ )
        {
            if ( !game.Tasks[i].IsAvailable )
            {
                textSlots[i].fontStyle = FontStyles.Strikethrough;
                textSlots[i].alpha = .5f;
            }
        }
        
        //TODO: play scribbling-out noise
    }
    
    
    
    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnomalyFinderGame : Action
{
    public AudioClip noiseSound;
    public GameObject anomalyObject, scrollArea, playArea;

    
    private AudioSource audio;
    
    private GameObject spawnedAnomaly;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = noiseSound;
        audio.loop = true;
        audio.playOnAwake = false;
        
    }

    private void Update()
    {
        if ( spawnedAnomaly != null )
        {
            SoundEffects();
        }
    }


    private void SoundEffects()
    {
        RectTransform playAreaRect = playArea.GetComponent<RectTransform>();
        
        float dist =   Vector2.Distance(Vector2.zero, scrollArea.transform.localPosition + spawnedAnomaly.transform.localPosition);
        
        if ( playAreaRect.GetWorldRect().Contains( spawnedAnomaly.transform.position ) )
        {
            audio.volume = 1;
        }
        else
        {
            audio.volume = .3f;
        }
    }

    public override void StartAction()
    {
        SetWindowActive( true );
        audio.Play();
        spawnedAnomaly = Instantiate( anomalyObject, scrollArea.transform.position, Quaternion.identity, scrollArea.transform );
        //spawnedAnomaly.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        
    }

    public void SubmitGame()
    {

        if ( spawnedAnomaly == null )
            return;
        
        //Debug.Log( "submit anomaly" );
        RectTransform playAreaRect = playArea.GetComponent<RectTransform>();
        RectTransform anomalyRect = spawnedAnomaly.GetComponent<RectTransform>();
        
        if ( playAreaRect.GetWorldRect().Contains( spawnedAnomaly.transform.position ) )
        {
            Debug.Log( "True" );
            CompleteAction();
        }
        
    }
    
    //Enables/disables the canvas and interactability to show/hide the window
    private void SetWindowActive(bool value)
    {
        GetComponent<Canvas>().enabled = value;
        GetComponent<CanvasGroup>().interactable = value;
    }
    
    public override void CompleteAction()
    {
        SetUpdateAvailability( false );
        SetWindowActive( false );
    }
}

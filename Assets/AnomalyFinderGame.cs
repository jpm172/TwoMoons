using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AnomalyFinderGame : Action
{
    public AudioClip noiseSound;
    public GameObject anomalyObject, scrollArea, playArea;

    public float threshold, minVolume;
    
    private AudioSource audio;
    
    private GameObject spawnedAnomaly;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = noiseSound;
        audio.loop = true;
        audio.playOnAwake = false;
        //SetWindowActive( false );
        
    }

    private void Update()
    {
        if ( spawnedAnomaly != null )
        {
            SoundEffects();
        }
    }


    //TODO add secondary clicking noise when getting really close to the anomaly
    //Adjusts the volume of the sound effects according to how close the anomaly is to the play area
    private void SoundEffects()
    {
        //calculate the distance between the anomaly and the center of the play area
        float dist = Vector2.Distance(Vector2.zero, scrollArea.transform.localPosition + spawnedAnomaly.transform.localPosition);
        dist = Mathf.Max( dist, 1 );

        //calculate how loud the noise should be according to the distance
        float progress =  1 - ( dist / threshold );
        float volume = ExpSmoothingCurve( 7, progress );

        audio.volume = Mathf.Max( minVolume,  volume);
    }

    public override void StartAction()
    {
        Rect scrollAreaRect = scrollArea.GetComponent<RectTransform>().rect;
        
        SetWindowActive( true );
        audio.Play();

        if ( spawnedAnomaly != null )
        {
            Destroy( spawnedAnomaly );
        }
        
        //randomly spawn an anomaly within the scroll area, while keeping a buffer zone from the edge of the scroll area
        float xRange = ( scrollAreaRect.width * .9f ) / 2;
        float yRange = ( scrollAreaRect.height * .9f ) / 2;
        float randX = UnityEngine.Random.Range( -xRange, xRange );
        float randY = UnityEngine.Random.Range( -yRange, yRange );
        
        Vector3 randomPos = new Vector2(randX, randY);
        spawnedAnomaly = Instantiate( anomalyObject, scrollArea.transform.position + randomPos, Quaternion.identity, scrollArea.transform );
        
    }

    public void SubmitGame()
    {
        if ( spawnedAnomaly == null )
            return;
        
        RectTransform playAreaRect = playArea.GetComponent<RectTransform>();

        if ( playAreaRect.GetWorldRect().Contains( spawnedAnomaly.transform.position ) )
        {
            CompleteAction();
        }
        
    }

    //Interpolates between 0 and 1 using an exponential curve
    //steepness: defines how "steep" or extreme the curve is between x = 0 and x = 1
    //x: the position that is being sampled from, only in the ranges of [0, 1]
    private float ExpSmoothingCurve( float steepness, float x )
    {
        steepness = Mathf.Max( steepness, 2 );
        x = Mathf.Clamp01( x );
        return ( Mathf.Pow( steepness, x ) - 1 ) / ( steepness - 1 );
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
        StartCoroutine( FadeAudioOut(1) );
    }

    //Makes the audio source lower its current volume -> 0 over a set amount of time, then stops the audio
    //This prevents the clicking noise when shutting off a clip
    private IEnumerator FadeAudioOut(float time)
    {
        time = Mathf.Max( time, .1f );
        float timer = 0;
        float curVolume = audio.volume;
        
        while ( timer < 1 )
        {
            audio.volume = curVolume - ExpSmoothingCurve( 7, timer );
            timer += Time.deltaTime * (1/time);
            yield return null;
        }

        audio.Stop();
    }
}

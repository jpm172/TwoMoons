using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AnomalyFinderGame : Action
{
    public AudioClip noiseSound, clickingSound;
    public GameObject anomalyObject, scrollArea, playArea;

    public float mainThreshold, clickingThreshold, minVolume;
    
    private AudioSource audio1, audio2;
    
    private GameObject spawnedAnomaly;

    private Settings settings;

    
    
    void Start()
    {
        settings = GetComponentInParent<Settings>();
        
        //add audio sources for each clip that needs to be played
        audio1 = gameObject.AddComponent<AudioSource>();
        audio1.clip = noiseSound;
        audio1.loop = true;
        audio1.playOnAwake = false;
        
        audio2 = gameObject.AddComponent<AudioSource>();
        audio2.clip = clickingSound;
        audio2.loop = true;
        audio2.playOnAwake = false;
        
        SetWindowActive( false );
        
    }

    private void Update()
    {
        if ( spawnedAnomaly != null )
        {
            SoundEffects();
        }

    }


    //TODO: add secondary clicking noise when getting really close to the anomaly
    //BUG: the position of the anomaly doesnt appear to be calculated correctly for distance calculations
    //Adjusts the volume of the sound effects according to how close the anomaly is to the play area
    private void SoundEffects()
    {
        Vector2 vec = scrollArea.transform.localPosition  + (spawnedAnomaly.transform.localPosition);
        //calculate the distance between the anomaly and the center of the play area
        float dist = Vector2.Distance(Vector2.zero, vec);

        //calculate how loud the noise should be according to the distance
        float mainProgress =  1 - ( dist / mainThreshold );
        float mainVolume = ExpSmoothingCurve( 7, mainProgress );
        
        float clickingProgress = 1 - ( dist / clickingThreshold );
        float clickingVolume = ExpSmoothingCurve( 10, clickingProgress );

        audio1.volume = settings.SfxVolume * settings.MasterVolume * Mathf.Max( minVolume,  mainVolume);//main noise
        audio2.volume = settings.SfxVolume * settings.MasterVolume * Mathf.Max( 0, clickingVolume );//clicking
    }

    public override void StartAction() 
    {
        Rect scrollAreaRect = scrollArea.GetComponent<RectTransform>().rect;
        
        SetWindowActive( true );
        
        audio1.Play();
        audio2.Play();

        if ( spawnedAnomaly != null )
        {
            Destroy( spawnedAnomaly );
        }


        //randomly spawn an anomaly within the scroll area, while keeping a buffer zone from the edge of the scroll area
        float xRange = ( scrollAreaRect.width * .8f ) / 2;
        float yRange = ( scrollAreaRect.height * .8f ) / 2;
        float randX = UnityEngine.Random.Range( -xRange, xRange );
        float randY = UnityEngine.Random.Range( -yRange, yRange );
        
        Vector3 randomPos = new Vector2(randX, randY);
        spawnedAnomaly = Instantiate( anomalyObject, scrollArea.transform.position, Quaternion.identity, scrollArea.transform );
        spawnedAnomaly.transform.localPosition = randomPos;
        
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
        float curVolume = audio1.volume;
        
        while ( timer < 1 )
        {
            float newVolume = curVolume - ExpSmoothingCurve( 7, timer );
            audio1.volume = newVolume;
            audio2.volume = newVolume;
            
            timer += Time.deltaTime * (1/time);
            yield return null;
        }

        audio1.Stop();
        audio2.Stop();
    }
}

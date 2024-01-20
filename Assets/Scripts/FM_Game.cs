using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class FM_Game : MonoBehaviour
{

    [Range(1,10000)]  //Creates a slider in the inspector
    public float frequency;

    [Range(0,1)]  //Creates a slider in the inspector
    public float volume = .5f;
    
    public float sampleRate = 44100;

    private AudioSource audio;
    private int timeIndex = 0;
   [SerializeField] private float phase = 0;
    
    
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.spatialBlend = 0;
        audio.Stop(); //avoids audiosource from starting to play automatically
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(!audio.isPlaying)
            {
                timeIndex = 0;  //resets timer before playing sound
                audio.Play();
            }
            else
            {
                audio.Stop();
            }
        }
    }
    
    
    
    
    void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i = 0 ; i < data.Length ; i += channels)
        {  
            //increase phase by a ratio of 2 pi and the frequency/sample rate, then using modulo to keep it within [0, 2pi]
            phase = (phase + (2 * Mathf.PI * frequency / sampleRate)) % (2 * Mathf.PI);

            float waveData = SineWave( phase )*SquareWave( phase );
            
            data[i] = waveData * volume;
            if(channels == 2)
                data[i+1] =  waveData*volume;
            
        }
    }
    
    
    //Creates a sinewave
    public float SineWave(float phase)
    {
        return Mathf.Sin(phase  + Mathf.PerlinNoise( phase,phase ));
    }

    public float SquareWave( float phase )
    {
        return Mathf.Sign( Mathf.Sin( phase + Mathf.PerlinNoise( phase, phase ) ) );
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Settings : MonoBehaviour
{
    [SerializeField]private float masterVolume, musicVolume, sfxVolume;
    public Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider;
    [SerializeField]private bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
    }
    

    public void UpdateMasterVolume()
    {
        masterVolume = masterVolumeSlider.value;
    }
    
    public void UpdateMusicVolume()
    {
        musicVolume = musicVolumeSlider.value;
    }
    
    public void UpdateSfxVolume()
    {
        sfxVolume = sfxVolumeSlider.value;
    }

    //saves the player's settings to a json file
    public void SaveSettings()
    {
        Debug.Log( "Save" );
        string saveFile = Application.persistentDataPath + "/Settings.json";
        
        Config c = new Config( masterVolume, musicVolume, sfxVolume );
        string jsonContents = JsonUtility.ToJson( c );
        File.WriteAllText(saveFile, jsonContents);
    }

    //loads the player's settings from the file and sets the ingame values to match
    public void LoadSettings()
    {
        Debug.Log( "Load" );
        string saveFile = Application.persistentDataPath + "/Settings.json";
        
        if ( File.Exists( saveFile ) )
        {
            string fileContents = File.ReadAllText( saveFile );
            Config c = JsonUtility.FromJson<Config>(fileContents);

            masterVolumeSlider.value = c.MasterVolume;
            musicVolumeSlider.value = c.MusicVolume;
            sfxVolumeSlider.value = c.sfxVolume;
        }
    }

    public float MasterVolume => masterVolume;

    public float MusicVolume => musicVolume;

    public float SfxVolume => sfxVolume;

    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }
}


[System.Serializable]
public class Config
{
    public float MasterVolume;
    public float MusicVolume;
    public float sfxVolume;

    public Config(float newMasterVol, float newMusicVol, float newSfxVol)
    {
        MasterVolume = newMasterVol;
        MusicVolume = newMusicVol;
        sfxVolume = newSfxVol;
    }
}

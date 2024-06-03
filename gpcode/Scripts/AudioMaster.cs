using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalConstantValues;
using UnityEditor;

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Masters/Audio Master")]
public class AudioMaster : MonoBehaviour
{
    #region Variable Declaration
    // Audio Source
    private AudioSource audioSource;

    // Clips
    [Tooltip("List of music to play during runtime.")]
    [SerializeField] private AudioClip[] music;
    [Space(5)]
    
    [Tooltip("Sound when player dies.")]
    [SerializeField] private AudioClip deathClip;

    // Masters
    private GameMaster gameMaster;
    private UIMaster uiMaster;
    #endregion

    #region Initialization
    // Start is called before the first frame update
    void Start() => Initialize();

    //Method to initialize masters used, and the AudioSource Component
    void Initialize()
    {
        gameMaster = GlobalMasterCreationReadonly.GameMaster;
        uiMaster = GlobalMasterCreationReadonly.UiMaster;
        audioSource = GetComponent<AudioSource>();
    }
    #endregion

    #region Update Methods
    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying) PlayRandomMusic();  //Checks if there is no music playing, and then plays and random music clip
    }

    //Method to select a random clip of music
    void PlayRandomMusic() => PlayMusicClip(Random.Range(0, music.Length));

    //Plays a music clip based off of the index given
    void PlayMusicClip(int index)
    {
        audioSource.clip = music[index];
        audioSource.Play();
    }
    #endregion

    #region Audio Control Methods
    //Method to change the volume of the audioSource
    public void ChangeVolume(float volume) => audioSource.volume = volume;

    //TODO          Plays the death sound assigned
    public void PlayDeathSound()
    {
        audioSource.clip = deathClip;
        audioSource.Play();
    }
    #endregion
}

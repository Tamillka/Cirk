using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Dropdown musicDropdown; // Reference to the Dropdown UI for music selection
    [SerializeField] private AudioClip[] musicTracks; // Array of possible music tracks

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Load and set the previously selected music track, if any
        if (PlayerPrefs.HasKey("MusicTrack"))
        {
            int savedTrackIndex = PlayerPrefs.GetInt("MusicTrack");
            audioSource.clip = musicTracks[savedTrackIndex];
            audioSource.Play();
            musicDropdown.value = savedTrackIndex;
        }
        else
        {
            // Default to the first track if no music is saved
            audioSource.clip = musicTracks[0];
            audioSource.Play();
            musicDropdown.value = 0;
        }

        // Add listener to the dropdown to change the music when a new option is selected
        musicDropdown.onValueChanged.AddListener(ChangeMusic);
    }

    // Method to handle music change based on the dropdown value
    public void ChangeMusic(int index)
    {
        // Set the new music track
        audioSource.clip = musicTracks[index];
        audioSource.Play();

        // Save the selected track index to PlayerPrefs
        PlayerPrefs.SetInt("MusicTrack", index);
    }

    private void OnDestroy()
    {
        // Clean up the listener when the object is destroyed
        musicDropdown.onValueChanged.RemoveListener(ChangeMusic);
    }
}

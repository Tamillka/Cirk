using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Dropdown musicDropdown; // Reference to the Dropdown UI for music selection (only used in settings)
    [SerializeField] private AudioClip[] musicTracks; // Array of possible music tracks

    private AudioSource audioSource;
    private static MusicManager instance;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Load saved music track or use default
        int savedTrackIndex = PlayerPrefs.GetInt("MusicTrack", 0);
        audioSource.clip = musicTracks[savedTrackIndex];
        audioSource.Play();

        // If the dropdown is assigned (only in settings scene), set its value and add listener
        if (musicDropdown != null)
        {
            musicDropdown.value = savedTrackIndex;
            musicDropdown.onValueChanged.AddListener(ChangeMusic);
        }
    }

    public void ChangeMusic(int index)
    {
        audioSource.clip = musicTracks[index];
        audioSource.Play();
        PlayerPrefs.SetInt("MusicTrack", index);
    }

    private void OnDestroy()
    {
        if (musicDropdown != null)
        {
            musicDropdown.onValueChanged.RemoveListener(ChangeMusic);
        }
    }
}
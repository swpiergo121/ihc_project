using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI; // Required for UI elements like Text/Image
using TMPro;      // Required if using TextMeshPro

public class VideoControlManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    // Assign the Text/TextMeshPro component from the Play/Pause Button's child
    public TextMeshProUGUI playPauseButtonText; // Use 'Text' if not using TMP
    // or public Text playPauseButtonText; 

    private const string PLAY_SYMBOL = "▶️";
    private const string PAUSE_SYMBOL = "⏸️";

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found on this GameObject!");
        }

        // Initialize the button symbol based on the initial state (assuming initially stopped/paused)
        UpdatePlayPauseSymbol();
    }

    /// <summary>
    /// Toggles the video state between Play and Pause. This function is for the UI Button.
    /// </summary>
    public void TogglePlayPause()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Video Paused.");
        }
        else
        {
            // If the video is at the end, Play() will restart it.
            videoPlayer.Play();
            Debug.Log("Video Playing.");
        }

        UpdatePlayPauseSymbol();
    }

    /// <summary>
    /// Updates the text symbol on the UI Button based on the VideoPlayer's current state.
    /// </summary>
    private void UpdatePlayPauseSymbol()
    {
        if (playPauseButtonText == null || videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            playPauseButtonText.text = PAUSE_SYMBOL; // Set to Pause symbol (⏸️)
        }
        else
        {
            playPauseButtonText.text = PLAY_SYMBOL;  // Set to Play symbol (▶️)
        }
    }

    /// <summary>
    /// Seeks to a specific moment (in seconds).
    /// </summary>
    /// <param name="targetTime">The time in seconds to jump to.</param>
    public void SeekToMoment(float targetTime)
    {
        // ... (Keep the SeekToMoment implementation from the previous answer)
        // Ensure you call videoPlayer.Play() after setting time if it was paused
        if (videoPlayer != null)
        {
            if (targetTime >= 0f && targetTime <= videoPlayer.length)
            {
                videoPlayer.time = targetTime;

                if (!videoPlayer.isPlaying)
                {
                    videoPlayer.Play();
                }
                Debug.Log($"Video set to time: {targetTime} seconds.");
            }
            UpdatePlayPauseSymbol(); // Update symbol after seeking and playing
        }
    }
}
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI; // Required for UI elements like Text/Image
using TMPro;      // Required if using TextMeshPro
using UnityEngine.EventSystems; // Required for Event Trigger interfaces

public class VideoControlManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private VideoPlayer videoPlayer;

    public TextMeshProUGUI playPauseButtonText;
    public Slider videoProgressBar; // Reference to your UI Slider

    private const string PLAY_SYMBOL = "▶️";
    private const string PAUSE_SYMBOL = "⏸️";

    private bool isSeeking = false; // Flag to prevent conflicting updates during user seek

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found on this GameObject!");
        }

        // Initialize the button symbol based on the initial state (assuming initially stopped/paused)
        UpdatePlayPauseSymbol();

        // Initialize slider if assigned
        if (videoProgressBar != null)
        {
            videoProgressBar.minValue = 0;
            // Max value will be set once video is prepared
            // We will NOT add a listener here for onValueChanged,
            // instead we'll use IPointerDownHandler and IPointerUpHandler
            // to manage the seeking state and then directly call SeekToMoment
            // from the slider's On Value Changed event in the Inspector.
        }
    }

    void Start()
    {
        // Ensure video is prepared before setting slider max value
        videoPlayer.prepareCompleted += OnVideoPrepared;
        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare(); // Start preparing if not already
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        if (videoProgressBar != null)
        {
            videoProgressBar.maxValue = (float)vp.length;
            Debug.Log($"Video prepared. Slider max value set to: {videoProgressBar.maxValue}");
        }
    }

    void Update()
    {
        // Only update slider if video is playing and user is not currently dragging it
        if (videoPlayer != null && videoPlayer.isPlaying && videoProgressBar != null && !isSeeking)
        {
            videoProgressBar.value = (float)videoPlayer.time;
        }
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
        if (playPauseButtonText == null)
        {
            // If this happens, it means the assignment is still incorrect despite your check.
            // You won't see this error on device, but it's a final safeguard.
            Debug.LogError("playPauseButtonText is NOT assigned in the Inspector!");
            return;
        }
        if (videoPlayer == null)
        {
            // This shouldn't happen if the script is on the same GameObject as VideoPlayer.
            Debug.LogError("VideoPlayer is NULL in UpdatePlayPauseSymbol!");
            return;
        }

        // Check if the video is actually playing or if it's paused/stopped
        if (videoPlayer.isPlaying)
        {
            if (playPauseButtonText.text != PAUSE_SYMBOL)
            {
                playPauseButtonText.text = PAUSE_SYMBOL; // Set to Pause symbol (⏸️)
            }
        }
        else // Video is paused or stopped
        {
            if (playPauseButtonText.text != PLAY_SYMBOL)
            {
                playPauseButtonText.text = PLAY_SYMBOL;  // Set to Play symbol (▶️)
            }
        }
    }


    /// <summary>
    /// Seeks to a specific moment (in seconds).
    /// </summary>
    /// <param name="targetTime">The time in seconds to jump to.</param>
    public void SeekToMoment(float targetTime)
    {
        if (videoPlayer != null)
        {
            // Clamp targetTime to valid range
            targetTime = Mathf.Clamp(targetTime, 0f, (float)videoPlayer.length);
            videoPlayer.time = targetTime;

            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
            // Debug.Log($"Video set to time: {targetTime} seconds."); // Removed for less spam
            UpdatePlayPauseSymbol(); // Update symbol after seeking and playing
        }
    }

    // --- New Methods for Slider Interaction ---

    /// <summary>
    /// Called when the user starts dragging the slider.
    /// This is part of the IPointerDownHandler interface.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (videoProgressBar != null && eventData.pointerPress == videoProgressBar.gameObject)
        {
            isSeeking = true;
            Debug.Log("Slider drag started.");
        }
    }

    /// <summary>
    /// Called when the user releases the slider handle.
    /// This is part of the IPointerUpHandler interface.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (videoProgressBar != null && eventData.pointerPress == videoProgressBar.gameObject)
        {
            isSeeking = false;
            Debug.Log("Slider released. Resuming normal updates.");
        }
    }

    /// <summary>
    /// This function should be hooked to the Slider's "On Value Changed" event in the Inspector.
    /// </summary>
    public void OnVideoSliderChanged(float value)
    {
        if (videoPlayer == null) return;
        SeekToMoment(value);
    }
}

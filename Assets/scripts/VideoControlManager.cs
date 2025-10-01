using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI; // Required for UI elements like Text/Image
using TMPro;      // Required if using TextMeshPro

public class VideoControlManager : MonoBehaviour
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
            videoProgressBar.onValueChanged.AddListener(OnSliderValueChanged);
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
        if (videoPlayer != null)
        {
            // Clamp targetTime to valid range
            targetTime = Mathf.Clamp(targetTime, 0f, (float)videoPlayer.length);
            videoPlayer.time = targetTime;

            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
            Debug.Log($"Video set to time: {targetTime} seconds.");
            UpdatePlayPauseSymbol(); // Update symbol after seeking and playing
        }
    }

    /// <summary>
    /// Called when the slider's value changes (user drags it).
    /// </summary>
    public void OnSliderValueChanged(float value)
    {
        if (videoPlayer == null) return;

        // Set the seeking flag to true when the user starts dragging
        // This prevents the Update() method from overriding the slider's value
        // while the user is actively seeking.
        isSeeking = true;
        SeekToMoment(value);
    }

    /// <summary>
    /// Call this function when the user releases the slider handle.
    /// This can be hooked up to the Slider's "On Pointer Up" event (requires Event Trigger).
    /// </summary>
    public void OnSliderPointerUp()
    {
        isSeeking = false; // Reset the seeking flag
        Debug.Log("Slider released. Resuming normal updates.");
    }
}

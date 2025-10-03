using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VideoControlManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private VideoPlayer videoPlayer;

    public TextMeshProUGUI playPauseButtonText;
    public Slider videoProgressBar;

    private const string PLAY_SYMBOL = "▶️";
    private const string PAUSE_SYMBOL = "⏸️";

    private bool isSeeking = false; // Flag to prevent conflicting updates during user seek
    private bool wasPlayingBeforeSeek = false; // To remember video state before seeking

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found on this GameObject!");
        }

        UpdatePlayPauseSymbol();

        if (videoProgressBar != null)
        {
            videoProgressBar.minValue = 0;
        }
    }

    void Start()
    {
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoLoopPointReached;

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
        }
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoLoopPointReached;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        if (videoProgressBar != null)
        {
            videoProgressBar.maxValue = (float)vp.length;
            Debug.Log($"Video prepared. Slider max value set to: {videoProgressBar.maxValue}");
        }
        UpdatePlayPauseSymbol();
    }

    void OnVideoLoopPointReached(VideoPlayer vp)
    {
        Debug.Log("Video finished playing.");
        UpdatePlayPauseSymbol();
        if (videoProgressBar != null)
        {
            videoProgressBar.value = (float)vp.length;
        }
    }

    void Update()
    {
        // Always update the play/pause symbol in Update to reflect the current state
        UpdatePlayPauseSymbol();

        // **SLIDER PROGRESS UPDATE LOGIC**
        // Only update slider if video is playing AND user is NOT currently dragging it
        if (videoPlayer != null && videoPlayer.isPlaying && videoProgressBar != null && !isSeeking)
        {
            videoProgressBar.value = (float)videoPlayer.time;
        }
        // If the video is paused and not seeking, the slider should stay where it is.
        // If the video is paused and seeking, the slider is controlled by user input.
    }

    public void TogglePlayPause()
    {
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            Debug.Log("Video Paused by button.");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("Video Playing by button.");
        }
        // UpdatePlayPauseSymbol() will be called by Update()
    }

    private void UpdatePlayPauseSymbol()
    {
        if (playPauseButtonText == null)
        {
            Debug.LogError("playPauseButtonText is NOT assigned in the Inspector!");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is NULL in UpdatePlayPauseSymbol!");
            return;
        }

        if (videoPlayer.isPlaying)
        {
            if (playPauseButtonText.text != PAUSE_SYMBOL)
            {
                playPauseButtonText.text = PAUSE_SYMBOL;
            }
        }
        else
        {
            if (playPauseButtonText.text != PLAY_SYMBOL)
            {
                playPauseButtonText.text = PLAY_SYMBOL;
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

            // **RESTORE PLAY STATE AFTER SEEKING**
            // If the video was playing before the seek, and it's currently paused, resume it.
            if (wasPlayingBeforeSeek && !videoPlayer.isPlaying && videoPlayer.isPrepared)
            {
                videoPlayer.Play();
                Debug.Log("SeekToMoment: Resuming play after seek.");
            }
            // If the video was paused before the seek, and it's currently playing (e.g., from a brief play call), pause it.
            else if (!wasPlayingBeforeSeek && videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                Debug.Log("SeekToMoment: Pausing after seek (was paused before).");
            }
        }
    }

    // --- Methods for Slider Interaction ---

    /// <summary>
    /// Called when the user starts dragging the slider.
    /// This is part of the IPointerDownHandler interface.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // **SLIDER DRAG START LOGIC**
        // Check if the pointerPress is the slider itself OR a child of the slider (like the handle)
        if (videoProgressBar != null && (eventData.pointerPress == videoProgressBar.gameObject ||
            (eventData.pointerPress != null && eventData.pointerPress.transform.IsChildOf(videoProgressBar.transform))))
        {
            isSeeking = true; // User is now dragging the slider
            Debug.Log("Slider drag started.");
            // Remember if the video was playing before we paused it for seeking
            wasPlayingBeforeSeek = videoPlayer.isPlaying;
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause(); // Pause video while dragging for smoother seeking
                Debug.Log("Slider drag: Video paused.");
            }
        }
    }

    /// <summary>
    /// Called when the user releases the slider handle.
    /// This is part of the IPointerUpHandler interface.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        // **SLIDER DRAG END LOGIC**
        // Check if the pointerPress is the slider itself OR a child of the slider (like the handle)
        if (videoProgressBar != null && (eventData.pointerPress == videoProgressBar.gameObject ||
            (eventData.pointerPress != null && eventData.pointerPress.transform.IsChildOf(videoProgressBar.transform))))
        {
            isSeeking = false; // User has released the slider
            Debug.Log("Slider released. Resuming normal updates.");
            // Resume video playback only if it was playing before the seek started
            if (wasPlayingBeforeSeek && videoPlayer.isPrepared && !videoPlayer.isPlaying)
            {
                videoPlayer.Play();
                Debug.Log("Slider released: Video resumed playing.");
            }
            // Reset the flag
            wasPlayingBeforeSeek = false;
        }
    }

    /// <summary>
    /// This function should be hooked to the Slider's "On Value Changed" event in the Inspector.
    /// </summary>
    public void OnVideoSliderChanged(float value)
    {
        if (videoPlayer == null) return;
        // **CONDITIONAL SEEKING LOGIC**
        // ONLY seek if the user is actively dragging the slider (i.e., isSeeking is true).
        // This prevents the Update loop's programmatic slider updates from triggering seeks.
        if (isSeeking)
        {
            SeekToMoment(value);
            Debug.Log($"Slider On Value Changed (seeking): {value}");
        }
    }
}

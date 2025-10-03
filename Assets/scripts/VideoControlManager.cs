using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class VideoControlManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    public Button button;
    public Sprite startSprite;
    public Sprite stopSprite;

    private const string PLAY_SYMBOL = "▶️";
    private const string PAUSE_SYMBOL = "⏸️";

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer component not found on this GameObject!");
        }

        UpdatePlayPauseSymbol();
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
        UpdatePlayPauseSymbol();
    }

    void OnVideoLoopPointReached(VideoPlayer vp)
    {
        Debug.Log("Video finished playing.");
        UpdatePlayPauseSymbol();
    }

    void Update()
    {
        // Always update the play/pause symbol in Update to reflect the current state
        UpdatePlayPauseSymbol();
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
        if (button.image == null)
        {
            Debug.LogError("buttonImage is NOT assigned in the Inspector!");
            return;
        }
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer is NULL in UpdatePlayPauseSymbol!");
            return;
        }

        if (videoPlayer.isPlaying)
        {
            button.image.sprite = stopSprite;
        }
        else
        {
            button.image.sprite = startSprite;
        }
    }

}

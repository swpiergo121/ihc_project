using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class Progress : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer videoPlayer;

[SerializeField]
    private Camera arCamera; // Add a field for your AR Camera

    private Image progress;

    private void Awake() {
        progress = GetComponent<Image>();
        if (arCamera == null)
        {
            // Attempt to find the main camera if not assigned.
            // In an AR scene, this is usually the AR camera.
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("AR Camera not assigned and Camera.main not found! UI interactions might be incorrect.");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (videoPlayer.frameCount > 0) {
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        }
        
    }

    public void OnDrag(PointerEventData eventData) {
        TrySkip(eventData);
    }

    public void OnPointerDown(PointerEventData eventData) { // Added: Implementation for IPointerDownHandler
        TrySkip(eventData);
    }


    private void TrySkip(PointerEventData eventData) {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(progress.rectTransform, eventData.position,
                                                                    arCamera, out localPoint)) {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax,
                                          localPoint.x);
            SkipToPercent(pct);
        }
    }

    private void SkipToPercent(float pct) {
        var frame = videoPlayer.frameCount * pct;
        videoPlayer.frame = (long)frame;
    }
}

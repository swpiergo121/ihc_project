using UnityEngine;
using Vuforia;
using System.Collections.Generic;

public class ImageTargetContentManager : MonoBehaviour
{
    // A list to hold all your Image Targets and their associated content
    public List<ImageTargetInfo> imageTargetInfos = new List<ImageTargetInfo>();

    // Mark this class as Serializable so Unity can display it in the Inspector
    [System.Serializable]
    public class ImageTargetInfo
    {
        public ImageTargetBehaviour imageTarget;
        public GameObject contentToShow; // The specific content for this target
    }

    void Start()
    {
        // Register for Vuforia's tracking events
        VuforiaBehaviour.Instance.World.OnObserverStatusChanged += OnObserverStatusChanged;

        // Initially hide all content
        foreach (var info in imageTargetInfos)
        {
            if (info.contentToShow != null)
            {
                info.contentToShow.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
        // Unregister to prevent memory leaks
        if (VuforiaBehaviour.Instance != null && VuforiaBehaviour.Instance.World != null)
        {
            VuforiaBehaviour.Instance.World.OnObserverStatusChanged -= OnObserverStatusChanged;
        }
    }

    private void OnObserverStatusChanged(ObserverBehaviour observer, ObserverStatus newStatus)
    {
        // Check if the observer is an ImageTargetBehaviour
        ImageTargetBehaviour currentImageTarget = observer as ImageTargetBehaviour;

        if (currentImageTarget != null)
        {
            // Iterate through all registered image targets
            foreach (var info in imageTargetInfos)
            {
                if (info.imageTarget == currentImageTarget)
                {
                    // This is the currently tracked target
                    if (newStatus == ObserverStatus.TRACKED || newStatus == ObserverStatus.EXTENDED_TRACKED)
                    {
                        // Activate its content
                        if (info.contentToShow != null)
                        {
                            info.contentToShow.SetActive(true);
                            Debug.Log($"Content for {info.imageTarget.TargetName} activated.");
                        }
                    }
                    else
                    {
                        // Deactivate its content if it loses tracking
                        if (info.contentToShow != null)
                        {
                            info.contentToShow.SetActive(false);
                            Debug.Log($"Content for {info.imageTarget.TargetName} deactivated.");
                        }
                    }
                }
                else
                {
                    // For all other image targets, ensure their content is deactivated
                    if (info.contentToShow != null)
                    {
                        info.contentToShow.SetActive(false);
                    }
                }
            }
        }
    }
}

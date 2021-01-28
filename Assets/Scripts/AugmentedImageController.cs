using GoogleARCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentedImageController : MonoBehaviour
{
    [SerializeField] private AugmentedImageVisulaizer _augmentedImageVisualizer;

    private readonly Dictionary<int, AugmentedImageVisulaizer> _visualizers =
        new Dictionary<int, AugmentedImageVisulaizer>();

    private readonly List<AugmentedImage> _images = new List<AugmentedImage>();

    void Update()
    {
        if(Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        Session.GetTrackables(_images, TrackableQueryFilter.Updated);
        VisualizeTrackables();
    }

    private void VisualizeTrackables()
    {
        foreach(var image in _images)
        {
            var visualizer = GetVisualizer(image);

            if(image.TrackingState == TrackingState.Tracking && visualizer == null)
            {
                AddVisualizer(image);
            }
            else if(image.TrackingState == TrackingState.Stopped && visualizer != null)
            {
                RemoveVisualizer(image, visualizer);
            }
        }
    }

    private void RemoveVisualizer(AugmentedImage image, AugmentedImageVisulaizer visualizer)
    {
        _visualizers.Remove(image.DatabaseIndex);
        Destroy(visualizer.gameObject);
    }

    private void AddVisualizer(AugmentedImage image)
    {
        var anchor = image.CreateAnchor(image.CenterPose);
        var visualizer = Instantiate(_augmentedImageVisualizer, anchor.transform);
        visualizer.Image = image;
        _visualizers.Add(image.DatabaseIndex, visualizer);
    }

    private AugmentedImageVisulaizer GetVisualizer(AugmentedImage image)
    {
        AugmentedImageVisulaizer visulaizer;
        _visualizers.TryGetValue(image.DatabaseIndex, out visulaizer);
        return visulaizer;
    }
}

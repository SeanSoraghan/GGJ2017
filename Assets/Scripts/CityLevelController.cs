using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityLevelController : OSCReciever
{
    public GameObject WindowsCollectionObject;
    public GameObject AudioLayersObject;

    private List<AudioSource> AudioLayers = new List<AudioSource>();
    private List<GameObject> Windows      = new List<GameObject>();
    private List<float> MaxLevels         = new List<float>();
    private List<float> LevelTargets      = new List<float>();

    public float TimeBetweenSwitches = 0.5f;

    public float BeginningVolumeThreshold = 0.1f;
    public float FinalVolumeThreshold     = 0.9f;
    public int NumWindowsPerLevel         = 4;
    public int NumWindowsPerAudioLayer    = 8;

    private float OverThresholdTime  = 0.0f;
    private float UnderThresholdTime = 0.0f;
    private float ThresholdIncrease  = 0.2f;

    private int WindowIndex = 0;

    private float VolumeDistanceThreshold = 0.05f;

    protected override void InitialiseLevel()
    {
        if (WindowsCollectionObject != null)
            foreach (Transform child in WindowsCollectionObject.transform)
                Windows.Add (child.gameObject);

        int NumLevels = Windows.Count / NumWindowsPerLevel;
        ThresholdIncrease = (FinalVolumeThreshold - BeginningVolumeThreshold) / NumLevels;

        AudioSegmenter.VolumeThreshold = BeginningVolumeThreshold;

        if (AudioLayersObject != null)
        { 
            foreach (AudioSource source in AudioLayersObject.GetComponents<AudioSource>())
            { 
                AudioLayers.Add  (source);
                MaxLevels.Add    (source.volume);
                LevelTargets.Add (0.0f);
                source.volume = 0.0f;
            }
        }

        DeactivateWindows();
    }

    void SetWindowActive (int windowIndex, bool active)
    {
        SpriteRenderer windowRenderer =  Windows[windowIndex].GetComponent<SpriteRenderer>();
        if (windowRenderer != null)
            windowRenderer.enabled = active;
    }

    void DeactivateWindows()
    {
        for (int i = 0; i < Windows.Count; ++i)
            SetWindowActive (i, false);
    }

    public override void MapFeaturesToVisualisers()
    {
        float rms = osc.Feature (AudioFeature.RMS);

        if (AudioGesturePlaying)
        {
            OverThresholdTime += Time.deltaTime;
            if (OverThresholdTime > TimeBetweenSwitches)
            {
                IlluminateNextWindow();
                OverThresholdTime = 0.0f;
            }
            if (AudioSegmenter.CheckGestureEnd (ref osc, Time.deltaTime))
                AudioGestureEnded();
        }
        else
        { 
            UnderThresholdTime += Time.deltaTime;
            if (UnderThresholdTime > TimeBetweenSwitches)
            {
                TurnOffLastWindow();
                UnderThresholdTime = 0.0f;
            }
            if (AudioSegmenter.CheckGestureStart (ref osc, Time.deltaTime))
                AudioGestureBegan();
        } 
    }

    void IlluminateNextWindow()
    {
        if (WindowIndex < Windows.Count && WindowIndex > -1)
        { 
            SetWindowActive (WindowIndex, true);
            WindowIndex ++;

            UpdateThreshold();
            UpdateAudioLayers();
        }
    }

    void TurnOffLastWindow()
    {
        if (WindowIndex > 0)
            WindowIndex --;
        if (WindowIndex < Windows.Count && WindowIndex > -1)
            SetWindowActive (WindowIndex, false); 

        UpdateThreshold();
        UpdateAudioLayers();
    }

    void UpdateThreshold()
    {
        AudioSegmenter.VolumeThreshold = BeginningVolumeThreshold + (WindowIndex / NumWindowsPerLevel) * ThresholdIncrease;
    }

    void UpdateAudioLayers()
    {
        int layer = WindowIndex / NumWindowsPerAudioLayer;
        for (int audioLayer = 0; audioLayer < AudioLayers.Count; audioLayer++)
        {
            float volumeProportion = 0.0f;
            if (layer >= audioLayer)
            {
                int numLitWindowsInLayer = Mathf.Min (WindowIndex - (audioLayer * NumWindowsPerAudioLayer), NumWindowsPerAudioLayer);
                volumeProportion += numLitWindowsInLayer / (float) NumWindowsPerAudioLayer;
            }
            LevelTargets[audioLayer] = volumeProportion * MaxLevels[audioLayer];
            if (Mathf.Abs (LevelTargets[audioLayer] - AudioLayers[audioLayer].volume) > VolumeDistanceThreshold)
                AudioLayers[audioLayer].volume = Mathf.Lerp (AudioLayers[audioLayer].volume, LevelTargets[audioLayer], Time.deltaTime);
        }
    }

    void AudioGestureBegan()
    {
        AudioGesturePlaying = true;
        UnderThresholdTime = 0.0f;
    }

    void AudioGestureEnded()
    {
        AudioGesturePlaying = false;
        OverThresholdTime = 0.0f;
    }
}

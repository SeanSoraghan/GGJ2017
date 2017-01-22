using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityLevelController : OSCReciever
{
    public GameObject WindowsCollectionObject;
    public GameObject AudioLayersObject;

    private AudioLevelsManager LevelsManager = new AudioLevelsManager();
    private List<GameObject> Windows         = new List<GameObject>();

    public float TimeBetweenSwitches = 0.5f;

    public float BeginningVolumeThreshold = 0.1f;
    public float FinalVolumeThreshold     = 0.9f;
    public int NumWindowsPerLevel         = 4;
    public int NumWindowsPerAudioLayer    = 8;

    private float OverThresholdTime  = 0.0f;
    private float UnderThresholdTime = 0.0f;
    private float ThresholdIncrease  = 0.2f;

    private int WindowIndex = 0;

    protected override void InitialiseLevel()
    {
        if (WindowsCollectionObject != null)
            foreach (Transform child in WindowsCollectionObject.transform)
                Windows.Add (child.gameObject);

        int NumLevels = Windows.Count / NumWindowsPerLevel;
        ThresholdIncrease = (FinalVolumeThreshold - BeginningVolumeThreshold) / NumLevels;

        AudioSegmenter.VolumeThreshold = BeginningVolumeThreshold;

        LevelsManager.InitialiseFromGameObject (AudioLayersObject);

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
        base.MapFeaturesToVisualisers();

        if (AudioGesturePlaying)
        {
            OverThresholdTime += Time.deltaTime;
            if (OverThresholdTime > TimeBetweenSwitches)
            {
                IlluminateNextWindow();
                OverThresholdTime = 0.0f;
            }
        }
        else
        { 
            UnderThresholdTime += Time.deltaTime;
            if (UnderThresholdTime > TimeBetweenSwitches)
            {
                TurnOffLastWindow();
                UnderThresholdTime = 0.0f;
            }
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
            if (WindowIndex > Windows.Count - 1)
            { 
                GlobalController.GetGlobalController().CurrentLevelCompleted();
            }
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
        for (int audioLayer = 0; audioLayer < LevelsManager.AudioLayers.Count; audioLayer++)
        {
            float volumeProportion = 0.0f;
            if (layer >= audioLayer)
            {
                int numLitWindowsInLayer = Mathf.Min (WindowIndex - (audioLayer * NumWindowsPerAudioLayer), NumWindowsPerAudioLayer);
                volumeProportion += numLitWindowsInLayer / (float) NumWindowsPerAudioLayer;
            }
            LevelsManager.LevelTargets[audioLayer] = volumeProportion * LevelsManager.MaxLevels[audioLayer];
        }
        LevelsManager.UpdateLevels();
    }

    public override void AudioGestureBegan()
    {
        base.AudioGestureBegan();
        UnderThresholdTime = 0.0f;
    }

    public override void AudioGestureEnded()
    {
        base.AudioGestureEnded();
        OverThresholdTime = 0.0f;
    }

    public override void AudioRMSGestureBegan()
    {
        base.AudioRMSGestureBegan();
        UnderThresholdTime = 0.0f;
    }

    public override void AudioRMSGestureEnded()
    {
        base.AudioRMSGestureEnded();
        OverThresholdTime = 0.0f;
    }
}

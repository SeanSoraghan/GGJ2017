﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityLevelController : OSCReciever
{
    public GameObject WindowsCollectionObject;
    public GameObject AudioLayersObject;

    public List<AudioSource> AudioLayers = new List<AudioSource>();
    public List<GameObject> Windows = new List<GameObject>();

    public float TimeBetweenSwitches = 0.5f;

    public float BeginningVolumeThreshold = 0.1f;
    public float FinalVolumeThreshold = 0.9f;
    public int NumWindowsPerLevel = 4;

    public int NumWindowsPerAudioLayer = 8;

    public float AudioGestureMinTimeThreshold = 0.2f;
    public float AudioGestureTimeoutThreshold = 0.1f;
    public bool DebugRMSValue = false;
    

    private bool AudioGesturePlaying = false;
    private float TimeSinceLastOnset = 0.0f;
    private float TimeSinceLastDip = 0.0f;
    private float OverThresholdTime = 0.0f;
    private float UnderThresholdTime = 0.0f;
    private float ThresholdIncrease = 0.2f;
    private float VolumeThreshold; 

    private int WindowIndex = 0;

    protected override void InitialiseLevel()
    {
        if (WindowsCollectionObject != null)
            foreach (Transform child in WindowsCollectionObject.transform)
                Windows.Add (child.gameObject);

        int NumLevels = Windows.Count / NumWindowsPerLevel;
        ThresholdIncrease = (FinalVolumeThreshold - BeginningVolumeThreshold) / NumLevels;

        VolumeThreshold = BeginningVolumeThreshold;

        if (AudioLayersObject != null)
            foreach (AudioSource source in AudioLayersObject.GetComponents<AudioSource>())
                AudioLayers.Add (source);

        if (AudioLayers.Count > 0)
            AudioLayers[0].Play();

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
        if (DebugRMSValue)
        { 
            Debug.Log ("RMS: " + rms + " | Current Thresh: " + VolumeThreshold);
        }
        AudioGesturePlaying = AudioGesturePlaying ? (!CheckGestureEnd (rms)) : CheckGestureStart (rms);

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
        VolumeThreshold = BeginningVolumeThreshold + (WindowIndex / NumWindowsPerLevel) * ThresholdIncrease;
    }

    void UpdateAudioLayers()
    {
        int layer = WindowIndex / NumWindowsPerAudioLayer;
        for (int audioLayer = 1; audioLayer < AudioLayers.Count; audioLayer++)
        {
            if (layer >= audioLayer && !AudioLayers[audioLayer].isPlaying)
                AudioLayers[audioLayer].Play();
            if (layer < audioLayer && AudioLayers[audioLayer].isPlaying)
                AudioLayers[audioLayer].Stop();
        }
    }

    void AudioGestureBegan()
    {
        UnderThresholdTime = 0.0f;
    }

    void AudioGestureEnded()
    {
        OverThresholdTime = 0.0f;
    }

    public bool CheckGestureStart (float rms)
    {
        if (rms > VolumeThreshold)
            TimeSinceLastOnset += Time.deltaTime;
        else
            TimeSinceLastOnset = 0.0f;
        if (TimeSinceLastOnset > AudioGestureMinTimeThreshold)
        {
            AudioGestureBegan();
            return true;
        }
        return false;
    }

    public bool CheckGestureEnd (float rms)
    {
        if (rms < VolumeThreshold)
            TimeSinceLastDip += Time.deltaTime;
        else
            TimeSinceLastDip = 0.0f;
        if (TimeSinceLastDip > AudioGestureTimeoutThreshold)
        {
            AudioGestureEnded();
            return true;
        }

        return false;
    }
}

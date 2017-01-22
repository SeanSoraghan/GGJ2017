using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicListener : MonoBehaviour
{
    private static GameObject MicListenerObject;

    public float MicClipSwitchTimeout = 8.0f;
    public DynamicallyThresholdedValue RMS = new DynamicallyThresholdedValue (5, false);
    
    private const int AnalysisBufferSize = 512;
    private string MicName = "";
    private float LastMicSwitchTime = 0.0f;
    private AudioClip InputClip;
    private float[] AnalysisBuffer;

    void Awake()
    {
        DontDestroyOnLoad (transform.gameObject);
    }

	// Use this for initialization
	void Start ()
    {
	    if (Microphone.devices.Length > 0)
            MicName = Microphone.devices[0];
        AnalysisBuffer = new float[AnalysisBufferSize];
        SwitchMicClip();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        RecalculateRMS();
        
		if ((Time.time - LastMicSwitchTime) >= MicClipSwitchTimeout)
            SwitchMicClip();
	}

    private void RecalculateRMS()
    {
        int sampleStartPosition = Microphone.GetPosition (MicName) - AnalysisBufferSize;
        if (InputClip != null && sampleStartPosition >= 0)
            InputClip.GetData (AnalysisBuffer, sampleStartPosition);
        float squareSum = 0.0f;
        foreach (float sample in AnalysisBuffer)
            squareSum += Mathf.Pow (sample, 2.0f);
        float squareMean = 1.0f;
        if (AnalysisBufferSize > 0.0f)
            squareMean = squareSum / AnalysisBufferSize;
        float rms = Mathf.Sqrt (squareMean);
        RMS.setNewValueFromNewObservation (Mathf.Clamp (rms, 0.0f, 1.0f));
    }

    private void SwitchMicClip()
    {
        Microphone.End (MicName);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = Microphone.Start (MicName, true, 10, 44100);
            InputClip = audioSource.clip;
        }

        LastMicSwitchTime = Time.time;
    }

    public static MicListener GetGlobalMicListener()
    {
        if (MicListenerObject == null)
        { 
            MicListenerObject = new GameObject();
            MicListenerObject.AddComponent<MicListener>();
            MicListenerObject.AddComponent<AudioSource>();
            MicListenerObject.GetComponent<MicListener>().SwitchMicClip();
        }

        if (MicListenerObject != null)
        {
            MicListener controller = MicListenerObject.GetComponent<MicListener>();
            if (controller != null)
                return controller;
        }
        return null;
    }
}

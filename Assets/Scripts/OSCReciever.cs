using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public enum AudioFeature
{
	Onset = 0,
    RMS,
    F0,
    Centroid,
    Slope,
    Spread,
    Flatness,
    LER,
    Flux,
    HER,
    OER,
    Inharmonicity,
    NumFeatures
}

/*************************************************************************************/
/*************************************************************************************/

public class DynamicallyThresholdedValue
{
    private float[] history;
    
    private float value      = 0.0f;
    private float currentMax = 1.0f;
    private float currentMin = 0.0f;

    private int recordedHistory = 0;
    private bool followLeaps = true;

    public DynamicallyThresholdedValue (int historySize, bool fl = true)
    {
        followLeaps = fl;
        history = new float[historySize];
        for (int i = 0; i < historySize; i++)
        {
            history[i] = 0.0f;
        }
    }

    public void insertNewValueAndUpdateHistory (float newValue)
    {
        //float logValue = log10 (newValue * 9.0f + 1.0f);

        for (int index = 0; index < history.Length - 1; index ++)
        {
            history[index] = history[index + 1];
        }
        history[history.Length - 1] = newValue;//logValue;

        if (recordedHistory < (int) history.Length)
            recordedHistory ++;
    }

    public float getTotal()
    {
        float total = 0.0f;
        for (int i = 0; i < history.Length; ++i)
        {
            float value = history[i];
            total += value;
        }
        return total;
    }

    public float getRunningAverage()
    {
        if (recordedHistory < 1)
            return getTotal();

        return getTotal() / (float) recordedHistory;
    }

    public void setNewValueFromNewObservation (float newObservation)
    {
        float av       = getRunningAverage();
        float newValue = (getTotal() + newObservation) / (float) (recordedHistory + 1);

        if (followLeaps)
        {
            if (newObservation >= av * 1.8f || newObservation <= av * 0.2f)
            {
                newValue += (newObservation - newValue) * 0.5f;
            }
        }


        float eps = 0.0001f;

        if (newValue < eps)
            newValue = 0.0f;

        insertNewValueAndUpdateHistory (newValue);
        value = newValue;
    }

    
    public bool checkLatestValueForMax()
    {
        float newCandidateMax = history[history.Length - 1];

        for (int index = 0; index < history.Length - 1; index ++)
            if (history[index] > newCandidateMax)
                return false;

        return true;
    }

    public bool checkLatestValueForMin()
    {
        float newCandidateMin = history[history.Length - 1];

        for (int index = 0; index < history.Length - 1; index ++)
            if (history[index] < newCandidateMin)
                return false;

        return true;
    }

    public void checkLatestValueForMinMax()
    {
        if (checkLatestValueForMax())
            currentMax = history[history.Length - 1];

        if (checkLatestValueForMin())
            currentMin = history[history.Length - 1];
    }

    public void addNewValueToHistoryAndUpdateCurrentValue (float v)
    {
        insertNewValueAndUpdateHistory (v);
        checkLatestValueForMinMax();
        value = history[history.Length - 1] / (recordedHistory >= (int) history.Length ? currentMax : 1.0f);
    }

    public float getValue()                { return value; }
    public void  setValue (float newValue) { value = newValue; }  
    
}

/*************************************************************************************/
/*************************************************************************************/

public class OSCFeaturesInputHandler
{
    public DynamicallyThresholdedValue[] features;

    public OSCFeaturesInputHandler ()
    {
        const int numFeatures = (int) AudioFeature.NumFeatures;
		features = new DynamicallyThresholdedValue[numFeatures];
		for (int i = 0; i < (int) AudioFeature.NumFeatures; i++)
        { 
            if (i == (int) AudioFeature.HER || i == (int) AudioFeature.Inharmonicity || i == (int) AudioFeature.OER )
			    features[i] = new DynamicallyThresholdedValue (5, false);
            else 
                features[i] = new DynamicallyThresholdedValue (5, false);
        } 
    }

    public void OnFeaturesReceived( OscMessage message )
	{
        for (int i = 0; i < message.args.Count; i++)
            if (i < features.Length)
                features[i].setNewValueFromNewObservation (Mathf.Clamp ((float) message.args[i]/*logFeature*/, 0.0f, 1.0f));
	}

    public float Feature (AudioFeature f)
	{
		return features[(int)f].getValue();
	}
}

/*************************************************************************************/
/*************************************************************************************/

public class AudioGestureSegmenter
{
    public float AudioGestureMinTimeThreshold = 0.2f;
    public float AudioGestureTimeoutThreshold = 0.1f;
    public float VolumeThreshold; 
    public float LastAudioGestureLength = 0.0f;
    public float[] GestureFeatureAverages;

    private float TimeSinceLastOnset = 0.0f;
    private float TimeSinceLastDip   = 0.0f; 

    private float GestureStartTime = 0.0f;

    private int GestureFrameCount = 0;
    private float[] GestureFeatureTotals;

    public AudioGestureSegmenter()
    {
        const int numFeatures  = (int) AudioFeature.NumFeatures;
		GestureFeatureTotals   = new float[numFeatures];
        GestureFeatureAverages = new float[numFeatures];
    }

    public bool CheckGestureStart (ref OSCFeaturesInputHandler osc, float deltaTime)
    {
        float rms = osc.Feature (AudioFeature.RMS);
        if (rms > VolumeThreshold)
            TimeSinceLastOnset += deltaTime;
        else
            TimeSinceLastOnset = 0.0f;
        if (TimeSinceLastOnset > AudioGestureMinTimeThreshold)
        {
            ResetGestureProperties();
            GestureStartTime = Time.time;
            GestureFrameCount ++;
            for (int i = 0; i < (int) AudioFeature.NumFeatures; i++)
                GestureFeatureTotals[i] += osc.Feature ((AudioFeature)i);
            return true;
        }
        return false;
    }

    public bool CheckGestureEnd (ref OSCFeaturesInputHandler osc, float deltaTime)
    {
        float rms = osc.Feature (AudioFeature.RMS);
        if (rms < VolumeThreshold)
            TimeSinceLastDip += Time.deltaTime;
        else
            TimeSinceLastDip = 0.0f;
        if (TimeSinceLastDip > AudioGestureTimeoutThreshold)
        {
            LastAudioGestureLength = Time.time - GestureStartTime;

            GestureFrameCount ++;
            for (int i = 0; i < (int) AudioFeature.NumFeatures; i++)
                GestureFeatureAverages[i] = GestureFeatureTotals[i] / GestureFrameCount;

            return true;
        }
        else
        {
            GestureFrameCount ++;
            for (int i = 0; i < (int) AudioFeature.NumFeatures; i++)
                GestureFeatureTotals[i] += osc.Feature ((AudioFeature)i);

            return false;
        }
    }

    public float GetGestureFeature (AudioFeature f)
    {
        return GestureFeatureAverages[(int)f];
    }

    void ResetGestureProperties()
    {
        GestureFrameCount = 0;
        for (int i = 0; i < (int) AudioFeature.NumFeatures; i++)
        {
            GestureFeatureTotals[i]   = 0.0f;
            GestureFeatureAverages[i] = 0.0f;
        }
    }
}

/*************************************************************************************/
/*************************************************************************************/

public class OSCReciever : MonoBehaviour
{
    public float VolumeThreshold = 0.4f;
    public bool DebugAudioFeatures = false;
    public float AudioGestureMinTimeThreshold = 0.06f;
    public float AudioGestureTimeoutThreshold = 0.06f;

    protected OscIn                   OSCHandler;
    protected AudioGestureSegmenter   AudioSegmenter      = new AudioGestureSegmenter();
    protected bool                    AudioGesturePlaying = false;
    protected OSCFeaturesInputHandler osc                 = new OSCFeaturesInputHandler();
    const string                      featuresAddress     = "/Audio/A0";

    // Use this for initialization
	void Start ()
    {
        OSCHandler = gameObject.AddComponent<OscIn>();
		OSCHandler.Open (10001);
        OSCHandler.Map  (featuresAddress, OnFeaturesReceived );
        AudioSegmenter.AudioGestureMinTimeThreshold = AudioGestureMinTimeThreshold;
        AudioSegmenter.AudioGestureTimeoutThreshold = AudioGestureTimeoutThreshold;
        AudioSegmenter.VolumeThreshold = VolumeThreshold;
        InitialiseLevel();
	}

    public void OnFeaturesReceived (OscMessage m)
    {
        float rms = osc.Feature (AudioFeature.RMS);
        float pitch = osc.Feature (AudioFeature.F0);
        if (DebugAudioFeatures)
            Debug.Log ("RMS: " + rms + " | Pitch: " + pitch);

        osc.OnFeaturesReceived (m);
    }

	// Update is called once per frame
	void Update ()
    {
		MapFeaturesToVisualisers();
	}

    protected virtual void InitialiseLevel() {}

    public virtual void MapFeaturesToVisualisers() {}

    
}

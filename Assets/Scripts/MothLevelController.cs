using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothLevelController : OSCReciever
{
    public MothController Moth;

    // Use this for initialization
    protected override void InitialiseLevel ()
    {
        
    }

    public override void MapFeaturesToVisualisers ()
    {
        if (AudioGesturePlaying)
        {
            if (AudioSegmenter.CheckGestureEnd (ref osc, Time.deltaTime))
                AudioGestureEnded();
        }
        else
        { 
            if (AudioSegmenter.CheckGestureStart (ref osc, Time.deltaTime))
                AudioGestureBegan();
        }
    }

    void AudioGestureBegan()
    {
        AudioGesturePlaying = true;
        if (Moth != null && !Moth.IsRunning())
            Moth.RunAway();
    }

    void AudioGestureEnded()
    {
        AudioGesturePlaying = false;
    }
}

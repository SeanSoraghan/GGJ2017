using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothLevelController : OSCReciever
{
    public int NumRequiredSaves = 3;
    public MothController Moth;
    public float MothListenYPosition = 0.5f;

    private int NumSaves = 0;
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
        if (Moth != null && !Moth.IsRunning() && Moth.transform.position.y <= MothListenYPosition)
        { 
            Moth.RunAway();
            if (++NumSaves >= NumRequiredSaves)
                GlobalController.GetGlobalController().PlayWinSound();
        }
    }

    void AudioGestureEnded()
    {
        AudioGesturePlaying = false;
    }
}
